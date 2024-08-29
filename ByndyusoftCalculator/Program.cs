using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.Linq;

public class Calculator
{
    private class Operation
    {
        public Func<double, double, double> Function { get; }
        public int Priority { get; }

        public Operation(Func<double, double, double> function, int priority)
        {
            Function = function;
            Priority = priority;
        }
    }

    private Dictionary<string, Operation> operations;

    public Calculator()
    {
        operations = new Dictionary<string, Operation>();
        // Predefine some operations with their priorities
        AddOperation("+", (a, b) => a + b, 1);
        AddOperation("-", (a, b) => a - b, 1);
        AddOperation("*", (a, b) => a * b, 2);
        AddOperation("/", (a, b) =>
        {
            if (b == 0) throw new DivideByZeroException("Cannot divide by zero.");
            return a / b;
        }, 2);
    }

    /// <summary>
    /// Добавление операции
    /// </summary>
    /// <param name="symbol">Символ ' '</param>
    /// <param name="operation">Операция (a,b)=>Math.</param>
    /// <param name="priority">Приоритет 1,2,3</param>
    public void AddOperation(string symbol, Func<double, double, double> operation, int priority)
    {
        operations[symbol] = new Operation(operation, priority);
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

        infix = infix.Replace(" ", "");
        if (infix[0] == '-') infix = "0" + infix;

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
            else if (operations.ContainsKey(infix[i].ToString()))
            {
                string currentOp = infix[i].ToString();
                while (stack.Count > 0 && operations.ContainsKey(stack.Peek()) && operations[stack.Peek()].Priority >= operations[currentOp].Priority)
                {
                    output.Add(stack.Pop());
                }
                stack.Push(currentOp);
                i++;
            }
            else if (infix[i] == '(')
            {
                stack.Push("(");
                i++;
            }
            else if (infix[i] == ')')
            {
                while (stack.Peek() != "(")
                {
                    output.Add(stack.Pop());
                }
                stack.Pop();
                i++;
            }
        }

        while (stack.Count > 0)
        {
            output.Add(stack.Pop());
        }

        return output;
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
            else if (operations.ContainsKey(token))
            {
                double right = stack.Pop();
                double left = stack.Pop();
                stack.Push(operations[token].Function(left, right));
            }
        }
        return stack.Pop();
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
        // calc.AddOperation("%",(a,b)=>Math.IEEERemainder(a,b), 2); приоритет 2, результат 5
        // calc.AddOperation("%",(a,b)=>Math.IEEERemainder(a,b), 1); приоритет 1, результат 1
        double result = calc.Evaluate(input);
        Console.WriteLine($"Результат: {result}");
    }
}
