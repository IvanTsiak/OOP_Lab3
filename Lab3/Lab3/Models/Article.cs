using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Lab3.Models
{
    public class Author
    {
        public string Name { get; set; }
        public string Position { get; set; }
    }

    public class  Review
    {
        public string User {  get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
    }
    public class Article
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }
        public string Author { get; set; }
        public string Department { get; set; }
        public string Category { get; set; }
        public string Annotation { get; set; }

    }
}
