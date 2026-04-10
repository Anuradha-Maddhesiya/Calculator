using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Calculator : MonoBehaviour
{
    public TMP_Text display;

    string input = "";
    bool hasResult = false;

    private void Start()
    {
        display.text = "0";
    }

    public void OnNumberPressed(string number)
    {
        if(hasResult)
        {
            input = "";
            hasResult = false;
        }
        if (number == "." && NumberHasDecimal())
            return;
        input += number;
        display.text = input;
    }
    public void OnOperatorPressed(string op)
    {
        hasResult = false;

        // for negative number allow minus otherwise do nothing
        if(input.Length == 0)
        {
            if(op =="-") input = "-";
            return;
        }

        char lastChar = input[input.Length-1];
        if (IsOperator(lastChar))
        {
            input = input.Substring(0, input.Length - 1) + op;
        }
        else
            input += op;
        display.text = input;
    }
    public void OnEqualToPressed()
    {
        if (input.Length == 0) return;

        char lastChar = input[input.Length - 1];
        if (IsOperator(lastChar))
        {
            display.text = "Invalid Input";
            return;
        }
        double result = EvaluateExpression(input);

        // If result is a whole number, don't show decimal
        string resultText = (result%1==0) ? ((long)result).ToString() : result.ToString("G10");

        display.text = resultText;
        input = resultText;
        hasResult = true;
    }
    public void OnAllClearPressed() 
    {
        input = "";
        hasResult = false;
        display.text = "0";
    }

    //DMAS rule
    private double EvaluateExpression(string expr)
    {
        // Split operators and numbers
        List<double> numbers = new List<double>();
        List<char> operators = new List<char>();
        SplitExpression(expr, numbers, operators);

        // Solve / and x first
        ApplyHighPrecedence(numbers, operators);

        // Solve + and -
        double result = ApplyLowPrecedence(numbers, operators);

        return result;
    }
    private void SplitExpression(string expr, List<double> numbers, List<char> operators)
    {
        string currentNumber = "";
        for(int i = 0;i<expr.Length;i++)
        {
            char c = expr[i];

            if(char.IsDigit(c) || c=='.')
            {
                currentNumber += c;
            }
            else 
            {
                // save the completed number
                numbers.Add(double.Parse(currentNumber));
                // save the operator
                operators.Add(c);
                currentNumber = "";
            }
        }

        // Add last number
        numbers.Add(double.Parse(currentNumber));
    }
    private void ApplyHighPrecedence(List<double> numbers, List<char> operators)
    {
       int i = 0;
       while (i < operators.Count) 
       {
            if (operators[i] == 'x'|| operators[i]=='/')
            {
                double left = numbers[i];
                double right = numbers[i+1];
                double result = 0;

                if (operators[i] == 'x')
                    result = left * right;
                else
                    result = Divide(left,right);

                numbers[i] = result;
                numbers.RemoveAt(i+1);
                operators.RemoveAt(i);
            }
            else
                i++;
       }
    }
    private double ApplyLowPrecedence(List<double> numbers, List<char> operators)
    {
       double result = numbers[0];
       for(int i = 0; i < operators.Count ; i++)
       {
            if (operators[i]== '+')
                result = result + numbers[i+1];
            else
                result = result - numbers[i+1];
       }
       return result;
    }

    private double Divide(double a,double b)
    {
        if (b == 0)
        {
            display.text = "Infinity";
            input = "";
            hasResult = false;
            return 0;
        }
        return a / b;
    }


    private bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == 'x' || c == '/';
    }
    private bool NumberHasDecimal()
    {
        for (int i = input.Length - 1; i >= 0; i--)
        {
            if (IsOperator(input[i])) break;
            if (input[i]=='.')return true;
        }
        return false;
    }
}
