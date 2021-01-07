using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordAnalyzer
{
    class Analyzer
    {
        String URL;
        Word word;
        public HashSet<String> wordSet;
        public List<String> list;
        List<int> sideList;
        List<int> num;
        List<int> letters;
        List<int> followNum;
        StreamReader sr;
        int current, next;
        List<char> buffer;
        int count=1;  //记录行数


        public bool ListToFile() {
            try
            {
                StreamWriter sw = new StreamWriter(Path.ChangeExtension(URL, "list"));
                foreach (String s in list)
                {
                    sw.WriteLine(s);
                }
                sw.Flush();
                sw.Dispose();
            }
            catch(Exception e){
                Console.WriteLine("write failed");
                return false;
            }
            return true;
        }

        public bool WordSetToFile()
        {
            try
            {
                StreamWriter sw = new StreamWriter(Path.ChangeExtension(URL, "set"));
                foreach (String s in wordSet)
                {
                    sw.WriteLine(s);
                }
                sw.Flush();
                sw.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("write failed");
                return false;
            }
            return true;
        }

        private void initializeList(String URL) {
            this.URL = URL;
            word = new Word();
            buffer = new List<char>();
            followNum = new List<int>();
            followNum.Add(41);
            followNum.Add(32);
            followNum.Add(93);
            followNum.Add(61);
            followNum.Add(47);
            followNum.Add(43);
            followNum.Add(45);
            followNum.Add(42);
            followNum.Add(59);
            followNum.Add(60);
            followNum.Add(62);
            followNum.Add(9);
            followNum.Add(10);
            followNum.Add(13);
            followNum.Add(-1);
            followNum.Add(44);
            sideList = new List<int>();
            sideList.Add(40); // (
            sideList.Add(41); //  )
            sideList.Add(32);  // 空格
            sideList.Add(123);  //  {
            sideList.Add(125);   //  }
            sideList.Add(91);    // [
            sideList.Add(93);    // ]
            sideList.Add(61);     //  =
            sideList.Add(47);     // / 
            sideList.Add(43);    //  +
            sideList.Add(45);    //  -
            sideList.Add(42);    // *
            sideList.Add(59);    //  ;
            sideList.Add(60);     // <
            sideList.Add(62);     // >
            sideList.Add(9);     //制表
            sideList.Add(10);    //换行
            sideList.Add(13);    //回车
            sideList.Add(-1);    //结束
            sideList.Add(44);
            num = new List<int>();
            for (int i = 48; i <= 57; i++)
            {
                num.Add(i);
            }
            letters = new List<int>();
            for (int i = 97; i <= 122; i++)
            {
                letters.Add(i);
            }
            for (int i = 65; i <= 90; i++)
            {
                letters.Add(i);
            }
            wordSet = new HashSet<string>();
            list = new List<string>();
            try
            {
                sr = new StreamReader(URL);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Initialization failed\n");
                Console.WriteLine(e);
            }
            list.Add("L 1");
        }

        private void illegalSymbolHandler() {
            buffer.Add((char)current);
            current = next;
            while ((next = sr.Read()) != -1)
            {
                if (sideList.Contains(next))
                {
                    buffer.Add((char)current);
                    current = next;
                    break;
                }
                else
                {
                    buffer.Add((char)current);
                    current = next;
                }
            }
            Console.WriteLine("illegal symbol '"+new String(buffer.ToArray())+"' at line "+count);
            list.Add("IL "+new String(buffer.ToArray()));
            buffer.Clear();
        }

        public Analyzer(String URL)
        {
            initializeList(URL);
            current = sr.Read();
            while ((next = sr.Read()) != -1 || current!=-1) {
                if (sideList.Contains(current))
                {
                    if (current == 47)    //  /和注释
                    {
                        if (next == 42)
                        {  //处理注解
                            wordSet.Add("S /*");
                            current = next;
                            while ((next = sr.Read()) != -1)
                            {
                                if ((next == 47) && (current == 42))
                                {
                                    wordSet.Add("S */");
                                    break;
                                }
                                current = next;
                            }
                            if (next == -1)
                            {
                               Console.WriteLine("do not find */ to match /*");
                            }
                            current = sr.Read();
                            continue;
                        }
                        else
                        {
                            list.Add("S /");
                            wordSet.Add("S /");
                            current = next;
                            continue;
                        }
                    }
                    else if (current == 61)  //  =和==
                    {
                        if (next == 61)
                        {
                            list.Add("S ==");
                            wordSet.Add("S ==");
                            current = sr.Read();
                            continue;
                        }
                        else
                        {
                            list.Add("S =");
                            wordSet.Add("S =");
                            current = next;
                            continue;
                        }
                    }
                    else if (current == 60)    // <和<>
                    {
                        if (next == 62)
                        {
                            list.Add("S <>");
                            wordSet.Add("S <>");
                            current = sr.Read();
                            continue;
                        }
                        else
                        {
                            list.Add("S <");
                            wordSet.Add("S <");
                            current = next;
                            continue;
                        }
                    }
                    else if (current == 9 || current == 10 || current == 13 || current==32) {
                        if (current == 10) {
                            count++;
                            list.Add("L "+count.ToString());
                        }
                        current = next;
                    }
                    else
                    {
                        list.Add("S "+((char)current).ToString());
                        wordSet.Add("S "+((char)current).ToString());
                        current = next;
                    }
                }
                else if (num.Contains(current))
                {
                    bool flag = false;//标记是否存在小数点
                    buffer.Add((char)current);
                    current = next;
                    if (!(num.Contains(current)||current==46))
                    {
                        if (sideList.Contains(current))
                        {
                            list.Add("C " + new String(buffer.ToArray()));
                            wordSet.Add("C " + new String(buffer.ToArray()));
                            buffer.Clear();
                        }
                        else {
                            next = sr.Read();
                            illegalSymbolHandler();
                        }
                    }
                    else
                    {
                        if (current == 46) {
                            flag = true;
                        }
                        while ((next = sr.Read()) != -1)
                        {
                            if (num.Contains(next))
                            {
                                buffer.Add((char)current);
                                current = next;
                                continue;
                            }
                            else if (followNum.Contains(next))
                            {
                                buffer.Add((char)current);
                                if (flag)
                                {
                                    list.Add("D " + new String(buffer.ToArray()));
                                    wordSet.Add("D " + new String(buffer.ToArray()));
                                }
                                else
                                {
                                    list.Add("C " + new String(buffer.ToArray()));
                                    wordSet.Add("C " + new String(buffer.ToArray()));
                                }
                                buffer.Clear();
                                current = next;
                                break;
                            }
                            else if (next == 46) {
                                if (flag)
                                {
                                    illegalSymbolHandler();
                                    break;
                                }
                                else {
                                    flag = true;
                                    buffer.Add((char)current);
                                    current = next;
                                    continue;
                                }
                            }
                            else
                            {
                                illegalSymbolHandler();
                                break;
                            }

                        }
                    }

                }
                else if (letters.Contains(current))
                {
                    buffer.Add((char)current);
                    current = next;
                    if (sideList.Contains(current))
                    {
                        list.Add("I "+new String(buffer.ToArray()));
                        wordSet.Add("I "+new String(buffer.ToArray()));
                        buffer.Clear();
                    }
                    else if (letters.Contains(next) || num.Contains(next) || next == 95)
                    {
                        while ((next = sr.Read()) != -1)
                        {
                            if (sideList.Contains(next))
                            {
                                buffer.Add((char)current);
                                if (word.keyword.Contains(new String(buffer.ToArray())))
                                {
                                    list.Add("K " + new String(buffer.ToArray()));
                                    wordSet.Add("K " + new String(buffer.ToArray()));
                                }
                                else if (current == 95)
                                {
                                    Console.WriteLine("illegal symbol '" + new String(buffer.ToArray()) + "' at line " + count);
                                    list.Add("IL " + new String(buffer.ToArray()));
                                    wordSet.Add("IL " + new String(buffer.ToArray()));
                                }
                                else
                                {
                                    list.Add("I "+new String(buffer.ToArray()));
                                    wordSet.Add("I "+new String(buffer.ToArray()));
                                }
                                buffer.Clear();
                                current = next;
                                break;
                            }
                            else if (letters.Contains(next) || num.Contains(next) || next == 95)
                            {
                                buffer.Add((char)current);
                                current = next;
                            }
                            else
                            {
                                illegalSymbolHandler();
                                break;
                            }
                        }
                    }
                    else
                    {
                        next = sr.Read();
                        illegalSymbolHandler();
                    }
                }
                else {
                    illegalSymbolHandler();
                }
            }
            sr.Close();
        }
    }
}