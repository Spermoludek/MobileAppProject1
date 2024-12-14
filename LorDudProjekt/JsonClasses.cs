using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorDudProjekt
{

    public class Bible
    {
        public string? abbreviation { get; set; }
        public string? description { get; set; }
        public string? language { get; set; }
        public string? name { get; set; }
    }

    public class Books
    {
        public string? abbreviation { get; set; }
        public Book[]? books { get; set; }
    }

    public class Book
    {
        public string? abbreviation { get; set; }
        public int? chapters { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
    }

    public class Chapter
    {
        public int? chapter { get; set; }
        public string? type { get; set; }
        public Verse[]? verses { get; set; }
    }

    public class Verse
    {
        public string? text { get; set; }
        public string? verse { get; set; }
    }

}
