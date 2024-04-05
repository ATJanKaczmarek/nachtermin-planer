using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Nachtermin_Planer.Data;

namespace Nachtermin_Planer
{
	public partial class MainPage : ContentPage
	{
		private ObservableCollection<SupervisionWithStudentInfo> slot1Entries = new ObservableCollection<SupervisionWithStudentInfo>();
		private ObservableCollection<SupervisionWithStudentInfo> slot2Entries = new ObservableCollection<SupervisionWithStudentInfo>();

		public MainPage()
		{
			InitializeComponent();
			LoadPageAsync();
		}

		private async void LoadPageAsync()
		{
			await LoadData();
			CheckAndHighlightOverlappingEntries();
		}

		private DatabaseHandler db = new DatabaseHandler();
		private async Task LoadData()
		{
			await db.Init();
			await LoadStudentsFromDatabase();
			await LoadSupervisionsFromDatabase();
		}

		private async Task LoadStudentsFromDatabase()
		{
			var students = await db.GetAllStudents();

			if (students.Any())
			{
				ObservableCollection<StudentDataModel> studentCollection = new ObservableCollection<StudentDataModel>(students);

				studentListView.ItemsSource = studentCollection;
			}
			else
			{
				await DisplayAlert("Hinweis", "Keine Schüler gefunden.", "OK");
			}
		}

		private async Task LoadSupervisionsFromDatabase()
		{
			var supervisions = await db.GetAllSupervisionsWithStudentInfo();
			if (supervisions.Any())
			{
				ObservableCollection<SupervisionWithStudentInfo> supervisionCollection = new ObservableCollection<SupervisionWithStudentInfo>(supervisions);

				foreach (var entry in supervisionCollection)
				{
					if (entry.Slot == 1)
					{
						slot1Entries.Add(entry);
					}
					else if (entry.Slot == 2)
					{
						slot2Entries.Add(entry);
					}
				}

				slot1ListView.ItemsSource = slot1Entries;
				slot2ListView.ItemsSource = slot2Entries;
			}
			else
			{
				await DisplayAlert("Hinweis", "Keine Überwachungseinträge gefunden.", "OK");
			}
		}


		private async void OnAddStudentButtonClicked(object sender, EventArgs e)
		{
			var all = await db.GetAllStudents();
			foreach (var item in all)
			{
				Console.WriteLine(item);
			}
			var student = new StudentDataModel
			{
				Class = classEntry.Text,
				FirstName = firstNameEntry.Text,
				LastName = lastNameEntry.Text
			};
			await db.AddStudent(student);

			ClearInputFields();
			await LoadStudentsFromDatabase();
		}

		private StudentDataModel selectedStudent;

		private async void OnStudentSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
				return;

			selectedStudent = e.SelectedItem as StudentDataModel;
		}

		private async void OnDeleteButtonClicked(object sender, EventArgs e)
		{
			if (sender is Button button && button.CommandParameter is int studentId)
			{
				await db.DeleteStudent(studentId);
				await LoadStudentsFromDatabase();

				CheckAndHighlightOverlappingEntries();
			}
		}


