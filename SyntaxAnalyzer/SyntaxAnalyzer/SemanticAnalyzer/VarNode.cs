using System;
using System.Collections.Generic;
using System.Text;

namespace SyntaxAnalyzer
{
    class VarNode
    {
        public String Type { get;set; }
        public String Name  {get;set;}

        public VarNode(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
