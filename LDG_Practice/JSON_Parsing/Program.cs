using System;
using System.Collections.Generic;

namespace JSON_Parsing {
    class Program {
        static void Main(string[] args) {
            string str = "request:login,id:id123,pw:pw123";
            string[] parse = str.Split(new char[2] { ',', ':' });
            foreach(string s in parse) {
                Console.WriteLine(s);
            }
        }
    }


}
