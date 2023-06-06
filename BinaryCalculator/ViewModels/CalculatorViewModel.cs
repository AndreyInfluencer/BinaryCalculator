using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BinaryCalculator.Commands;

namespace BinaryCalculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string display;
        private string currentNumber;
        private string previousNumber;
        private string operation;
        private string lastResult;
        private string lastOperation;

        public CalculatorViewModel()
        {
            NumberCommand = new RelayCommand<string>(EnterNumber);
            OperationCommand = new RelayCommand<string>(SetOperation);
            ClearEntryCommand = new RelayCommand(ClearEntry);
            ClearCommand = new RelayCommand(ClearAll);
            EqualsCommand = new RelayCommand(PerformCalculation);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Display
        {
            get { return display; }
            set
            {
                display = value;
                OnPropertyChanged();
            }
        }

        public ICommand NumberCommand { get; }
        public ICommand OperationCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand EqualsCommand { get; }

        private void EnterNumber(string number)
        {
            if (operation == null)
            {
                currentNumber += number;
                Display = currentNumber;
            }
            else
            {
                currentNumber += number;
                Display = currentNumber;
            }
        }

        private void SetOperation(string op)
        {
            operation = op;
            previousNumber = display;
            currentNumber = string.Empty;
        }

        private void ClearEntry()
        {
            currentNumber = string.Empty;
            Display = currentNumber;
        }

        private void ClearAll()
        {
            currentNumber = string.Empty;
            previousNumber = string.Empty;
            operation = null;
            Display = string.Empty;
        }

        private void PerformCalculation()
        {
            if (operation != null && previousNumber != string.Empty && currentNumber != string.Empty)
            {
                lastResult = CalculateResult(operation);
                UpdateDisplay(lastResult);
                previousNumber = currentNumber;
                currentNumber = string.Empty;
                lastOperation = operation;
                operation = null;
            }
            else if (lastOperation != null && previousNumber != string.Empty && Display != string.Empty)
            {
                if (lastResult != null)
                {
                    currentNumber = lastResult;
                    lastResult = CalculateResult(lastOperation, true);
                    if (lastResult != string.Empty)
                    {
                        UpdateDisplay(lastResult);
                    }
                    else
                    {
                        ClearAll();
                    }
                }
            }
        }

        private string CalculateResult(string operation, bool? isRepeatOperation = false)
        {
            if (display != string.Empty)
            {
                currentNumber = display;
            }

            int num1 = Convert.ToInt32(previousNumber, 2);
            int num2 = Convert.ToInt32(currentNumber, 2);
            int result = 0;

            switch (operation)
            {
                case "+":
                    result = num1 + num2;
                    break;
                case "-":
                    if (isRepeatOperation == true && (num1 < num2))
                    {
                        int temp = num1;
                        num1 = num2;
                        num2 = temp;
                    }
                    result = num1 - num2;
                    break;
            }

            if (result < 0)
            {
                MessageBox.Show("Negative results are not supported.", "Error", 
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                ClearAll();
                return string.Empty;
            }

            return Convert.ToString(result, 2);
        }

        private void UpdateDisplay(string value)
        {
            Display = value;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
