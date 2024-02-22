using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using WpfApp1;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Note> Notes { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Notes = new ObservableCollection<Note>();
            List.ItemsSource = Notes;
        }

        private void List_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (List.SelectedItem != null)
            {
                Note selectedNote = (Note)List.SelectedItem;
                Name.Text = selectedNote.Title;
                Description.Text = selectedNote.Description;
                Date.SelectedDate = selectedNote.Date;
            }
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            Note newNote = new Note
            {
                Title = Name.Text,
                Description = Description.Text,
                Date = Date.SelectedDate ?? DateTime.Now
            };

            Notes.Add(newNote);
            ClearFields();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            {
                if (List.SelectedItem != null)
                {
                    Note selectedNote = (Note)List.SelectedItem;
                    selectedNote.Title = Name.Text;
                    selectedNote.Description = Description.Text;
                    selectedNote.Date = Date.SelectedDate ?? DateTime.Now;

                    ClearFields();
                }
            }

        }
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (List.SelectedItem != null)
            {
                Note selectedNote = (Note)List.SelectedItem;
                Notes.Remove(selectedNote);
                ClearFields();
            }
        }

        private void SelectDate(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateListByDate();
        }

        private void ClearFields()
        {
            Name.Text = "";
            Description.Text = "";
            Date.SelectedDate = DateTime.Now;
            List.SelectedItem = null;
        }

        private void UpdateListByDate()
        {
            DateTime selectedDate = Date.SelectedDate ?? DateTime.Now;
            ObservableCollection<Note> filteredNotes = new ObservableCollection<Note>();

            foreach (Note note in Notes)
            {
                if (note.Date.Date == selectedDate.Date)
                {
                    filteredNotes.Add(note);
                }
            }

            List.ItemsSource = filteredNotes;
        }

        private void SerializeNotes()
        {
            // Сериализовать ObservableCollection<Note> в файл
            SerializationUtils.SerializeToFile(Notes, "notes.xml");
        }
        private void DeserializeNotes()
        {
            Notes = SerializationUtils.DeserializeFromFile<ObservableCollection<Note>>("notes.xml");
            List.ItemsSource = Notes;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeserializeNotes();
        }
    }

    public class Note
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public Note()
        {
        }
    }

    public static class SerializationUtils
    {
        public static void SerializeToFile<T>(T data, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, data);
            }
        }

        public static T DeserializeFromFile<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(reader);
                }
            }
            else
            {
                return default(T);
            }
        }
    }
}
    
