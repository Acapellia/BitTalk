using System;

namespace GitPractice {
    class Calculator {
        int a, b;
        int result;
        public void read(int a, int b)
        { this.a = a;
            this.b = b;
        }
        public void result()
        {
            Console.WriteLine(result);
        }
        public int Add() {
            return a + b;
        }
        public int Sub() {
            return a - b;
        }
        public int Mul() {
            return a * b;
        }
        public int Div() {
            if(b != 0) {
                return a / b;
            }
            return 0;
        }
    }
    class User {
        Calculator calc = new Calculator();
        public void InputNum() {
            int num1, num2;
            num1 = Convert.ToInt32(Console.ReadLine());
            num2 = Convert.ToInt32(Console.ReadLine());
            calc.read(num1, num2);
        }
        public void Calc() {
            string sel;
            sel = Console.ReadLine();
            switch(sel){
                case "+":
                    calc.Add();
                    break;
                case "-":
                    calc.Sub();
                    break;
                case "*":
                    calc.Mul();
                    break;
                case "/":
                    calc.Div();
                    break;
            }
        }
    }
    class Program {
        static void Main(string[] args) {
            User user = new User();
            user.InputNum();
            user.Calc();
        }
    }
}

