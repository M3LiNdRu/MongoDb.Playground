using System;
using System.Collections.Generic;
using MongoDb.Playground.Repository;
using MongoDbPlayground.Test.Models;

namespace MongoDbPayground
{
    public class Document<T> : ICollectionDocument where T : DocumentType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public DateTime Timestamp { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IDictionary<string, string> Properties { get; set; }
        public T Special { get; set; }
        public Base Polimorfism { get; set; }
    }
}