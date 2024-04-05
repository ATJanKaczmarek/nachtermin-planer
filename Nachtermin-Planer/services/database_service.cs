using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SQLite;

namespace Nachtermin_Planer.Data
{
  public class DatabaseHandler
  {
    private SQLiteAsyncConnection? _conn;
    public async Task Init()
    {
      if (_conn is not null)
      {
        return;
      }

      _conn = new SQLiteAsyncConnection(FileAccessHelper.GetLocalFilePath("database.db3"));

      await _conn.CreateTableAsync<StudentDataModel>();
      await _conn.CreateTableAsync<Supervision>();

      Debug.WriteLine("ASDF:" + await _conn.GetTableInfoAsync("Students"));
    }

    public async Task AddStudent(string firstName, string lastName, string Class)
    {
      try
      {
        var student = new StudentDataModel
        {
          FirstName = firstName,
          LastName = lastName,
          Class = Class,
        };

        await _conn!.InsertAsync(student);
        Debug.WriteLine($"New student added: {student.FirstName} {student.LastName}");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error adding student: {ex.Message}");
      }
    }
    public async Task AddStudent(StudentDataModel newStudent)
    {
      ArgumentNullException.ThrowIfNull(newStudent);
      try
      {
        await _conn!.InsertAsync(newStudent);
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error adding student: {ex.Message}");
      }
    }
    public async Task<List<StudentDataModel>> GetAllStudents()
    {
      Console.WriteLine("CALLED GETALLSTUDENTS()");

      var all = await _conn.QueryAsync<StudentDataModel>("SELECT * FROM Students");

      return all;
    }
    public async Task<StudentDataModel> GetStudentFromId(int id)
    {
      Console.WriteLine("CALLED GETALLSTUDENTS()");

      var result = await _conn.QueryAsync<StudentDataModel>($"SELECT * FROM Students WHERE Students.id=={id}");

      return result[0];
    }

    public async Task<List<Supervision>> GetAllSupervisions()
    {
      return await _conn.Table<Supervision>().ToListAsync();
    }

    public async Task DeleteStudent(int studentId)
    {
      ArgumentNullException.ThrowIfNull(studentId);
      try
      {
        var studentToDelete = await _conn.GetAsync<StudentDataModel>(studentId);
        if (studentToDelete != null)
        {
          await _conn.DeleteAsync(studentToDelete);
          Debug.WriteLine($"Student with Id {studentId} deleted.");
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error deleting student: {ex.Message}");
      }
    }
    public async Task AddSupervision(Supervision newSupervision)
    {
      try
      {
        await _conn!.InsertAsync(newSupervision);
        Debug.WriteLine("New supervision added.");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error adding supervision: {ex.Message}");
      }
    }

    public async Task AddSupervision(SupervisionWithStudentInfo newSupervision)
    {
      try
      {
        // Zuerst den Schüler in die Datenbank einfügen (falls er noch nicht vorhanden ist)
        await AddStudent(newSupervision.Student);

        // Dann die Supervision einfügen
        var supervision = new Supervision
        {
          StudentId = newSupervision.Student.Id,
          TimeInMinutes = newSupervision.TimeInMinutes,
          TypeOfAssessment = newSupervision.TypeOfAssessment,
          Slot = newSupervision.Slot,
          Teacher = newSupervision.Teacher,
          Subject = newSupervision.Subject
        };

        await _conn.InsertAsync(supervision);
        Debug.WriteLine("New supervision added.");
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error adding supervision: {ex.Message}");
      }
    }

    public async Task<List<SupervisionWithStudentInfo>> GetAllSupervisionsWithStudentInfo()
    {
      var supervisions = await GetAllSupervisions();
      var supervisionWithStudentInfoList = new List<SupervisionWithStudentInfo>();

      foreach (var supervision in supervisions)
      {
        var student = await _conn.Table<StudentDataModel>().Where(s => s.Id == supervision.StudentId).FirstOrDefaultAsync();
        supervisionWithStudentInfoList.Add(new SupervisionWithStudentInfo
        {
          Id = supervision.Id,
          Student = student,
          TimeInMinutes = supervision.TimeInMinutes,
          TypeOfAssessment = supervision.TypeOfAssessment,
          Slot = supervision.Slot,
          Teacher = supervision.Teacher,
          Subject = supervision.Subject
        });
      }

      return supervisionWithStudentInfoList;
    }

    public async Task DeleteSupervision(int supervisionId)
    {
      try
      {
        var supervisionToDelete = await _conn.GetAsync<Supervision>(supervisionId);
        if (supervisionToDelete != null)
        {
          await _conn.DeleteAsync(supervisionToDelete);
          Debug.WriteLine($"Supervision with Id {supervisionId} deleted.");
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error deleting supervision: {ex.Message}");
      }
    }


  }

  public class SupervisionWithStudentInfo
  {
    public int Id { get; set; } // Eindeutiger Primärschlüssel

    public StudentDataModel Student { get; set; }

    public int TimeInMinutes { get; set; }

    public string TypeOfAssessment { get; set; }

    public int Slot { get; set; } // Slot, in dem der Eintrag platziert ist (Slot 1 oder Slot 2)

    public string Teacher { get; set; }

    public string Subject { get; set; }

    public SupervisionWithStudentInfo() { }

    public SupervisionWithStudentInfo(Supervision supervision, StudentDataModel student)
    {
      Id = supervision.Id;
      Student = student;
      TimeInMinutes = supervision.TimeInMinutes;
      TypeOfAssessment = supervision.TypeOfAssessment;
      Slot = supervision.Slot;
      Teacher = supervision.Teacher;
      Subject = supervision.Subject;
    }
  }
}
