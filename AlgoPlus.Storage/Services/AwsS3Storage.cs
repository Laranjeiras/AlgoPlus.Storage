using AlgoPlus.Storage.Configs;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoPlus.Storage.Services
{
    public class AwsS3Storage : IStorage
    {
        private readonly string bucketname;
        private readonly IAmazonS3 client;
        private readonly Amazon.RegionEndpoint regionEndpoint;
        private readonly string name;
        public string Name => name;
        public string BasePath => null;

        public AwsS3Storage(AwsS3Config config, string name = null)
        {
            this.regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(config.RegionEndPoint);
            this.bucketname = config.Bucketname;
            var awsCredential = new BasicAWSCredentials(config.AccessKey, config.SecretKey);
            client = new AmazonS3Client(awsCredential, this.regionEndpoint);
            this.name = name ?? nameof(AwsS3Storage);
        }

        public async Task<ReturnFileInfo> SaveAsync(string filename, string content)
        {
            if (string.IsNullOrEmpty(bucketname))
                throw new ArgumentNullException(nameof(bucketname));
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    ContentBody = content,
                    BucketName = bucketname,
                    Key = filename
                };
                request.Metadata.Add("title", "AlgoPlus.Storage");

                PutObjectResponse response = await client.PutObjectAsync(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                }
            }

            return new ReturnFileInfo { Filename = filename, AbsolutePath = "TESTE" };
        }

        public async Task<ReturnFileInfo> SaveAsync(string filename, byte[] content)
        {
            var str = Encoding.ASCII.GetString(content);
            return await SaveAsync(filename, str);
        }

        public async Task<bool> DeleteAsync(string path)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = bucketname,
                    Key = path
                };

                var response = await client.DeleteObjectAsync(request);
                return true;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when deleting an object", amazonS3Exception.Message);
                }
                return false;
            }
        }

        public async Task<byte[]> GetFileAsync(string filename)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = bucketname,
                    Key = filename
                };

                using (GetObjectResponse response = await client.GetObjectAsync(request))
                {
                    string title = response.Metadata["x-amz-meta-title"];
                    string contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);

                    byte[] responseByte;
                    var ms = new MemoryStream();
                    using (Stream responseStream = response.ResponseStream)
                    {
                        responseStream.CopyTo(ms);
                        responseByte = ms.ToArray();
                    }
                    return responseByte;
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message);
                }
            }
            return null;
        }

        public async Task<IList<ReturnFileInfo>> GetFilesAsync()
        {
            try
            {
                ListObjectsRequest request = new ListObjectsRequest()
                {
                    BucketName = bucketname
                };

                ListObjectsResponse response = await client.ListObjectsAsync(request);

                return response.S3Objects
                    .Select(x => new ReturnFileInfo { AbsolutePath = x.Key, Filename = x.Key, RelativePath = x.BucketName })
                    .ToList();

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when listing objects", amazonS3Exception.Message);
                }
            }
            return new List<ReturnFileInfo>();
        }

        private async Task<IList<string>> ListBuckets()
        {
            try
            {
                ListBucketsResponse response = await client.ListBucketsAsync();
                var files = response.Buckets;
                return files.Select(x => x.BucketName).ToList();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine("An Error, number {0}, occurred when listing buckets with the message '{1}", amazonS3Exception.ErrorCode, amazonS3Exception.Message);
                }
            }
            return new List<string>();
        }
    }
}
