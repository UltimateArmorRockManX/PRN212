// A utility to analyze text files and provide statistics
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FileAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("File Analyzer - .NET Core");
            Console.WriteLine("This tool analyzes text files and provides statistics.");
            
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a file path as a command-line argument.");
                Console.WriteLine("Example: dotnet run myfile.txt");
                return;
            }
            
            string filePath = args[0];
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File '{filePath}' does not exist.");
                return;
            }
            
            try
            {
                Console.WriteLine($"Analyzing file: {filePath}");
                
                // Read the file content
                string content = File.ReadAllText(filePath);
                
                // TODO: Implement analysis functionality
                // 1. Count words
                // 2. Count characters (with and without whitespace)
                // 3. Count sentences
                // 4. Identify most common words
                // 5. Average word length
                
                // Example implementation for counting lines:
                int lineCount = File.ReadAllLines(filePath).Length;
                Console.WriteLine($"Number of lines: {lineCount}");

                var words = Regex.Split(content, @"\W+").where(w => !string.IsNullOrEmpty(w)).ToArray();

                int wordCount = words.Length;
                Console.WriteLine($"2. Number of words: {wordCount}");

                int charCountWithSpaces = content.Length;
                int charCountWithoutSpaces = content.Count(c => !char.IsWhiteSpace(c));
                Console.WriteLine($"3.1 Characters (with spaces): {charCountWithSpaces}");
                Console.WriteLine($"3.2 Characters (without spaces): {charCountWithoutSpaces}");

                int sentenceCount = Regex.Matches(content, @"[\.!?]+").Count;
                Console.WriteLine($"4. Number of sentences: {sentenceCount}");

                var topWords = words.GroupBy(w => w.ToLower()).OrderByDescending(g => g.Count()).Take(5).Select(g => new
                    {                         
                        Word = g.Key,
                        Count = g.Count()
                    });

                Console.WriteLine("5. Top 5 most common words:");
                foreach (var item in topWords)
                {
                    Console.WriteLine($"   - \"{item.Word}\": {item.Count} times");
                }

                double averageWordLength = wordCount > 0
                    ? words.Average(w => w.Length)
                    : 0;
                Console.WriteLine($"6. Average word length: {averageWordLength:F2} words");

                // TODO: Additional analysis to be implemented
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file analysis: {ex.Message}");
            }
        }
    }
}