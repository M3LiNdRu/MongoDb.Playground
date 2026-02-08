using System;
namespace MongoDbPlayground.Test.Models
{
    public class TopSecret : DocumentType
    {
        public string Origin { get; set; }
        public DateTime CreationDate { get; set; }
        public string Author { get; set; }
    }
}
