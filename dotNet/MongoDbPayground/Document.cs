using System;
using System.Collections.Generic;
using MongoDb.Playground.Repository;

namespace MongoDbPayground
{
    public class Document : ICollectionDocument
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public DateTime Timestamp { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IDictionary<string, string> Properties { get; set; }
    }
}