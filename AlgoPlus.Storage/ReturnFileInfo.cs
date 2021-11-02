using System;

namespace AlgoPlus.Storage
{
    public class ReturnFileInfo
    {
        public string AbsolutePath { get; set; }
        public string RelativePath { get; set; }
        public string Filename { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModified { get; set; }
        public long? ContentLength { get; set; }
    }
}
