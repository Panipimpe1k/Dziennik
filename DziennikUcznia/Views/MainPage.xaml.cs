using DziennikUcznia.Animations;
using DziennikUcznia.Models;
using DziennikUcznia.Services;

namespace DziennikUcznia.Views;

public partial class MainPage : ContentPage
{
    private List<Student> students = new List<Student>();
    private FileService fileService = new FileService();
    private Dictionary<string, List<Student>> allClasses;

    public MainPage()
    {
        InitializeComponent();

        allClasses = fileService.LoadClasses();

        ClassPicker.ItemsSource = allClasses.Keys.ToList();

    }

    private void OnClassChanged(object sender, EventArgs e)
    {
        string selectedClass = ClassPicker.SelectedItem as string;

        if(selectedClass != null && allClasses.ContainsKey(selectedClass))
        {
            students = allClasses[selectedClass];
            StudentsList.ItemsSource = students;
        }
    }

    private void OnAddStudent(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(StudentEntry.Text))
        {
            int newNumber = GetNextAvailableNumber();
            students.Add(new Student(newNumber, StudentEntry.Text));
            SortStudents();
            StudentEntry.Text = "";
        }
    }

    private void OnSave(object sender, EventArgs e)
    {
        if(ClassPicker.SelectedItem != null)
        {
            string className = ClassPicker.SelectedItem.ToString();
            fileService.SaveClass(className, students);
            DisplayAlert("Sukces", "Zapisano do pliku", "OK");
        }
    }
    private int GetNextAvailableNumber()
    {
        var numbers = students.Select(s => s.Number).OrderBy(n => n).ToList();
        int expected = 1;
        foreach (var n in numbers)
        {
            if (n != expected)
                return expected;
            expected++;
        }
        return expected;
    }


    private void OnDrawStudent(object sender, EventArgs e)
    {
        var presentStudents = students.Where(s => s.IsPresent).ToList();

        if (presentStudents.Count > 0)
        {
            Random random = new Random();
            int index = random.Next(presentStudents.Count);
            var drawnStudent = presentStudents[index];

            DrawnStudentLabel.Text = $"Wylosowany: {drawnStudent.FullName} (#{drawnStudent.Number})";
        }
        else
        {
            DisplayAlert("Brak obecnych", "Nie ma żadnego obecnego ucznia do wylosowania", "OK");
        }
    }

    private void OnMarkAllPresent(object sender, EventArgs e)
    {
        foreach (var student in students)
            student.IsPresent = true;

        StudentsList.ItemsSource = null;
        StudentsList.ItemsSource = students;
    }


    private void OnRemoveStudent(object sender, EventArgs e)
    {
        var selectedStudent = StudentsList.SelectedItem as Student;

        if (selectedStudent != null)
        {
            students.Remove(selectedStudent);

            StudentsList.ItemsSource = null;
            StudentsList.ItemsSource = students;
        }
    }

    private void OnAddClass(object sender, EventArgs e)
    {
        string newClass = NewClassEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(newClass) && !allClasses.ContainsKey(newClass))
        {
            allClasses[newClass] = new List<Student>();
            ClassPicker.ItemsSource = allClasses.Keys.ToList();
            ClassPicker.SelectedItem = newClass;
            NewClassEntry.Text = "";
        }
    }

    private void SortStudents()
    {
        students = students.OrderBy(s => s.Number).ToList();
        StudentsList.ItemsSource = null;
        StudentsList.ItemsSource = students;
    }

    private async void OnDrawLuckyNumber(object sender, EventArgs e)
    {
        int maxCount = allClasses.Values.Any() ? allClasses.Values.Max(c => c.Count) : 1;
        Random rnd = new Random();
        int luckyNumber = rnd.Next(1, maxCount + 1);

        LuckyNumberLabel.Text = $"Szczęśliwy numerek: {luckyNumber}";

        foreach (var s in students)
            s.IsLucky = (s.Number == luckyNumber);

        StudentsList.ItemsSource = null;
        StudentsList.ItemsSource = students;

        var animation = new CarAnimation(CarBody, WheelLeft, WheelRight, CarNumberLabel);
        await animation.PlayAsync(luckyNumber);

    }
}
