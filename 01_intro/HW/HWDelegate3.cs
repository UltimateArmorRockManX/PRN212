
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DelegatesLinQ.Homework
{
    public class LinqDataProcessor
    {
        private List<Student> _students;

        public LinqDataProcessor()
        {
            _students = GenerateSampleData();
        }

        public void BasicQueries()
        {
            Console.WriteLine("=== BASIC LINQ QUERIES ===");
            var highGPA = _students.Where(s => s.GPA > 3.5);
            Console.WriteLine("High GPA Students:");
            foreach (var s in highGPA)
                Console.WriteLine($"- {s.Name} ({s.GPA})");

            var groupedByMajor = _students.GroupBy(s => s.Major);
            foreach (var group in groupedByMajor)
            {
                Console.WriteLine($"Major: {group.Key}");
                foreach (var s in group)
                    Console.WriteLine($"  - {s.Name}");
            }

            var avgByMajor = _students
                .GroupBy(s => s.Major)
                .Select(g => new { Major = g.Key, AvgGPA = g.Average(s => s.GPA) });

            Console.WriteLine("Average GPA by Major:");
            foreach (var m in avgByMajor)
                Console.WriteLine($"- {m.Major}: {m.AvgGPA:F2}");

            var enrolledCS101 = _students.Where(s => s.Courses.Any(c => c.Code == "CS101"));
            Console.WriteLine("Enrolled in CS101:");
            foreach (var s in enrolledCS101)
                Console.WriteLine($"- {s.Name}");

            var sortedByEnrollment = _students.OrderBy(s => s.EnrollmentDate);
            Console.WriteLine("Sorted by Enrollment Date:");
            foreach (var s in sortedByEnrollment)
                Console.WriteLine($"- {s.Name}: {s.EnrollmentDate.ToShortDateString()}");
        }

        public void CustomExtensionMethods()
        {
            Console.WriteLine("=== CUSTOM EXTENSION METHODS ===");

            var filtered = _students.FilterByAgeRange(20, 25);
            Console.WriteLine("Age 20–25:");
            foreach (var s in filtered)
                Console.WriteLine($"- {s.Name}, Age: {s.Age}");

            var avgByMajor = _students.AverageGPAByMajor();
            foreach (var kv in avgByMajor)
                Console.WriteLine($"{kv.Key} => {kv.Value:F2}");

            Console.WriteLine("
Grade Reports: ");
            foreach (var s in _students)
                Console.WriteLine(s.ToGradeReport());

            var stats = _students.CalculateStatistics();
            Console.WriteLine($"Mean GPA: {stats.Mean:F2}, Median GPA: {stats.Median:F2}, StdDev: {stats.StandardDeviation:F2}");
        }

        public void DynamicQueries()
        {
            Console.WriteLine("=== DYNAMIC QUERIES ===");
            var predicate = DynamicFilterBuilder.BuildFilter<Student>("GPA", ">", 3.5);
            var result = _students.AsQueryable().Where(predicate).ToList();
            foreach (var s in result)
                Console.WriteLine($"- {s.Name} ({s.GPA})");
        }

        public void StatisticalAnalysis()
        {
            Console.WriteLine("=== STATISTICAL ANALYSIS ===");
            var stats = _students.CalculateStatistics();
            Console.WriteLine($"Mean: {stats.Mean}, Median: {stats.Median}, StdDev: {stats.StandardDeviation}");
        }

        public void PivotOperations()
        {
            Console.WriteLine("=== PIVOT OPERATIONS ===");
            var pivot = _students
                .GroupBy(s => s.Major)
                .Select(g => new
                {
                    Major = g.Key,
                    GPA_3_0_3_5 = g.Count(s => s.GPA >= 3.0 && s.GPA < 3.5),
                    GPA_3_5_4_0 = g.Count(s => s.GPA >= 3.5 && s.GPA <= 4.0)
                });

            foreach (var row in pivot)
                Console.WriteLine($"{row.Major}: 3.0–3.5 => {row.GPA_3_0_3_5}, 3.5–4.0 => {row.GPA_3_5_4_0}");
        }

        private List<Student> GenerateSampleData()
        {
            return new List<Student>
            {
                new Student
                {
                    Id = 1, Name = "Alice", Age = 20, Major = "CS", GPA = 3.8,
                    EnrollmentDate = new DateTime(2022, 9, 1),
                    Courses = new List<Course> { new Course { Code = "CS101", Grade = 3.7 } }
                },
                new Student
                {
                    Id = 2, Name = "Bob", Age = 22, Major = "Math", GPA = 3.2,
                    EnrollmentDate = new DateTime(2021, 9, 1),
                    Courses = new List<Course> { new Course { Code = "STAT101", Grade = 3.2 } }
                },
                new Student
                {
                    Id = 3, Name = "Carol", Age = 19, Major = "CS", GPA = 3.9,
                    EnrollmentDate = new DateTime(2023, 9, 1),
                    Courses = new List<Course> { new Course { Code = "CS201", Grade = 4.0 } }
                }
            };
        }

        public static void Main(string[] args)
        {
            var processor = new LinqDataProcessor();
            processor.BasicQueries();
            processor.CustomExtensionMethods();
            processor.DynamicQueries();
            processor.StatisticalAnalysis();
            processor.PivotOperations();
        }
    }

    public static class StudentExtensions
    {
        public static IEnumerable<Student> FilterByAgeRange(this IEnumerable<Student> students, int min, int max)
            => students.Where(s => s.Age >= min && s.Age <= max);

        public static Dictionary<string, double> AverageGPAByMajor(this IEnumerable<Student> students)
            => students.GroupBy(s => s.Major).ToDictionary(g => g.Key, g => g.Average(s => s.GPA));

        public static string ToGradeReport(this Student student)
            => $"{student.Name} ({student.Major}): GPA = {student.GPA:F2}";

        public static StudentStatistics CalculateStatistics(this IEnumerable<Student> students)
        {
            var gpas = students.Select(s => s.GPA).OrderBy(x => x).ToList();
            var mean = gpas.Average();
            var median = gpas.Count % 2 == 1 ? gpas[gpas.Count / 2]
                          : (gpas[gpas.Count / 2 - 1] + gpas[gpas.Count / 2]) / 2;
            var stdDev = Math.Sqrt(gpas.Average(x => Math.Pow(x - mean, 2)));

            return new StudentStatistics
            {
                Mean = mean,
                Median = median,
                StandardDeviation = stdDev
            };
        }
    }

    public static class DynamicFilterBuilder
    {
        public static Expression<Func<T, bool>> BuildFilter<T>(string property, string op, object value)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(param, property);
            var constant = Expression.Constant(Convert.ChangeType(value, member.Type));
            Expression body = op switch
            {
                ">" => Expression.GreaterThan(member, constant),
                "<" => Expression.LessThan(member, constant),
                ">=" => Expression.GreaterThanOrEqual(member, constant),
                "<=" => Expression.LessThanOrEqual(member, constant),
                "==" => Expression.Equal(member, constant),
                "!=" => Expression.NotEqual(member, constant),
                _ => throw new NotSupportedException("Operator not supported")
            };
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    public class StudentStatistics
    {
        public double Mean { get; set; }
        public double Median { get; set; }
        public double StandardDeviation { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Major { get; set; }
        public double GPA { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public List<Course> Courses { get; set; } = new();
    }

    public class Course
    {
        public string Code { get; set; }
        public double Grade { get; set; }
    }
}
