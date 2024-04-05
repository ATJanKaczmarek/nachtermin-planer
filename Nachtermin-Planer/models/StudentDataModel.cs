using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using ColumnAttribute = SQLite.ColumnAttribute;
using TableAttribute = SQLite.TableAttribute;

namespace Nachtermin_Planer
{
  [Table("Students")]
  public class StudentDataModel
  {
    [PrimaryKey, AutoIncrement, Unique]
    [Column("id")]
    public int Id { get; set; }
    [Column("first_name")]
    public string FirstName { get; set; }
    [Column("last_name")]
    public string LastName { get; set; }
    [Column("class")]
    public string Class { get; set; }
  }
}
