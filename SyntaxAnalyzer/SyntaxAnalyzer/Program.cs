using System;
using System.Collections.Generic;
using System.IO;

namespace SyntaxAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyzer an = new Analyzer(@"C:\Users\tp103\Desktop\aaa.list");
            Node node = an.Analyze();
            CodeGenerator cg = new CodeGenerator();
            cg.Generate(node);
            cg.InstructionsToFile(@"C:\Users\tp103\Desktop\aaa.code");
            int w = 1;
            foreach (InstructionNode i in cg.instructionNodes)
            {
                Console.WriteLine(w + " " + i.ToString());
                w++;
            }
        }

    }
}
