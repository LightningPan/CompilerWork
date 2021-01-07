using System;
using System.Collections.Generic;
using System.IO;
namespace WordAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
             Analyzer a = new Analyzer(@"C:\Users\tp103\Desktop\aaa.txt");
            /* a.ListToFile();
             a.WordSetToFile();*/
            a.ListToFile();
        }
    }
}
