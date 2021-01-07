using System;
using System.Collections.Generic;
using System.Text;

namespace SyntaxAnalyzer
{
    class InstructionNode
    {
        public string Type { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
        public string Result { get; set; }
        public static int NextQuad = 1;

        public InstructionNode(string type, string arg1, string arg2, string result)
        {
            Type = type;
            Arg1 = arg1;
            Arg2 = arg2;
            Result = result;
            NextQuad++;
        }

        override
        public string ToString() {
            return Type + " " + Arg1 + " " + Arg2 + " " + Result;
        
        }
    }
}
