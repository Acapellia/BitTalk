using System;

namespace GitPractice
{
    //test_branch511
    class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
        public int Sub(int a, int b)
        {
            return a - b;
        }
        public int Mul(int a, int b)
        {
            return a * b;
        }
        public int Div(int a, int b)
        {
            if (b != 0)
            {
                return a / b;
            }
            return 0;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Calculator c = new Calculator();
            Console.WriteLine(c.Add(6, 3));
            Console.WriteLine(c.Sub(6, 3));
            Console.WriteLine(c.Mul(6, 3));
            Console.WriteLine(c.Div(6, 3));
        }
    }
}