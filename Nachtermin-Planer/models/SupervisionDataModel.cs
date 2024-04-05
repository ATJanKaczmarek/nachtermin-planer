using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using TableAttribute = SQLite.TableAttribute;
using ColumnAttribute = SQLite.ColumnAttribute;

namespace Nachtermin_Planer
{
    [Table("Supervisions")]
    public class Supervision
    {
        [PrimaryKey, AutoIncrement, NotNull]
        [Column("id")]
        public int Id { get; set; }
        [Column("student_id")]
        public int StudentId { get; set; }
        [Column("time_in_mins")]
        public int TimeInMinutes { get; set; }
        [Column("type_of_assessment")]
        public string TypeOfAssessment { get; set; }
        [Column("slot")]
        public int Slot { get; set; }
        [Column("teacher")]
        public string Teacher { get; set; }

        [Column("subject")]
        public string Subject { get; set; }

    }
}
