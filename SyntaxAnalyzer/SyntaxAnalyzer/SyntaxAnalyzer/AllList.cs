using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SyntaxAnalyzer
{
    class AllList
    {
        private List<String> terminal = new List<string>();
        private List<String> NonTerminal = new List<string>();
        public String[][] Action = new String[28][];
        public int[][] Goto = new int[16][];
        public readonly int[] r = { 0, 5, 3, 2, 1, 1, 2, 1, 4, 5, 7, 5, 3, 3, 1, 1, 1, 3, 3, 5, 5, 1, 3, 3, 6, 10, 3, 3,1,1, 4, 1, 3, 3, 1, 3, 3, 1, 3, 3, 1, 3, 3, 1, 2, 1, 3, 1, 1, 1 };
        public readonly TokenType[] type = {
            TokenType.program,
            TokenType.program,
            TokenType.block,
            TokenType.block,
            TokenType.type,
            TokenType.type,
            TokenType.stmts,
            TokenType.stmts,
            TokenType.stmt,
            TokenType.stmt,
            TokenType.stmt,
            TokenType.stmt,
            TokenType.stmt,
            TokenType.stmt,
            TokenType.stmt,
            TokenType.stmt,
            TokenType.stmt,
            TokenType.decl,
            TokenType.args,
            TokenType.args,
            TokenType.args,
            TokenType.args,
            TokenType.args,
            TokenType.args,
            TokenType.declArray,
            TokenType.declArray,
            TokenType.init,
            TokenType.init,
            TokenType.init,
            TokenType.init,
            TokenType.loc,
            TokenType.loc,
            TokenType.equality,
            TokenType.equality,
            TokenType.equality,
            TokenType.rel,
            TokenType.rel,
            TokenType.rel,
            TokenType.expr,
            TokenType.expr,
            TokenType.expr,
            TokenType.term,
            TokenType.term,
            TokenType.term,
            TokenType.unary,
            TokenType.unary,
            TokenType.factor,
            TokenType.factor,
            TokenType.factor,
            TokenType.factor
        };
        public AllList() {
            for (int i=0; i<28;i++) {
                Action[i] = new String[96];
            }
            for (int i = 0; i < 16; i++)
            {
                Goto[i] = new int[96];
            }
            using (StreamReader sr=new StreamReader("ACTION.txt")) {
                for (int i = 0; i < 96; i++)
                {
                    String tmp = sr.ReadLine();
                    MatchCollection matches = Regex.Matches(tmp, "[a-z0-9]+");
                    for (int j = 0; j < 28; j++) {
                        Action[j][i] = matches[j].Value;
                    }
                }
            }
            using (StreamReader sr = new StreamReader("GOTO.txt"))
            {
                for (int i = 0; i < 96; i++)
                {
                    String tmp = sr.ReadLine();
                    MatchCollection matches = Regex.Matches(tmp, "[0-9]+");
                    for (int j = 0; j < 16; j++)
                    {
                        Goto[j][i] = int.Parse(matches[j].Value);
                            
                    }
                }
            }
            terminal.Add("id");
            terminal.Add("(");
            terminal.Add(")");
            terminal.Add("{");
            terminal.Add("}");
            terminal.Add("int");
            terminal.Add("double");
            terminal.Add("=");
            terminal.Add(";");
            terminal.Add("if");
            terminal.Add("else");
            terminal.Add("while");
            terminal.Add("read");
            
            terminal.Add("write");
            terminal.Add(",");
            terminal.Add("num");
            terminal.Add("[");
            terminal.Add("]");
            terminal.Add("==");
            terminal.Add("<>");
            terminal.Add("<");
            terminal.Add(">");
            terminal.Add("+");
            terminal.Add("-");
            terminal.Add("*");
            terminal.Add("/");
            terminal.Add("real");
            terminal.Add("$");
            NonTerminal.Add("program");
            NonTerminal.Add("block");
            NonTerminal.Add("type");
            NonTerminal.Add("stmts");
            NonTerminal.Add("stmt");
            NonTerminal.Add("decl");
            NonTerminal.Add("args");
            NonTerminal.Add("declArray");
            NonTerminal.Add("init");
            NonTerminal.Add("loc");
            NonTerminal.Add("equality");
            NonTerminal.Add("rel");
            NonTerminal.Add("expr");
            NonTerminal.Add("term");
            NonTerminal.Add("unary");
            NonTerminal.Add("factor");

        }
        public int GetNonX(String s) {
            return NonTerminal.LastIndexOf(s);
        }
        public int GetX(String s)
        {
            return terminal.LastIndexOf(s);
        }
    }
}
