

namespace PQS.Test.DummyOData.Models
{
    using System;
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime? DiscontinuedDate { get; set; }
        public int Rating { get; set; }
        public double Price { get; set; }
    }
}
