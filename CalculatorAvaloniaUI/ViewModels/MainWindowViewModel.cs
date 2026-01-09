using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CalculatorAvaloniaUI.Services;

namespace CalculatorAvaloniaUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ICalculatorService _calculatorService;

    public MainWindowViewModel() : this(new CalculatorService())
    {
    }

    public MainWindowViewModel(ICalculatorService calculatorService)
    {
        _calculatorService = calculatorService;
    }

    [ObservableProperty]
    private string _displayText = "0";
    
    [ObservableProperty]
    private string _previousOperation = "";
    
    [ObservableProperty]
    private string _memoryIndicator = "";
    
    [ObservableProperty]
    private bool _hasMemoryValue = false;
    
    private double _currentValue = 0;
    private double _previousValue = 0;
    private double _memoryValue = 0;
    private string _currentOperation = "";
    private bool _isNewEntry = true;
    private bool _hasDecimalPoint = false;
    private bool _lastActionWasEquals = false;

    [RelayCommand]
    private void Number(string number)
    {
        if (_lastActionWasEquals)
        {
            Clear();
            _lastActionWasEquals = false;
        }

        if (_isNewEntry || DisplayText == "0")
        {
            DisplayText = number;
            _isNewEntry = false;
            _hasDecimalPoint = false;
        }
        else
        {
            if (DisplayText.Length < 15) // Prevent overflow
            {
                DisplayText += number;
            }
        }
    }

    [RelayCommand]
    private void Decimal()
    {
        if (_lastActionWasEquals)
        {
            Clear();
            _lastActionWasEquals = false;
        }

        if (_isNewEntry)
        {
            DisplayText = "0.";
            _isNewEntry = false;
            _hasDecimalPoint = true;
        }
        else if (!_hasDecimalPoint)
        {
            DisplayText += ".";
            _hasDecimalPoint = true;
        }
    }

    [RelayCommand]
    private void Operation(string operation)
    {
        if (!_isNewEntry && !string.IsNullOrEmpty(_currentOperation) && !_lastActionWasEquals)
        {
            Equals();
        }

        if (double.TryParse(DisplayText, out double value))
        {
            _previousValue = value;
        }

        _currentOperation = operation;
        PreviousOperation = $"{_calculatorService.FormatNumber(_previousValue)} {operation}";
        _isNewEntry = true;
        _lastActionWasEquals = false;
    }

    [RelayCommand]
    private void Equals()
    {
        if (string.IsNullOrEmpty(_currentOperation)) return;

        if (double.TryParse(DisplayText, out double value))
        {
            _currentValue = value;
        }

        try
        {
            var result = _calculatorService.Calculate(_previousValue, _currentValue, _currentOperation);

            DisplayText = _calculatorService.FormatNumber(result);
            PreviousOperation = $"{_calculatorService.FormatNumber(_previousValue)} {_currentOperation} {_calculatorService.FormatNumber(_currentValue)} =";
        }
        catch (DivideByZeroException)
        {
            DisplayText = "Cannot divide by zero";
            PreviousOperation = "";
        }
        catch (OverflowException)
        {
            DisplayText = "Overflow";
            PreviousOperation = "";
        }

        _currentOperation = "";
        _isNewEntry = true;
        _lastActionWasEquals = true;
    }

    [RelayCommand]
    private void Clear()
    {
        DisplayText = "0";
        _currentValue = 0;
        _previousValue = 0;
        _currentOperation = "";
        PreviousOperation = "";
        _isNewEntry = true;
        _hasDecimalPoint = false;
        _lastActionWasEquals = false;
    }

    [RelayCommand]
    private void ClearEntry()
    {
        DisplayText = "0";
        _isNewEntry = true;
        _hasDecimalPoint = false;
    }

    [RelayCommand]
    private void Backspace()
    {
        if (DisplayText.Length > 1 && DisplayText != "0")
        {
            if (DisplayText.EndsWith("."))
            {
                _hasDecimalPoint = false;
            }
            DisplayText = DisplayText[..^1];
        }
        else
        {
            DisplayText = "0";
            _isNewEntry = true;
            _hasDecimalPoint = false;
        }
    }

    // Memory Functions
    [RelayCommand]
    private void MemoryClear()
    {
        _memoryValue = 0;
        HasMemoryValue = false;
        MemoryIndicator = "";
    }

    [RelayCommand]
    private void MemoryRecall()
    {
        DisplayText = _calculatorService.FormatNumber(_memoryValue);
        _isNewEntry = true;
        _hasDecimalPoint = DisplayText.Contains(".");
    }

    [RelayCommand]
    private void MemoryAdd()
    {
        if (double.TryParse(DisplayText, out double value))
        {
            _memoryValue += value;
            UpdateMemoryIndicator();
        }
    }

    [RelayCommand]
    private void MemorySubtract()
    {
        if (double.TryParse(DisplayText, out double value))
        {
            _memoryValue -= value;
            UpdateMemoryIndicator();
        }
    }

    private void UpdateMemoryIndicator()
    {
        HasMemoryValue = _memoryValue != 0;
        MemoryIndicator = HasMemoryValue ? "M" : "";
    }
}