using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordAnalyzer
{
    class Word
    {
        public List<String> keyword;
        public List<String> symbol;
        public Word() {
            keyword = new List<string>();
            symbol = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(@"KeywordAndSymbol.txt");
                String nextLine;
                int flag = -1;
                while ((nextLine = sr.ReadLine()) != null)
                {
                    if (nextLine.CompareTo("keyword") == 0)
                    {
                        flag = 0;
                        continue;
                    }
                    if (nextLine.CompareTo("symbol") == 0)
                    {
                        flag = 1;
                        continue;
                    }
                    if (flag == 0)
                    {
                        keyword.Add(nextLine);
                    }
                    else
                    {
                        symbol.Add(nextLine);
                    }
                }
                sr.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Initialization failed\n");
                Console.WriteLine(e);
            }
            
        }


    }
}
