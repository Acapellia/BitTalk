using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Calc_
{
    class Program
    {
        private int Add(List<int> num)
        {
            int addNum = 0;

            foreach(int fornum in num)
            {
                addNum += fornum;
            }
            return addNum;
        }
        private int subtract(int one, int two)
        {
            int subtractNum = 0;

            subtractNum = one - two;

            return subtractNum;
        }
        static void Main(string[] args)
        {
            Program calc = new Program();
            List<int> num = new List<int>();
            for (int i = 1; i < 6; i++)
            { num.Add(i); Console.WriteLine($"더하는 숫자 : {i}"); }
            Console.WriteLine($"{calc.Add(num)}");
        }
    }
}
