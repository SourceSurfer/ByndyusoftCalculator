using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Calculator
{
    private Dictionary<string, Func<double, double, double>> operations;

    public Calculator()
    {
        operations = new Dictionary<string, Func<double, double, double>>
        {
            { "+", (a, b) => a + b },
            { "-", (a, b) => a - b },
            { "*", (a, b) => a * b },
            { "/", (a, b) => {
                if (b == 0) throw new DivideByZeroException("Cannot divide by zero.");
                return a / b;
            }}
        };
    }

    public void AddOperation(string symbol, Func<double, double, double> operation)
    {
        operations[symbol] = operation;
    }

    public double Evaluate(string expression)
    {
        var postfix = InfixToPostfix(expression);
        var result = EvaluatePostfix(postfix);
        return result;
    }

    private IEnumerable<string> InfixToPostfix(string infix)
    {
        var output = new List<string>();
        var stack = new Stack<string>();
        int i = 0;

        // Process unary operators at the beginning or after '(' or operators
        infix = infix.Replace(" ", "");
        infix = HandleUnaryOperators(infix);

        while (i < infix.Length)
        {
            if (char.IsDigit(infix[i]) || infix[i] == '.')
            {
                string number = "";
                while (i < infix.Length && (char.IsDigit(infix[i]) || infix[i] == '.'))
                {
                    number += infix[i++];
                }
                output.Add(number);
            }
            else if (operations.ContainsKey(infix[i].ToString()) || infix[i] == '(' || infix[i] == ')')
            {
                if (infix[i] == '(')
                {
                    stack.Push(infix[i].ToString());
                }
                else if (infix[i] == ')')
                {
                    while (stack.Peek() != "(")
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Pop(); // Remove '(' from the stack
                }
                else
                {
                    while (stack.Count > 0 && Priority(stack.Peek()) >= Priority(infix[i].ToString()))
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(infix[i].ToString());
                }
                i++;
            }
        }

        while (stack.Count > 0)
        {
            output.Add(stack.Pop());
        }

        return output;
    }

    private string HandleUnaryOperators(string expr)
    {
        if (expr.StartsWith("+") || expr.StartsWith("-"))
        {
            expr = "0" + expr; // Convert unary to binary by adding a "0" at the start
        }
        expr = expr.Replace("(-", "(0-");
        expr = expr.Replace("(+", "(0+");
        return expr;
    }

    private double EvaluatePostfix(IEnumerable<string> postfix)
    {
        var stack = new Stack<double>();
        foreach (var token in postfix)
        {
            if (double.TryParse(token, out double value))
            {
                stack.Push(value);
            }
            else
            {
                double right = stack.Pop();
                double left = stack.Pop();
                stack.Push(operations[token](left, right));
            }
        }
        return stack.Pop();
    }

    private int Priority(string op)
    {
        if (op == "+" || op == "-") return 1;
        if (op == "*" || op == "/") return 2;
        return 0;
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Введите выражение:");
        string input = Console.ReadLine();
        Calculator calc = new Calculator();
        double result = calc.Evaluate(input);
        Console.WriteLine($"Результат: {result}");
    }
}
