using System;

namespace DelegatesLinQ.Homework
{
    // Event delegates
    public delegate void CalculationEventHandler(string operation, double operand1, double operand2, double result);
    public delegate void ErrorEventHandler(string operation, string errorMessage);

    public class EventCalculator
    {
        public event CalculationEventHandler OperationPerformed;
        public event ErrorEventHandler ErrorOccurred;

        public double Add(double a, double b)
        {
            double result = a + b;
            OnOperationPerformed("Add", a, b, result);
            return result;
        }

        public double Subtract(double a, double b)
        {
            double result = a - b;
            OnOperationPerformed("Subtract", a, b, result);
            return result;
        }

        public double Multiply(double a, double b)
        {
            double result = a * b;
            OnOperationPerformed("Multiply", a, b, result);
            return result;
        }

        public double Divide(double a, double b)
        {
            if (b == 0)
            {
                OnErrorOccurred("Divide", "Cannot divide by zero.");
                return double.NaN;
            }

            double result = a / b;
            OnOperationPerformed("Divide", a, b, result);
            return result;
        }

        // Protected methods to raise events
        protected virtual void OnOperationPerformed(string operation, double operand1, double operand2, double result)
        {
            OperationPerformed?.Invoke(operation, operand1, operand2, result);
        }

        protected virtual void OnErrorOccurred(string operation, string errorMessage)
        {
            ErrorOccurred?.Invoke(operation, errorMessage);
        }
    }

    public class CalculationLogger
    {
        public void OnOperationPerformed(string operation, double operand1, double operand2, double result)
        {
            Console.WriteLine($"[LOG] Operation: {operation} | Operands: {operand1}, {operand2} | Result: {result}");
        }

        public void OnErrorOccurred(string operation, string errorMessage)
        {
            Console.WriteLine($"[LOG][ERROR] Operation: {operation} | Error: {errorMessage}");
        }
    }

    public class CalculationAuditor
    {
        private int _operationCount = 0;

        public void OnOperationPerformed(string operation, double operand1, double operand2, double result)
        {
            _operationCount++;
        }

        public void DisplayStatistics()
        {
            Console.WriteLine($"\n[Auditor] Total operations performed: {_operationCount}");
        }
    }

    public class ErrorHandler
    {
        public void OnErrorOccurred(string operation, string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR HANDLER] {operation} failed: {errorMessage}");
            Console.ResetColor();
        }
    }

    public class HW1_EventCalculator
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== HOMEWORK 1: EVENT CALCULATOR ===\n");

            EventCalculator calculator = new EventCalculator();
            CalculationLogger logger = new CalculationLogger();
            CalculationAuditor auditor = new CalculationAuditor();
            ErrorHandler errorHandler = new ErrorHandler();

            // Subscribe to events
            calculator.OperationPerformed += logger.OnOperationPerformed;
            calculator.OperationPerformed += auditor.OnOperationPerformed;
            calculator.ErrorOccurred += logger.OnErrorOccurred;
            calculator.ErrorOccurred += errorHandler.OnErrorOccurred;

            // Test operations
            calculator.Add(10, 5);
            calculator.Subtract(10, 3);
            calculator.Multiply(4, 7);
            calculator.Divide(15, 3);
            calculator.Divide(10, 0); // Triggers error

            // Display statistics
            auditor.DisplayStatistics();

            Console.ReadKey();
        }
    }
}
