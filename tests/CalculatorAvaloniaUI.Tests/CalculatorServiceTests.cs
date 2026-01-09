using CalculatorAvaloniaUI.Services;

namespace CalculatorAvaloniaUI.Tests;

public class CalculatorServiceTests
{
    private readonly CalculatorService _sut = new();

    [Theory]
    [InlineData(5, 3, "+", 8)]
    [InlineData(10, 4, "-", 6)]
    [InlineData(6, 7, "×", 42)]
    [InlineData(20, 4, "÷", 5)]
    public void Calculate_BasicOperations_ReturnsCorrectResult(double a, double b, string operation, double expected)
    {
        var result = _sut.Calculate(a, b, operation);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Calculate_DivideByZero_ThrowsDivideByZeroException()
    {
        Assert.Throws<DivideByZeroException>(() => _sut.Calculate(10, 0, "÷"));
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(123.456, "123.456")]
    [InlineData(1000000000000000, "1.00E+015")]
    [InlineData(-42.5, "-42.5")]
    public void FormatNumber_VariousInputs_ReturnsExpectedFormat(double input, string expected)
    {
        var result = _sut.FormatNumber(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatNumber_NaN_ReturnsError()
    {
        var result = _sut.FormatNumber(double.NaN);
        Assert.Equal("Error", result);
    }

    [Fact]
    public void FormatNumber_Infinity_ReturnsError()
    {
        var result = _sut.FormatNumber(double.PositiveInfinity);
        Assert.Equal("Error", result);
    }
}
