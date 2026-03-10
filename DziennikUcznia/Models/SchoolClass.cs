using System;
using System.Collections.Generic;
using System.Text;

namespace DziennikUcznia.Models
{
    internal class SchoolClass
    {
        public string ClassName { get; set; }
        public List<Student> Students { get; set; }

        public SchoolClass (string className)
        {
            ClassName = className; // nazwa
            Students = new List<Student>(); // lista uczniów
        }
    }
}
