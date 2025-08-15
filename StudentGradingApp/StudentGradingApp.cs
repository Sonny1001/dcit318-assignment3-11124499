using System;
using System.Collections.Generic;
using System.IO;

namespace StudentGradingApp
{
    // a) Student class
    public class Student
    {
        public int Id;
        public string FullName = string.Empty; // default to avoid null warning
        public int Score;

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }
    }

    // b,c) Custom exception classes
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // d) Processor class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using var reader = new StreamReader(inputFilePath);
            string? line;
            int lineNo = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 3)
                    throw new MissingFieldException($"Line {lineNo}: expected 3 fields, got {parts.Length}");

                var idStr = parts[0].Trim();
                var name = parts[1].Trim();
                var scoreStr = parts[2].Trim();

                if (!int.TryParse(idStr, out var id))
                    throw new InvalidScoreFormatException($"Line {lineNo}: invalid Id '{idStr}'");

                if (!int.TryParse(scoreStr, out var score))
                    throw new InvalidScoreFormatException($"Line {lineNo}: invalid score '{scoreStr}'");

                students.Add(new Student { Id = id, FullName = name, Score = score });
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using var writer = new StreamWriter(outputFilePath);
            foreach (var s in students)
            {
                writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
            }
        }
    }

    public class Program
    {
        // e) Main application flow
        public static void Main()
        {
            try
            {
                string input = "students_input.txt";
                string output = "students_report.txt";

                // Create sample file if missing
                if (!File.Exists(input))
                {
                    File.WriteAllLines(input, new[]
                    {
                        "101, Alice Smith, 84",
                        "102, Bernard Mensah, 73",
                        "103, Cynthia Ofori, 65",
                        "104, David Owusu, 49"
                    });
                    Console.WriteLine($"Sample input file '{input}' created.");
                }

                var processor = new StudentResultProcessor();
                var students = processor.ReadStudentsFromFile(input);
                processor.WriteReportToFile(students, output);

                Console.WriteLine($"Report written to {output}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"[FileNotFound] {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"[InvalidScoreFormat] {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"[MissingField] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UnknownError] {ex.Message}");
            }
        }
    }
}
