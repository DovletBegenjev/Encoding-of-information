using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeorInf4_Haff
{
    class Program
    {
        class Node
        {
            public int nodesWeight = 0;
            public char symbol = ' ';
            public Node left, right;

            public Node()
            {
                left = right = null;
            }

            public Node(Node L, Node R)
            {
                left = L;
                right = R;
                nodesWeight = L.nodesWeight + R.nodesWeight;
            }
        }

        static List<int> code = new List<int>();
        static List<int> tempList;
        static Dictionary<char, List<int> > table = new Dictionary<char, List<int> >();

        static void BuildTable(Node root)
        {
            if (root.left != null)
            {
                code.Add(0);
                BuildTable(root.left);
            }

            if (root.right != null)
            {
                code.Add(1);
                BuildTable(root.right);
            }

            if (root.left == null && root.right == null)
            {
                tempList = new List<int>();
                tempList.AddRange(code);
                table.Add(root.symbol, tempList);
            }

            if (code.Count != 0)
            {
                code.RemoveAt(code.Count - 1);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Введите сообщение");
            string text = Convert.ToString(Console.ReadLine());

            int textLength = text.Length;
            const int N = 73; // мощность алфавита - буквы и .,!?-""()
            char[] alphabet = new char[N];
            int[] numberOfSymbols = new int[N];
            int pointer = 0;
            bool flag = false;

            for (int i = 0; i < text.Length; ++i)
            {
                for (int j = 0; j < pointer + 1 && j < N; ++j)
                {
                    if (alphabet[j] != text[i])
                    {
                        flag = true;
                    }
                    else
                    {
                        ++numberOfSymbols[j];
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    alphabet[pointer] = text[i];
                    ++numberOfSymbols[pointer];
                    ++pointer;
                    flag = false;
                }
            }

            Dictionary<char, int> symbolQuantityPairs = new Dictionary<char, int>();
            for (int i = 0; i < pointer; ++i)
            {
                symbolQuantityPairs.Add(alphabet[i], numberOfSymbols[i]);
            }

            List<Node> t = new List<Node>();

            foreach (KeyValuePair <char, int> letter in symbolQuantityPairs.OrderBy(key => key.Value))
            {
                Node p = new Node();
                p.symbol = letter.Key;
                p.nodesWeight = letter.Value;
                t.Add(p);
            }

            while (t.Count != 1)
            {
                Node BranchL = t[0];
                t.RemoveAt(0);

                Node BranchR = t[0];
                t.RemoveAt(0);

                Node parent = new Node(BranchL, BranchR);
                t.Add(parent);
            }

            Node root = t[0];

            BuildTable(root);

            List<int> codedLetters;
            char c = ' ';
            int counter = 0;
            for (int i = 0; i < text.Length; ++i)
            {
                c = text[i];
                codedLetters = new List<int>();
                codedLetters = table[c];

                for (int j = 0; j < codedLetters.Count; ++j)
                {
                    Console.Write(codedLetters[j]);
                    ++counter;
                }
                //Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();

            foreach (KeyValuePair <char, List<int> > letter in table)
            {
                Console.Write(letter.Key + " - ");
                for (int i = 0; i < letter.Value.Count; ++i)
                {
                    Console.Write(letter.Value[i]);
                }
                Console.WriteLine();
            }

            double I1 = textLength * 8;
            double I2 = counter;
            double I3 = I2 / I1 * 100;

            Console.WriteLine();
            Console.WriteLine("Исходное сообщение содержит " + I1 + " бит информации");
            Console.WriteLine("Закодированное сообщение содержит " + I2 + " бит информации");
            Console.WriteLine("Исходное сообщение уменьшилось на " + I3 + "%");

            Console.Read();
        }
    }
}

// наша саша шла по шоссе
// во дворе трава на траве дрова
// сшит колпак да не по-колпаковски, надо колпак переколпаковать
// Ана, дэус, рики, паки, Дормы кормы констунтаки, Дэус дэус канадэус – бац!