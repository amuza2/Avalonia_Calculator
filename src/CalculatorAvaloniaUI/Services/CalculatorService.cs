using System;
using System.Globalization;

namespace CalculatorAvaloniaUI.Services;

public class CalculatorService : ICalculatorService
{
    public double Calculate(double a, double b, string operation)
    {
        return operation switch
        {
            "+" => a + b,
            "-" => a - b,
            "×" => a * b,
            "÷" => b != 0 ? a / b : throw new DivideByZeroException(),
            _ => b
        };
    }

    public string FormatNumber(double number)
    {
        // Handle special cases
        if (double.IsNaN(number) || double.IsInfinity(number))
            return "Error";

        // Format with appropriate decimal places
        if (Math.Abs(number) < 1e-10)
            return "0";

        // For very large or very small numbers, use scientific notation
        if (Math.Abs(number) >= 1e15 || (Math.Abs(number) < 1e-4 && number != 0))
            return number.ToString("E2", CultureInfo.InvariantCulture);

        // For normal numbers, remove unnecessary decimal places
        string formatted = number.ToString("0.##########", CultureInfo.InvariantCulture);
        
        // Limit display length
        if (formatted.Length > 15)
        {
            formatted = number.ToString("E2", CultureInfo.InvariantCulture);
        }

        return formatted;
    }
}
