using System;
using System.Diagnostics;
using System.Text;

namespace DelegatesLinQ.Homework
{
    public delegate string DataProcessor(string input);
    public delegate void ProcessingEventHandler(string stage, string input, string output);

    public class DataProcessingPipeline
    {
        public event ProcessingEventHandler ProcessingStageCompleted;

        public static string ValidateInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or empty.");
            return input;
        }

        public static string RemoveSpaces(string input)
        {
            string output = input.Replace(" ", "");
            return output;
        }

        public static string ToUpperCase(string input)
        {
            return input.ToUpper();
        }

        public static string AddTimestamp(string input)
        {
            return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {input}";
        }

        public static string ReverseString(string input)
        {
            char[] arr = input.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static string EncodeBase64(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        public string ProcessData(string input, DataProcessor pipeline)
        {
            string currentInput = input;
            string currentOutput = null;

            foreach (Delegate processor in pipeline.GetInvocationList())
            {
                try
                {
                    var stageName = processor.Method.Name;
                    var stopwatch = Stopwatch.StartNew();
                    currentOutput = ((DataProcessor)processor)(currentInput);
                    stopwatch.Stop();

                    // Raise event
                    OnProcessingStageCompleted(stageName, currentInput, currentOutput);
                    currentInput = currentOutput;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EXCEPTION] In stage '{processor.Method.Name}': {ex.Message}");
                    throw; // You can choose to rethrow or continue
                }
            }

            return currentOutput;
        }

        protected virtual void OnProcessingStageCompleted(string stage, string input, string output)
        {
            ProcessingStageCompleted?.Invoke(stage, input, output);
        }
    }

    public class ProcessingLogger
    {
        public void OnProcessingStageCompleted(string stage, string input, string output)
        {
            Console.WriteLine($"[LOG] Stage: {stage} | Input: '{input}' | Output: '{output}'");
        }
    }

    public class PerformanceMonitor
    {
        private readonly Dictionary<string, long> _executionTimes = new();
        private readonly Dictionary<string, int> _callCounts = new();

        public void OnProcessingStageCompleted(string stage, string input, string output)
        {
            var sw = Stopwatch.StartNew();
            sw.Stop();

            if (!_executionTimes.ContainsKey(stage))
            {
                _executionTimes[stage] = 0;
                _callCounts[stage] = 0;
            }

            _executionTimes[stage] += sw.ElapsedMilliseconds;
            _callCounts[stage]++;
        }

        public void DisplayStatistics()
        {
            Console.WriteLine("\n[Performance Statistics]");
            foreach (var stage in _executionTimes.Keys)
            {
                Console.WriteLine($"Stage: {stage} | Calls: {_callCounts[stage]} | Total Time: {_executionTimes[stage]}ms");
            }
        }
    }

    public class DelegateChain
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== HOMEWORK 2: CUSTOM DELEGATE CHAIN ===\n");

            DataProcessingPipeline pipeline = new DataProcessingPipeline();
            ProcessingLogger logger = new ProcessingLogger();
            PerformanceMonitor monitor = new PerformanceMonitor();

            // Subscribe
            pipeline.ProcessingStageCompleted += logger.OnProcessingStageCompleted;
            pipeline.ProcessingStageCompleted += monitor.OnProcessingStageCompleted;

            // Initial pipeline
            DataProcessor chain = DataProcessingPipeline.ValidateInput;
            chain += DataProcessingPipeline.RemoveSpaces;
            chain += DataProcessingPipeline.ToUpperCase;
            chain += DataProcessingPipeline.AddTimestamp;

            string input = "Hello World from C#";
            Console.WriteLine($"Input: {input}");
            string output = pipeline.ProcessData(input, chain);
            Console.WriteLine($"Output: {output}");

            // Extend chain
            chain += DataProcessingPipeline.ReverseString;
            chain += DataProcessingPipeline.EncodeBase64;
            Console.WriteLine("\n-- Extended Processing --");
            output = pipeline.ProcessData("Extended Pipeline Test", chain);
            Console.WriteLine($"Extended Output: {output}");

            // Remove a stage
            chain -= DataProcessingPipeline.ReverseString;
            Console.WriteLine("\n-- Modified Chain (Without Reverse) --");
            output = pipeline.ProcessData("Without Reverse", chain);
            Console.WriteLine($"Modified Output: {output}");

            // Performance
            monitor.DisplayStatistics();

            // Error handling
            Console.WriteLine("\n-- Error Test --");
            try
            {
                pipeline.ProcessData(null, chain);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Handled Error] {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
