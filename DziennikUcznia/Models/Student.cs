namespace DziennikUcznia.Models
{
    public class Student
    {
        public int Number { get; set; }
        public string FullName { get; set; }
        public bool IsLucky { get; set; } = false; 
        public bool IsPresent { get; set; } = true;

        public Student(int number, string fullName)
        {
            Number = number;
            FullName = fullName;
        }

        public override string ToString()
        {
            return $"{Number}. {FullName}";
        }
    }
}