using System;


namespace GitPractice {


    class Calculator {
        int a, b;

        public void readUser(int a, int b){
            this.a = a;
            this.b = b;
        }

        public void result(int r)
        {
            Console.WriteLine(r);
        }
        public int Add() {
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

    class Program {
        static void Main(string[] args) {
            User user = new User();
            user.InputNum();
            user.Calc();

        }
    }
}