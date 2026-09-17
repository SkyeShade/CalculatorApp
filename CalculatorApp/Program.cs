using System;
using System.Drawing;
using System.Windows.Forms;

namespace CalculatorApp;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Set up Windows Forms and start the calculator window.
        ApplicationConfiguration.Initialize();
        Application.Run(new CalculatorForm());
    }
}

public class CalculatorForm : Form
{
    private readonly TextBox display;

    // Stores the current calculation state.
    private double firstNumber;
    private string currentOperator = "";
    private bool waitingForSecondNumber;

    public CalculatorForm()
    {
        // Configure the main window.
        Text = "Calculator";
        Size = new Size(320, 430);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        KeyPreview = true;

        // Create the calculator display.
        display = new TextBox
        {
            Text = "0",
            ReadOnly = true,
            TextAlign = HorizontalAlignment.Right,
            Font = new Font("Segoe UI", 24),
            Location = new Point(15, 15),
            Size = new Size(275, 55)
        };

        Controls.Add(display);

        // Buttons are created from this list in display order.
        string[] buttons =
        {
            "7", "8", "9", "/",
            "4", "5", "6", "*",
            "1", "2", "3", "-",
            "0", ".", "=", "+",
            "C"
        };

        int x = 15;
        int y = 90;

        // Create and position each calculator button.
        foreach (string text in buttons)
        {
            var button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 16),
                Size = new Size(text == "C" ? 275 : 65, 55),
                Location = new Point(x, y)
            };

            button.Click += ButtonClicked;
            Controls.Add(button);

            if (text == "C")
                break;

            x += 70;

            // Move to the next row after four buttons.
            if (x > 225)
            {
                x = 15;
                y += 60;
            }
        }

        // Enable keyboard input for the calculator.
        KeyDown += CalculatorForm_KeyDown;
    }

    private void ButtonClicked(object? sender, EventArgs e)
    {
        // Send the clicked button text to the shared input handler.
        if (sender is not Button button)
            return;

        HandleInput(button.Text);
    }

    private void HandleInput(string input)
    {
        // Handle number input.
        if (char.IsDigit(input[0]))
        {
            if (display.Text == "0" || waitingForSecondNumber)
            {
                display.Text = input;
                waitingForSecondNumber = false;
            }
            else
            {
                display.Text += input;
            }

            return;
        }

        // Add a decimal point if one is not already present.
        if (input == ".")
        {
            if (!display.Text.Contains("."))
                display.Text += ".";

            return;
        }

        // Clear the current calculation.
        if (input == "C")
        {
            Clear();
            return;
        }

        // Calculate the result.
        if (input == "=")
        {
            Calculate();
            return;
        }

        // Store the selected mathematical operator.
        if ("+-*/".Contains(input))
        {
            firstNumber = double.Parse(display.Text);
            currentOperator = input;
            waitingForSecondNumber = true;
        }
    }

    private void Calculate()
    {
        if (string.IsNullOrEmpty(currentOperator))
            return;

        double secondNumber = double.Parse(display.Text);

        // Perform the selected calculation.
        double result = currentOperator switch
        {
            "+" => firstNumber + secondNumber,
            "-" => firstNumber - secondNumber,
            "*" => firstNumber * secondNumber,
            "/" when secondNumber != 0 => firstNumber / secondNumber,
            "/" => double.NaN,
            _ => 0
        };

        // Show an error when attempting to divide by zero.
        display.Text = double.IsNaN(result)
            ? "Error"
            : result.ToString();

        currentOperator = "";
        waitingForSecondNumber = true;
    }

    private void Clear()
    {
        // Reset the calculator to its default state.
        display.Text = "0";
        firstNumber = 0;
        currentOperator = "";
        waitingForSecondNumber = false;
    }

    private void CalculatorForm_KeyDown(object? sender, KeyEventArgs e)
    {
        // Handle number keys on the main keyboard.
        if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
        {
            HandleInput(((int)e.KeyCode - (int)Keys.D0).ToString());
        }
        // Handle number keys on the numeric keypad.
        else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
        {
            HandleInput(((int)e.KeyCode - (int)Keys.NumPad0).ToString());
        }
        else
        {
            // Handle operator and control keys.
            switch (e.KeyCode)
            {
                case Keys.Add:
                    HandleInput("+");
                    break;

                case Keys.Subtract:
                    HandleInput("-");
                    break;

                case Keys.Multiply:
                    HandleInput("*");
                    break;

                case Keys.Divide:
                    HandleInput("/");
                    break;

                case Keys.Enter:
                    HandleInput("=");
                    break;

                case Keys.Decimal:
                    HandleInput(".");
                    break;

                case Keys.Escape:
                case Keys.Delete:
                    HandleInput("C");
                    break;
            }
        }
    }
}