using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SyntaxAnalyzer
{
    enum TokenType
    {
        program,
        block,
        type,
        stmts,
        stmt,
        loc,
        decl,
        declArray,
        args,
        init,
        equality,
        rel,
        expr,
        term,
        unary,
        factor,
        terminal
    }
    class Analyzer
    {
        Stack<Node> NodeStack;
        Stack<int> StateStack;
        StreamReader SR;
        String URL;
        int line;
        AllList hlist;
        private bool Initialize(String URL) {
            this.URL = URL;
            line = 0;
            NodeStack = new Stack<Node>();
            StateStack = new Stack<int>();
            StateStack.Push(0);
            hlist = new AllList();
            try
            {
                SR = new StreamReader(URL);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        public Analyzer(string URL)
        {
            this.URL = URL;
        }

        private Node PreWork() {
            int count = 0;
            Node s = new Node(TokenType.program);
            Node type = new Node(TokenType.type);
            type.Next = new List<Node>();
            type.Next.Add(new Node(TokenType.terminal)) ;
            Node name = new Node(TokenType.terminal);
            Node l = new Node(TokenType.terminal, "(",line);
            Node r = new Node(TokenType.terminal, ")",line);
            while (count < 4) {
                String tmp = SR.ReadLine();
                if (tmp == null) {
                    throw new Exception("EEROR at line "+line);
                }
                String[] p = tmp.Split(" ");
                if (p.Length == 1)
                {
                    tmp = SR.ReadLine();
                    continue;
                }
                if (p[1].Equals("int") || p[1].Equals("double"))
                {
                    if (count == 0)
                    {
                        type.Next[0].Name = p[1];
                        type.Line = line;
                        count++;
                    }
                    else
                    {
                        throw new Exception("EEROR at line " + line + " near '" + p[1] + "'");
                    }
                }
                else if (p[0].Equals("I"))
                {
                    if (count == 1)
                    {
                        name.Name = p[1];
                        name.Line = line;
                        count++;
                    }
                    else
                    {
                        throw new Exception("EEROR at line " + line + " near '" + p[1] + "'");
                    }
                }
                else if (p[1].Equals("("))
                {
                    if (count == 2)
                    {
                        l.Name = p[1];
                        l.Line = line;
                        count++;
                    }
                    else
                    {
                        throw new Exception("EEROR at line " + line + " near '" + p[1] + "'");
                    }
                }
                else if (p[1].Equals(")"))
                {
                    if (count == 3)
                    {
                        r.Name = p[1];
                        r.Line = line;
                        count++;
                    }
                    else
                    {
                        throw new Exception("EEROR at line " + line + " near '" + p[1] + "'");
                    }
                } else if (p[0].Equals("L")) {
                    line = int.Parse(p[1]);
                }
                else {
                    throw new Exception("EEROR at line " + line + " near '" + tmp + "'");

                }
            }
            s.Next = new List<Node>();
            s.Next.Add(type);
            s.Next.Add(name);
            s.Next.Add(l);
            s.Next.Add(r);
            return s;
        }

        public Node Analyze(){
            if (!Initialize(URL)) {
                throw new Exception("Initialization failed");
            }
            Node starter = PreWork();
            String tmp= SR.ReadLine();
            String[] s;
            while (tmp != null||NodeStack.Count!=0) {
                int state = StateStack.Peek();
                if (tmp == null) {
                    tmp = "E $";
                }
                s=tmp.Split(" ");
                if (s.Length == 1) {
                    tmp = SR.ReadLine();
                    continue;
                }
                if (s[0].Equals("L"))
                {
                    line = int.Parse(s[1]);
                    tmp = SR.ReadLine();
                }
                else if (s[0].Equals("IL"))
                {
                    throw new Exception("illegal token");
                }
                else {
                    int x;
                    if (s[0].StartsWith("C") || s[0].StartsWith("D"))
                    {
                        x = hlist.GetX("num");
                    }
                    else if (s[0].StartsWith("I"))
                    {
                        x = hlist.GetX("id");
                    }
                    else {
                        x = hlist.GetX(s[1]);
                    }
                    if (x == -1) {
                        throw new Exception("ERROR at line " + line + " near '" + s[1] + "'");
                    }
                    String ans = hlist.Action[x][state];
                    if (ans.StartsWith("a"))
                    {
                        starter.Next.Add(NodeStack.Pop());
                        return starter;
                    }
                    else if (ans.StartsWith("s"))
                    {
                        NodeStack.Push(new Node(TokenType.terminal, s[1],line));
                        StateStack.Push(int.Parse(ans.Remove(0, 1)));
                        tmp = SR.ReadLine();
                    }
                    else if (ans.StartsWith("0"))
                    {
                        throw new Exception("ERROR at line " + line + " near '" + s[1]+"'");
                    }
                    else {
                        int r = hlist.r[int.Parse(ans.Remove(0, 1))];
                        Node t = new Node(hlist.type[int.Parse(ans.Remove(0, 1))]);
                        t.Line = line;
                        t.Next = new List<Node>();
                        for (int j = 0; j < r; j++) {
                            StateStack.Pop();
                            t.Next.Add(NodeStack.Pop());
                        }
                        t.Next.Reverse();
                        NodeStack.Push(t);
                        if (hlist.Goto[hlist.GetNonX(t.Type.ToString())][StateStack.Peek()] == 0) {
                            throw new Exception("ERROR at line "+line+" near '"+s[1]+"'");
                        }
                        StateStack.Push(hlist.Goto[hlist.GetNonX(t.Type.ToString())][StateStack.Peek()]);
                    }
                }
            }
            return null;
        }


       


    }
}
