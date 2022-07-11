using System;

namespace AlgoPlus.Storage
{
    public class ReturnFileInfo
    {
        public string AbsolutePath { get; set; }
        public string RelativePath { get; set; }
        public string Filename { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public long? ContentLength { get; set; }
    }
}