		private async void OnAddSupervisionButtonClicked(object sender, EventArgs e)
		{
			if (studentListView.SelectedItem == null)
			{
				await DisplayAlert("Hinweis", "Bitte wählen Sie einen Schüler aus.", "OK");
				return;
			}

			var selectedStudent = studentListView.SelectedItem as StudentDataModel;

			if (string.IsNullOrWhiteSpace(timeEntry.Text) || string.IsNullOrWhiteSpace(typeEntry.Text) || string.IsNullOrWhiteSpace(teacherEntry.Text) || string.IsNullOrWhiteSpace(subjectEntry.Text))
			{
				await DisplayAlert("Hinweis", "Bitte füllen Sie alle Felder aus.", "OK");
				return;
			}

			if (!int.TryParse(timeEntry.Text, out int timeInMinutes))
			{
				await DisplayAlert("Hinweis", "Bitte geben Sie eine gültige Zeit in Minuten ein.", "OK");
				return;
			}

			var newSupervision = new Supervision
			{
				StudentId = selectedStudent.Id,
				TimeInMinutes = timeInMinutes,
				TypeOfAssessment = typeEntry.Text,
				Slot = slot1RadioButton.IsChecked ? 1 : slot2RadioButton.IsChecked ? 2 : 0,
				Teacher = teacherEntry.Text,
				Subject = subjectEntry.Text
			};

			await db.AddSupervision(newSupervision);

			var fetchStudent = await db.GetStudentFromId(newSupervision.StudentId);
			Debug.WriteLine($"got student: f_n{fetchStudent.FirstName} l_n{fetchStudent.LastName}");
			if (newSupervision.Slot == 1)
			{
				slot1Entries.Add(new SupervisionWithStudentInfo(newSupervision, fetchStudent));
				slot1ListView.ItemsSource = null;
				slot1ListView.ItemsSource = slot1Entries;
			}
			else if (newSupervision.Slot == 2)
			{
				slot2Entries.Add(new SupervisionWithStudentInfo(newSupervision, fetchStudent));
				slot2ListView.ItemsSource = null;
				slot2ListView.ItemsSource = slot2Entries;
			}

			ClearInputFields();

			CheckAndHighlightOverlappingEntries();
		}





		private async void OnSearchTextChanged(object sender, EventArgs e)
		{

		}

		private void ClearInputFields()
		{
			firstNameEntry.Text = string.Empty;
			lastNameEntry.Text = string.Empty;
			classEntry.Text = string.Empty;
			subjectEntry.Text = string.Empty;
			teacherEntry.Text = string.Empty;
			timeEntry.Text = string.Empty;
			typeEntry.Text = string.Empty;
		}

		private async void OnDeleteSupervisionClicked(object sender, EventArgs e)
		{
			if (sender is Button button && button.CommandParameter is int supervisionId)
			{
				await db.DeleteSupervision(supervisionId);

				var supervisionToRemove = slot1Entries.FirstOrDefault(s => s.Id == supervisionId);
				if (supervisionToRemove != null)
				{
					slot1Entries.Remove(supervisionToRemove);
				}

				supervisionToRemove = slot2Entries.FirstOrDefault(s => s.Id == supervisionId);
				if (supervisionToRemove != null)
				{
					slot2Entries.Remove(supervisionToRemove);
				}
			}
		}

		private void CheckAndHighlightOverlappingEntries()
		{
			DateTime startOfSlot2 = DateTime.Today.AddHours(15).AddMinutes(30);

			for (int i = 0; i < slot1Entries.Count; i++)
			{
				var supervision1 = slot1Entries[i];
				DateTime endTime1 = DateTime.Today.AddHours(14).AddMinutes(15 + supervision1.TimeInMinutes);

				if (endTime1 > startOfSlot2)
				{
					for (int j = 0; j < slot2Entries.Count; j++)
					{
						var supervision2 = slot2Entries[j];
						if (supervision1.Student.Id == supervision2.Student.Id)
						{
							DateTime endTime2 = DateTime.Today.AddHours(15).AddMinutes(30 + supervision2.TimeInMinutes);

							var cell1 = FindListViewCell(slot1ListView, supervision1);
							var cell2 = FindListViewCell(slot2ListView, supervision2);

							if (endTime1 > startOfSlot2 && endTime2 > DateTime.Today.AddHours(15).AddMinutes(30))
							{

								if (cell1 != null && cell2 != null)
								{
									cell1.View.BackgroundColor = Colors.Red;
									cell2.View.BackgroundColor = Colors.Red;
								}
							}
							else
							{
								if (cell1 != null && cell2 != null)
								{
									cell1.View.BackgroundColor = Colors.Transparent;
									cell2.View.BackgroundColor = Colors.Transparent;
								}
							}
						}
					}
				}
			}
		}





		private ViewCell FindListViewCell(ListView listView, object bindingContext)
		{
			foreach (var cell in listView.TemplatedItems)
			{
				if (cell.BindingContext == bindingContext)
				{
					return cell as ViewCell;
				}
			}
			return null;
		}
	}
}
