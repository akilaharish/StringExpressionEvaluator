using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringParser
{
    class Program
    {
        private static List<char> AllChars = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
        private static List<string> ValidOperators = new List<string>() { "+", "&&", "AND", "||", "OR", "-" };
        private static List<string> AvailableCharsCombinations = new List<string>();
        private static Random _rng = new Random();
        private static char[] OpenParentheses = { '(', '[', '{' };
        private static char[] CloseParentheses = { ')', ']', '}' };
        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                AvailableCharsCombinations.Add(RandomString(4, AllChars.ToArray()));
            }

            Console.WriteLine("Valid Chars are:");
            AllChars.ForEach(item => Console.WriteLine(item));
            Console.WriteLine("Available Combinations");
            AvailableCharsCombinations.ForEach(item => Console.WriteLine(item));
            Console.WriteLine("Valid Operators are:");
            ValidOperators.ForEach(item => Console.WriteLine(item));
            Console.Write("Enter expression for combination search:");
            string expression = Console.ReadLine();
            if (CheckIfExpressionIsValid(expression))
                Evaluate(expression);
            else
                Console.WriteLine("Invalid Expression");
            Console.Read();
        }

        private static string RandomString(int size, char[] _chars)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        private static bool CheckIfExpressionIsValid(string input)
        {
            return (CheckForValidBrackets(input) && CheckForValidOperators(input) && CheckForValidChars(input) && CheckForValidExpression(input));
        }

        private static bool CheckForValidExpression(string input)
        {
            List<string> bracketsAndOperatorArray = ValidOperators;
            OpenParentheses.ToList().ForEach(item => bracketsAndOperatorArray.Add(item.ToString()));
            CloseParentheses.ToList().ForEach(item => bracketsAndOperatorArray.Add(item.ToString()));

            bool isValid = true;
            for (int i = 0; i < input.Length; i++)
            {
                if (AllChars.IndexOf(input[i]) >= 0)
                {
                    //Its a char, check if  the prev and the next char are brackets or operators
                    if (i > 0)
                    {
                        if (bracketsAndOperatorArray.IndexOf(input[i - 1].ToString()) < 0)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (i < input.Length - 1)
                    {
                        if (bracketsAndOperatorArray.IndexOf(input[i + 1].ToString()) < 0)
                        {
                            isValid = false;
                            break;
                        }
                    }
                }
            }
            return isValid;
        }

        private static bool CheckForValidOperators(string input)
        {
            char[] bracketsAndCharArray = OpenParentheses.Union(CloseParentheses).Union(AllChars.ToArray()).ToArray();
            //Check for operators 
            string[] resultantOperatorArray = input.Split(bracketsAndCharArray, StringSplitOptions.RemoveEmptyEntries);
            return !resultantOperatorArray.Except(ValidOperators).Any();
        }

        private static bool CheckForValidChars(string input)
        {
            List<string> bracketsAndOperatorArray = ValidOperators;
            OpenParentheses.ToList().ForEach(item => bracketsAndOperatorArray.Add(item.ToString()));
            CloseParentheses.ToList().ForEach(item => bracketsAndOperatorArray.Add(item.ToString()));
            //Check for operators 
            string[] resultantCharArray = input.Split(bracketsAndOperatorArray.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            bool isValid = true;
            for (int i = 0; i < resultantCharArray.Length; i++)
            {
                if (AllChars.IndexOf(resultantCharArray[i][0]) < 0)
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }

        private static bool CheckForValidBrackets(string input)
        {
            Stack<int> parentheses = new Stack<int>();

            foreach (char chr in input)
            {
                int index;
                if ((index = Array.IndexOf(OpenParentheses, chr)) != -1)
                {
                    parentheses.Push(index);  // Add index to stach
                }
                else if ((index = Array.IndexOf(CloseParentheses, chr)) != -1)
                {
                    if (parentheses.Count == 0 || parentheses.Pop() != index)
                        return false;
                }
            }
            return parentheses.Count == 0;
        }

        private static void Evaluate(string expression)
        {
            Console.WriteLine("Result:");
            expression = expression.Replace("+", "&&").Replace("AND", "&&").Replace("OR", "||");

            string finalExpression = string.Empty;
            for (int i = 0; i < expression.Length; i++)
            {
                if (AllChars.IndexOf(expression[i]) > -1)
                {
                    bool hasSubOperatorBefore = false;
                    if (i > 0)
                    {
                        if (expression[i - 1] == '-')
                            hasSubOperatorBefore = true;
                    }
                    finalExpression += " ##.IndexOf(\"" + expression[i].ToString() + "\")" + (hasSubOperatorBefore ? "== -1 " : "> -1 ");
                }
                else
                    finalExpression += (expression[i].ToString() == "-" ? "&&" : expression[i].ToString());
            }

            for (int i = 0; i < AvailableCharsCombinations.Count; i++)
            {
                var currentExpression = finalExpression.Replace("##", "\"" + AvailableCharsCombinations[i] + "\"");
                var result = CSharpScript.EvaluateAsync(currentExpression).Result;
                if (bool.Parse(result.ToString()))
                    Console.WriteLine(AvailableCharsCombinations[i]);
            }
        }
    }
}

