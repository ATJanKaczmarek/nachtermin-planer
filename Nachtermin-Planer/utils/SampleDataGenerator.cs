// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace Nachtermin_Planer
// {
//   public static class SampleDataGenerator
//   {
//     public static List<StudentDataModel> GenerateSampleStudents()
//     {
//       List<StudentDataModel> students = new List<StudentDataModel>
//             {
//                 new StudentDataModel { Id = Guid.NewGuid().ToString(), FirstName = "Max", LastName = "Mustermann", Class = "12A" },
//                 new StudentDataModel { Id = Guid.NewGuid().ToString(), FirstName = "Anna", LastName = "Müller", Class = "11B" },
//                 new StudentDataModel { Id = Guid.NewGuid().ToString(), FirstName = "Lukas", LastName = "Schmidt", Class = "10C" },
//                 new StudentDataModel { Id = Guid.NewGuid().ToString(), FirstName = "Sophie", LastName = "Wagner", Class = "9D" },
//                 // Weitere Beispieldaten hinzufügen
//             };

//       return students;
//     }

//     public static List<Supervison> GenerateSampleEntries(List<StudentDataModel> students)
//     {
//       List<Supervison> entries = new List<Supervison>();
//       Random rand = new Random();

//       foreach (var student in students)
//       {
//         // Generiere 15 Einträge für Slot 1
//         for (int i = 0; i < 15; i++)
//         {
//           entries.Add(new Supervison
//           {
//             Id = rand.Next(1000).ToString(), // Eindeutigen Primärschlüssel generieren
//             StudentId = student.Id,
//             TimeInMinutes = rand.Next(30, 120), // Zufällige Dauer zwischen 30 und 120 Minuten
//             TypeOfAssessment = "Test" // Beispielwert für die Art des Leistungsnachweises
//           });
//         }
//       }

//       // Stelle sicher, dass in jedem Slot mindestens 15 Elemente liegen
//       var slot1Entries = entries.Take(15).ToList();
//       var slot2Entries = entries.Skip(15).Take(15).ToList();
//       entries.Clear();
//       entries.AddRange(slot1Entries);
//       entries.AddRange(slot2Entries);

//       return entries;
//     }





//   }
// }
