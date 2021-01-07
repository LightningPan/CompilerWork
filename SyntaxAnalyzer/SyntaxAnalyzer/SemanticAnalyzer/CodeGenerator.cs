using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SyntaxAnalyzer
{
    class CodeGenerator
    {
        public List<VarNode> varNodes = new List<VarNode>();
        public List<InstructionNode> instructionNodes = new List<InstructionNode>();

        public void InstructionsToFile(String URL) {
            try
            {
                int i = 1;
                StreamWriter sw = new StreamWriter(URL) ;
                foreach (InstructionNode s in instructionNodes)
                {
                    sw.WriteLine(i+" "+s.ToString());
                    i++;
                }
                sw.Flush();
                sw.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("write failed");
                return;
            }
            return;

        }

        public void Generate(Node node) {
            ProcessBlock(node.Next[4]);
        }


        void BackPatch(List<int> list, int m) {
            foreach (int s in list) {
                instructionNodes[s - 1].Result = m.ToString();
            }
        }

        List<int> merge(List<int> a, List<int> b) {
            List<int> c = new List<int>();
            foreach (int d in a) {
                c.Add(d);
            }
            foreach (int d in b)
            {
                c.Add(d);
            }
            return c;
        }

        bool checkVarNodes(String name) {
            foreach (VarNode vn in varNodes) {
                if (vn.Name.Equals(name)) {
                    return true;
                }
            }
            return false;
        }
        void checkType(String type, String name,int line) {
            foreach (VarNode vn in varNodes) {
                if (vn.Name.Equals(name)) {
                    if (vn.Type.Equals(type)) {
                        return;
                    }
                    throw new Exception("ERROR AT LINE "+line+" : "+"type is not compatible");
                }
            }
            throw new Exception("ERROR AT LINE " + line + " : " + name + " is not defined");
        }
        //用于临时变量管理
        int tmpVar = 1;

        //处理变量声明
        void ProcessDecl(Node node) {
            String t = node.Next[0].Next[0].Next[0].Name;
            ProcessArgs(t, node.Next[0].Next[1]);
        }
        //处理变量声明时初始化
        void ProcessArgs(String type, Node node) {
            if (checkVarNodes(node.Next[0].Name))
            {
                throw new Exception("Error at Line "+node.Next[0].Line+": "+node.Next[0].Name + " has been defined before");
            }
            if (node.Next.Count == 1)
            {
                instructionNodes.Add(new InstructionNode(type, "-", "-", node.Next[0].Name));
                varNodes.Add(new VarNode(type, node.Next[0].Name));
            }
            else if (node.Next.Count == 3)
            {
                if (node.Next[2].Type == TokenType.args)
                {
                    instructionNodes.Add(new InstructionNode(type, "-", "-", node.Next[0].Name));
                    varNodes.Add(new VarNode(type, node.Next[0].Name));
                    ProcessArgs(type, node.Next[2]);
                }
                else
                {
                    try
                    {
                        int t = int.Parse(node.Next[2].Name);
                        instructionNodes.Add(new InstructionNode(type, node.Next[2].Name, "-", node.Next[0].Name));
                        varNodes.Add(new VarNode(type, node.Next[0].Name));
                    }
                    catch (Exception)
                    {
                        checkType(type, node.Next[2].Name,node.Next[2].Line);
                        instructionNodes.Add(new InstructionNode(type, node.Next[2].Name, "-", node.Next[0].Name));
                        varNodes.Add(new VarNode(type, node.Next[0].Name));
                    }
                }
            }
            else if (node.Next.Count == 5)
            {
                try
                {
                    int t = int.Parse(node.Next[2].Name);
                    instructionNodes.Add(new InstructionNode(type, node.Next[2].Name, "-", node.Next[0].Name));
                    varNodes.Add(new VarNode(type, node.Next[0].Name));
                }
                catch (Exception)
                {
                    checkType(type, node.Next[2].Name,node.Next[2].Line);
                    instructionNodes.Add(new InstructionNode(type, node.Next[2].Name, "-", node.Next[0].Name));
                    varNodes.Add(new VarNode(type, node.Next[0].Name));
                }
                finally
                {
                    ProcessArgs(type, node.Next[4]);
                }
            }
            else {
                throw new Exception("Error at Line "+node.Line+" : undefined error on processing declaration");
            }
        }

        //处理数组声明
        void ProcessDeclArray(Node node) {
            string type = node.Next[0].Next[0].Next[0].Name;
            if (checkVarNodes(node.Next[0].Next[1].Name))
            {
                throw new Exception("Error at Line"+node.Line+" : "+node.Next[0].Next[1].Name + " has been defined");
            }
            if (node.Next[0].Next.Count == 6)
            {
                instructionNodes.Add(new InstructionNode(type + "[]", "-", "-", node.Next[0].Next[1].Name));
                varNodes.Add(new VarNode(type + "[]", node.Next[0].Next[1].Name));
            }
            else if (node.Next[0].Next.Count == 10)
            {
                instructionNodes.Add(new InstructionNode(type + "[]", "-", "-", node.Next[0].Next[1].Name));
                varNodes.Add(new VarNode(type + "[]", node.Next[0].Next[1].Name));
                ProcessInit(type, 0, node.Next[0].Next[1].Name, node.Next[0].Next[7]);
            }
            else {
                throw new Exception("Error at Line" + node.Line + " : "+"undefined error on processing declaration");
            }
        }
        //处理数组声明时初始化
        void ProcessInit(String type, int n, string id, Node node) {
            int num;
            if (type.Equals("int"))
            {
                num = 4;
                try
                {
                    int t = int.Parse(node.Next[0].Name);
                    instructionNodes.Add(new InstructionNode("*", num.ToString(), n.ToString(), "T1"));
                    instructionNodes.Add(new InstructionNode("+", id, "T1", "T2"));
                    instructionNodes.Add(new InstructionNode("SVST", t.ToString(), "-", "T2"));
                }
                catch (Exception)
                {
                    checkType(type, node.Next[0].Name,node.Next[0].Line);
                    instructionNodes.Add(new InstructionNode("*", num.ToString(), n.ToString(), "T1"));
                    instructionNodes.Add(new InstructionNode("+", id, "T1", "T2"));
                    instructionNodes.Add(new InstructionNode("SVST", node.Next[0].Name, "-", "T2"));
                }
                finally
                {
                    if (node.Next.Count == 3)
                    {
                        ProcessInit(type, n + 1, id, node.Next[2]);
                    }

                }
            }
            else
            {
                num = 8;
                try
                {
                    double t = double.Parse(node.Next[0].Name);
                    instructionNodes.Add(new InstructionNode("*", num.ToString(), n.ToString(), "T1"));
                    instructionNodes.Add(new InstructionNode("+", id, "T1", "T2"));
                    instructionNodes.Add(new InstructionNode("SVST", t.ToString(), "-", "T2"));
                }
                catch (Exception)
                {
                    checkType(type, node.Next[0].Name,node.Next[0].Line);
                    instructionNodes.Add(new InstructionNode("*", num.ToString(), n.ToString(), "T1"));
                    instructionNodes.Add(new InstructionNode("+", id, "T1", "T2"));
                    instructionNodes.Add(new InstructionNode("SVST", node.Next[0].Name, "-", "T2"));
                }
                finally
                {
                    if (node.Next.Count == 3)
                    {
                        ProcessInit(type, n + 1, id, node.Next[2]);
                    }
                }
            }
        }

        //处理读
        void ProcessRead(Node node) {
            if (checkVarNodes(node.Next[1].Name)) {
                instructionNodes.Add(new InstructionNode("read", "-", "-", node.Next[1].Name));
            }
        }

        //处理写
        void ProcessWrite(Node node) {
            ProcessNormalEquality(node.Next[1], 1);
            instructionNodes.Add(new InstructionNode("write", nums.Pop(), "-", "-"));
        }

        void ProscessAssignment(Node node) {
            ProcessNormalEquality(node.Next[2], 1);
            String right = nums.Pop();
            ProcessNormalEquality(node.Next[0], tmpVar + 1);
            String left = nums.Pop();
            instructionNodes.Add(new InstructionNode("=", right, "-", left));
        }

        //处理普通equality
        Stack<String> nums = new Stack<string>();
        void ProcessNormalEquality(Node node, int tmpCount) {
            tmpVar = tmpCount;
            getPostExpr(node);
            nums.Clear();
            foreach (String s in res) {
                if (nums.Count >= 3) {
                    tmpVar++;
                }
                if (s.Equals("OP_NEG"))
                {
                    instructionNodes.Add(new InstructionNode("-!", nums.Pop(), "-", "T" + tmpVar));
                    nums.Push("T" + tmpVar);
                }
                else if (s.Equals("+"))
                {
                    string tmp1 = nums.Pop();
                    string tmp2 = nums.Pop();
                    instructionNodes.Add(new InstructionNode("+", tmp2, tmp1, "T" + tmpVar));
                    nums.Push("T" + tmpVar);
                }
                else if (s.Equals("-"))
                {
                    string tmp1 = nums.Pop();
                    string tmp2 = nums.Pop();
                    instructionNodes.Add(new InstructionNode("-", tmp2, tmp1, "T" + tmpVar));
                    nums.Push("T" + tmpVar);
                }
                else if (s.Equals("*"))
                {
                    string tmp1 = nums.Pop();
                    string tmp2 = nums.Pop();
                    instructionNodes.Add(new InstructionNode("*", tmp2, tmp1, "T" + tmpVar));
                    nums.Push("T" + tmpVar);
                }
                else if (s.Equals("/"))
                {
                    string tmp1 = nums.Pop();
                    string tmp2 = nums.Pop();
                    instructionNodes.Add(new InstructionNode("/",tmp2, tmp1, "T" + tmpVar));
                    nums.Push("T" + tmpVar);
                }
                else if (s.Equals("locArray"))
                {
                    instructionNodes.Add(new InstructionNode("*", nums.Pop(), "4", "T" + tmpVar));
                    nums.Push("T" + tmpVar);
                    string tmp1 = nums.Pop();
                    string tmp2 = nums.Pop();
                    checkType("int[]", tmp2,node.Line);
                    instructionNodes.Add(new InstructionNode("=[]", tmp2, tmp1, "T" + tmpVar));
                    nums.Push("T" + tmpVar);
                }
                else {
                    try
                    {
                        if (nums.Count >= 3)
                        {
                            tmpVar--;
                        }
                        int t = int.Parse(s);
                        nums.Push(s);
                    }
                    catch (Exception) {
                        if (checkVarNodes(s))
                        {
                            nums.Push(s);
                        }
                        else {
                            throw new Exception("Error at Line "+node.Line+" : "+s + " has not been defined");
                        }
                    }
                }
            }
        }

        //获得后缀表达式使用的临时变量
        List<string> res = new List<string>();
        Stack<string> tmp = new Stack<string>();
        bool flag;
        //得到后缀表达式
        void getPostExpr(Node s)
        {
            res.Clear();
            tmp.Clear();
            flag = true;
            GeneratePostExpr(s);
            while (tmp.Count != 0)
            {
                res.Add(tmp.Pop());
            }
        }
        //生成后缀表达式
        void GeneratePostExpr(Node s)
        {
            if (s.Next == null)
            {
                if (s.Type == TokenType.terminal)
                {
                    if (s.Name.Equals("("))
                    {
                        tmp.Push("(");
                    }
                    else if (s.Name.Equals(")"))
                    {
                        while (tmp.Count != 0 && !tmp.Peek().Equals("("))
                        {
                            res.Add(tmp.Pop());
                        }
                        tmp.Pop();
                    }
                    else if (s.Name.Equals("+"))
                    {
                        while (tmp.Count != 0)
                        {
                            if (tmp.Peek().Equals("(") || tmp.Peek().Equals("["))
                            {
                                break;
                            }
                            res.Add(tmp.Pop());
                        }
                        tmp.Push("+");
                    }
                    else if (s.Name.Equals("-"))
                    {
                        while (tmp.Count != 0)
                        {
                            if (tmp.Peek().Equals("(") || tmp.Peek().Equals("["))
                            {
                                break;
                            }
                            res.Add(tmp.Pop());
                        }
                        if (flag)
                        {
                            tmp.Push("-");
                        }
                        else
                        {
                            tmp.Push("OP_NEG");
                            flag = true;
                        }

                    }
                    else if (s.Name.Equals("*"))
                    {
                        while (tmp.Count != 0)
                        {
                            if (tmp.Peek().Equals("+") || tmp.Peek().Equals("-") || tmp.Peek().Equals("(") || tmp.Peek().Equals("["))
                            {
                                break;
                            }
                            else
                            {
                                res.Add(tmp.Pop());
                            }
                        }
                        tmp.Push("*");
                    }
                    else if (s.Name.Equals("/"))
                    {
                        while (tmp.Count != 0)
                        {
                            if (tmp.Peek().Equals("+") || tmp.Peek().Equals("-") || tmp.Peek().Equals("(") || tmp.Peek().Equals("["))
                            {
                                break;
                            }
                            else
                            {
                                res.Add(tmp.Pop());
                            }
                        }
                        tmp.Push("/");
                    }
                    else if (s.Name.Equals("["))
                    {
                        tmp.Push("[");
                    }
                    else if (s.Name.Equals("]"))
                    {
                        while (tmp.Count != 0 && !tmp.Peek().Equals("["))
                        {
                            res.Add(tmp.Pop());
                        }
                        res.Add("locArray");
                        tmp.Pop();
                    }
                    else
                    {
                        res.Add(s.Name);
                    }
                }
                return;
            }
            else
            {
                if (s.Type == TokenType.unary)
                {
                    if (s.Next.Count == 2)
                    {
                        flag = false;
                    }

                }

                foreach (Node q in s.Next)
                {
                    GeneratePostExpr(q);
                }

            }
        }

        //处理布尔equality
        class E{
             public List<int> equalityTrue = new List<int>();
             public List<int> equalityFalse = new List<int>(); 
         }
        E ProcessBoolEquality(Node node) {
            E e = new E();
            if (node.Next.Count == 3)
            {
                ProcessNormalEquality(node.Next[0],1);
                String left = nums.Pop();
                ProcessNormalEquality(node.Next[2],tmpVar+1);
                String right = nums.Pop();
                e.equalityTrue.Add(InstructionNode.NextQuad);
                e.equalityFalse.Add(InstructionNode.NextQuad + 1);
                if (node.Next[1].Name.Equals("==")) {
                    instructionNodes.Add(new InstructionNode("JmpE", left, right, "-"));
                }
                else
                {
                    instructionNodes.Add(new InstructionNode("JmpNE", left, right, "-"));
                }
                instructionNodes.Add(new InstructionNode("Jmp", "-", "-", "-"));
            }
            else {
                ProcessNormalEquality(node.Next[0].Next[0],1);
                String left = nums.Pop();
                ProcessNormalEquality(node.Next[0].Next[2], tmpVar + 1);
                String right = nums.Pop();
                e.equalityTrue.Add(InstructionNode.NextQuad);
                e.equalityFalse.Add(InstructionNode.NextQuad + 1);
                if (node.Next[0].Next[1].Name.Equals("<"))
                {
                    instructionNodes.Add(new InstructionNode("JmpL", left, right, "-"));
                }
                else {
                    instructionNodes.Add(new InstructionNode("JmpG", left, right, "-"));
                }
                instructionNodes.Add(new InstructionNode("Jmp", "-", "-", "-"));
            }
            return e;
        
        }

        //相当于一个controller
        List<int> ProcessStmt(Node node) {
            if (node.Next[0].Type == TokenType.decl)
            {
                ProcessDecl(node);
                return new List<int>();
            }
            else if (node.Next[0].Type == TokenType.declArray)
            {
                ProcessDeclArray(node);
                return new List<int>();
            }
            else if (node.Next[0].Type == TokenType.loc)
            {
                ProscessAssignment(node);
                return new List<int>();
            }
            else if (node.Next[0].Type == TokenType.block) {
                return ProcessBlock(node.Next[0]);
            }
            else if (node.Next[0].Name.Equals("if"))
            {
                if (node.Next.Count == 7)
                {
                    return ProcessIfElse(node);
                }
                else
                {
                    return ProcessIf(node);
                }
            }
            else if (node.Next[0].Name.Equals("while"))
            {
                return ProcessWhile(node);
            }
            else if (node.Next[0].Name.Equals("read"))
            {
                ProcessRead(node);
                return new List<int>();
            }
            else if (node.Next[0].Name.Equals("write"))
            {
                ProcessWrite(node);
                return new List<int>();
            }
            else
            {
                throw new Exception("Error at Line "+node.Line+" : "+"Undefined Error");
            }
        }

        //处理if语句
        List<int> ProcessIf(Node node) {
            E e=ProcessBoolEquality(node.Next[2]);
            BackPatch(e.equalityTrue,InstructionNode.NextQuad);
            List<int> S = merge(e.equalityFalse,ProcessStmt(node.Next[4]));
            return S;
        }
        //处理if——else
        List<int> ProcessIfElse(Node node) {
            E e = ProcessBoolEquality(node.Next[2]);
            int m1 = InstructionNode.NextQuad;
            List<int> s1 = ProcessStmt(node.Next[4]);
            int n = InstructionNode.NextQuad;
            instructionNodes.Add(new InstructionNode("Jmp","-","-","-"));
            int m2 = InstructionNode.NextQuad;
            List<int> s2 = ProcessStmt(node.Next[6]);
            BackPatch(e.equalityTrue,m1);
            BackPatch(e.equalityFalse,m2);
            List<int> S = merge(s1,s2);
            S.Add(n);
            return S;
        }

        //处理while
        List<int> ProcessWhile(Node node) {
            int m1 = InstructionNode.NextQuad;
            E e = ProcessBoolEquality(node.Next[2]);
            int m2 = InstructionNode.NextQuad;
            BackPatch(ProcessStmt(node.Next[4]),m1);
            BackPatch(e.equalityTrue,m2);
            List<int> S = e.equalityFalse;
            instructionNodes.Add(new InstructionNode("Jmp","-","-",m1.ToString()));
            return S;
        }
        //处理stmts
        List<int> ProcessStmts(Node node)
        {
            if (node.Next[0].Type == TokenType.stmt)
            {
                return ProcessStmt(node.Next[0]);
            }
            else {
                List<int> L=ProcessStmts(node.Next[0]);
                int m = InstructionNode.NextQuad;
                List<int> S = ProcessStmt(node.Next[1]);
                BackPatch(L,m);
                return S;
            }

        }
        //处理Blcok
        List<int> ProcessBlock(Node node) {
            if (node.Next.Count == 2)
            {
                return new List<int>();
            }
            else {
                List<int> S=ProcessStmts(node.Next[1]);
                BackPatch(S,InstructionNode.NextQuad);
                return S;
            }
        
        }

    }
}
