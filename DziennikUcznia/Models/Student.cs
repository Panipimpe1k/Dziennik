using System.ComponentModel;

namespace DziennikUcznia.Models
{
    public class Student
    {
        public int Number { get; set; }
        public string FullName { get; set; }

        private bool isPresent = true;
        public bool IsPresent
        {
            get => isPresent;
            set
            {
                isPresent = value;
                OnPropertyChanged(nameof(IsPresent));
            }
        }

        private bool isLucky;
        public bool IsLucky
        {
            get => isLucky;
            set
            {
                isLucky = value;
                OnPropertyChanged(nameof(IsLucky));
            }
        }

        public Student(int number, string fullName)
        {
            Number = number;
            FullName = fullName;
            IsPresent = true;
            IsLucky = false;
        }

        public override string ToString()
        {
            return $"{Number}. {FullName}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}