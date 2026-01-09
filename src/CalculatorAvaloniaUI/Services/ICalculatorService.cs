namespace CalculatorAvaloniaUI.Services;

public interface ICalculatorService
{
    double Calculate(double a, double b, string operation);
    string FormatNumber(double number);
}
