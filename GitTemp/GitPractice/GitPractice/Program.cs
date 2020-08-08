using System;

namespace GitPractice {
    class Calculator {
        int a, b;
        int result;

        public void Menu()
        {
            Console.WriteLine("계산기를 시작합니다");
            Console.Write("숫자 2개를 입력하세요.");
        }
        public void Read(int a, int b)
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
            calc.Read(num1, num2);
        }
        public void Calc() {
            string sel;
            calc.Menu();
            InputNum();
            Console.Write("계산 기호를 입력 : ");
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
        public void SeeResult() {
            calc.Result();
        }
    }
    class Program {
        static void Main(string[] args) {
            User user = new User();
            user.Calc();
            user.SeeResult();
        }
    }
}

