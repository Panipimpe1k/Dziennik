using DziennikUcznia.Animations;
using DziennikUcznia.Models;
using DziennikUcznia.Services;

namespace DziennikUcznia.Views;

public partial class MainPage : ContentPage
{
    private List<Student> students = new List<Student>();
    private FileService fileService = new FileService();
    private Dictionary<string, List<Student>> allClasses;
    private int? currentLuckyNumber = null;


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
            UpdateLuckyHighlight();
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
        var availableStudents = students
            .Where(s => s.IsPresent &&
                   (!currentLuckyNumber.HasValue || s.Number != currentLuckyNumber.Value))
            .ToList();

        if (availableStudents.Count > 0)
        {
            Random random = new Random();
            var drawnStudent = availableStudents[random.Next(availableStudents.Count)];

            DrawnStudentLabel.Text =
                $"Wylosowany: {drawnStudent.FullName} (#{drawnStudent.Number})";
        }
        else
        {
            DisplayAlert("Brak dostępnych",
                "Nie można wylosować – szczęśliwy numerek jest wykluczony.",
                "OK");
        }
    }

    private void OnMarkAllPresent(object sender, EventArgs e)
    {
        foreach (var student in students)
            student.IsPresent = true;

        StudentsList.ItemsSource = null;
        StudentsList.ItemsSource = students;
    }


    private void OnDeleteStudent(object sender, EventArgs e)
    {
        var button = sender as Button;
        var student = button?.CommandParameter as Student;

        if (student != null)
        {
            students.Remove(student);

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
        if (!allClasses.Any())
        {
            await DisplayAlert("Brak klas", "Nie ma żadnej klasy.", "OK");
            return;
        }

        var allStudents = allClasses.Values.SelectMany(c => c).ToList();

        if (allStudents.Count == 0)
        {
            await DisplayAlert("Brak uczniów", "Nie ma uczniów.", "OK");
            return;
        }

        int maxNumber = allStudents.Max(s => s.Number);

        Random rnd = new Random();
        currentLuckyNumber = rnd.Next(1, maxNumber + 1);

        LuckyNumberLabel.Text = $"Szczęśliwy numerek: {currentLuckyNumber}";
        LuckyNumberLabel.TextColor = Colors.Green;

        UpdateLuckyHighlight();

        var animation = new CarAnimation(CarBody, WheelLeft, WheelRight, CarNumberLabel);
        await animation.PlayAsync(currentLuckyNumber.Value);
    }

    private void UpdateLuckyHighlight()
    {
        foreach (var s in students)
            s.IsLucky = (currentLuckyNumber.HasValue && s.Number == currentLuckyNumber.Value);
    }
}
