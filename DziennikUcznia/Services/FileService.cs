using DziennikUcznia.Models;

namespace DziennikUcznia.Services
{
    internal class FileService
    {
        private string filePath;

        public FileService()
        {
            filePath = Path.Combine(FileSystem.AppDataDirectory, "classes.txt");
        }

        public void SaveClass(string className, List<Student> students)
        {
            var allClasses = LoadClasses();
            allClasses[className] = students;

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                foreach (var schoolClass in allClasses)
                {
                    writer.WriteLine($"[{schoolClass.Key}]");

                    foreach (var student in schoolClass.Value)
                    {
                        writer.WriteLine($"{student.Number}|{student.FullName}");
                    }

                    writer.WriteLine();
                }
            }
        }

        public Dictionary<string, List<Student>> LoadClasses()
    {
        var result = new Dictionary<string, List<Student>>();

        if (!File.Exists(filePath))
            return result;

        string currentClass = "";

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                currentClass = line.Trim('[', ']');
                result[currentClass] = new List<Student>();
            }
            else if (!string.IsNullOrWhiteSpace(line))
            {
                // Linia w formacie "numer|imie"
                var parts = line.Split('|');
                if(parts.Length == 2)
                {
                    int number = int.Parse(parts[0]);
                    string name = parts[1];

                    result[currentClass].Add(new Student(number, name));
                }
            }
        }

        return result;
    }

    }
}
