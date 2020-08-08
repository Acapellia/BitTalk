using System;

namespace GitPractice {
    class Calculator {
        int a, b;
        int result;
        public void read(int a, int b)
        { this.a = a;
            this.b = b;
        }
        public void Result()
        {
            Console.WriteLine(result);
        }
        public void Add() {
            result =  a + b;
        }
        public void Sub() {
            result = a - b;
        }
        public void Mul() {
            result = a * b;
        }
        public void Div() {
            if(b != 0) {
                result = a / b;
            }
            result = 0;
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

