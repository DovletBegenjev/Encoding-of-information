using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeorInf4
{
    class Program
    {
        class Node
        {
            private List<double> left = new List<double>(); // правая ветвь
            private List<double> right = new List<double>(); // левая ветвь
            private double difference = 1, new_difference = 0; // разность до и после
            private int center = 0; // центр последовательности
            private List<double> freqOfSymbols_ordered;

            public Node(List<double> freqOfSymbols_ordered)
            {
                int size = freqOfSymbols_ordered.Count;
                this.freqOfSymbols_ordered = freqOfSymbols_ordered;
                center = freqOfSymbols_ordered.Count / 2;
            }

            public void FindCenter(int[,] codedLetters, int pointerOfDeepness, int startPos)
            {
                int size = freqOfSymbols_ordered.Count;

                for (int i = 0; i < size; ++i)
                {
                    right.Add(freqOfSymbols_ordered[i]);
                }

                for (int i = 0; i < size; ++i)
                {
                    left.Add(freqOfSymbols_ordered[i]);
                    right.Remove(freqOfSymbols_ordered[i]);
                    new_difference = right.Sum() - left.Sum();
                    if (new_difference > 0)
                    {
                        if (new_difference <= difference)
                        {
                            difference = new_difference;
                            center = i + 2;
                        }
                    }
                    else break;
                }

                EncodeLetters(pointerOfDeepness, codedLetters, startPos);
            }

            public void EncodeLetters(int pointerOfDeepness, int[,] codedLetters, int startPos)
            {
                for (int i = startPos; i < left.Count + startPos; ++i)
                {
                    codedLetters[i, pointerOfDeepness] = 0;
                }

                for (int i = startPos; i < right.Count + startPos; ++i)
                {
                    codedLetters[i + center, pointerOfDeepness] = 1;
                }
            }

            public List<double> GetLeftNode()
            {
                return left;
            }

            public List<double> GetRightNode()
            {
                return right;
            }

            public int GetCenterOfFreq()
            {
                return center + 1;
            }
        }

        static void FormLeftNodes(List<double> left, List<double> right, Node node, int pointerOfDeepness, int center, int startPos, int[,] codedLetters)
        {
            List<double> freqOfSymbols_left = new List<double>(); // массив частот в порядке убывания для левой ветви
            while (left.Count != 1)
            {
                freqOfSymbols_left.Clear();
                for (int i = 0; i < left.Count; ++i)
                {
                    freqOfSymbols_left.Add(left[i]);
                }

                node = new Node(freqOfSymbols_left);
                ++pointerOfDeepness;
                node.FindCenter(codedLetters, pointerOfDeepness, startPos);
                left = node.GetLeftNode();
                right = node.GetRightNode();
                center = node.GetCenterOfFreq();
                if (right.Count != 1)
                {
                    FormRightNodes(right, left, node, pointerOfDeepness, center, startPos + left.Count(), codedLetters);
                }
            }
        }

        static void FormRightNodes(List<double> right, List<double> left, Node node, int pointerOfDeepness, int center, int startPos, int[,] codedLetters)
        {
            List<double> freqOfSymbols_right = new List<double>(); // массив частот в порядке убывания для правой ветви
            while (right.Count != 1)
            {
                freqOfSymbols_right.Clear();
                for (int i = 0; i < right.Count; ++i)
                {
                    freqOfSymbols_right.Add(right[i]);
                }

                node = new Node(freqOfSymbols_right);
                ++pointerOfDeepness;
                node.FindCenter(codedLetters, pointerOfDeepness, startPos);
                right = node.GetRightNode();
                left = node.GetLeftNode();
                center = node.GetCenterOfFreq();
                if (left.Count != 1)
                {
                    FormLeftNodes(left, right, node, pointerOfDeepness, center, startPos, codedLetters);
                    startPos += left.Count();
                }
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

            Dictionary<char, double> freq = new Dictionary<char, double>(); // ассоцитативный массив символов и их частот
            for (int i = 0; i < N; ++i)
            {
                if (numberOfSymbols[i] != 0)
                {
                    freq.Add(alphabet[i], Math.Round(Convert.ToDouble(numberOfSymbols[i]) / Convert.ToDouble(textLength), 3));
                }
            }

            List<char> arrOfSymbols_ordered = new List<char>(); // массив символов в порядке убывания
            List<double> freqOfSymbols_ordered = new List<double>(); // массив частот в порядке убывания

            foreach (KeyValuePair <char, double> letter in freq.OrderByDescending(key => key.Value))
            {
                arrOfSymbols_ordered.Add(letter.Key);
                freqOfSymbols_ordered.Add(letter.Value);
            }

            int GPS = 0; // "глобальный" указатель - показывает, какая буква сейчас кодируется
            List<double> left = new List<double>(); // правая ветвь
            List<double> right = new List<double>(); // левая ветвь
            int center = freqOfSymbols_ordered.Count; // центр последовательности
            int size = freqOfSymbols_ordered.Count;
            int pointerOfDeepness = 0; // указатель глубины дерева
            int[,] codedLetters = new int[size, size - 1]; // массив кодов символов
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size - 1; ++j)
                {
                    codedLetters[i, j] = -1;
                }
            }

            Node node = new Node(freqOfSymbols_ordered);

            node.FindCenter(codedLetters, pointerOfDeepness, GPS);
            left = node.GetLeftNode();
            right = node.GetRightNode();
            center = node.GetCenterOfFreq();

            FormLeftNodes(left, right, node, pointerOfDeepness, center, GPS, codedLetters);

            pointerOfDeepness = 0;
            GPS = center - 1;

            FormRightNodes(right, left, node, pointerOfDeepness, center, GPS, codedLetters);

            int[,] codedText = new int[textLength, size - 1]; // массив кодов символов
            List<int> codedTextList = new List<int>();

            for (int i = 0; i < textLength; ++i)
            {
                for (int j = 0; j < size - 1; ++j)
                {
                    codedText[i, j] = -1;
                }
            }

            for (int i = 0; i < textLength; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    if (text[i] == arrOfSymbols_ordered[j])
                    {
                        for (int a = 0; a < size - 1; ++a)
                        {
                            if (codedLetters[j, a] != -1)
                            {
                                codedText[i, a] = codedLetters[j, a];
                                codedTextList.Add(codedLetters[j, a]);
                            }
                            else break;
                        }
                        break;
                    }
                }
            }

            for (int i = 0; i < textLength; ++i)
            {
                for (int j = 0; j < size - 1; ++j)
                {
                    if (codedText[i, j] != -1)
                    {
                        Console.Write(codedText[i, j]);
                    }
                }
                //Console.Write(" ");
            }

            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < freqOfSymbols_ordered.Count; ++i)
            {
                Console.Write(arrOfSymbols_ordered[i] + " - ");
                for (int j = 0; j < freqOfSymbols_ordered.Count - 1; ++j)
                {
                    if (codedLetters[i, j] != -1)
                    {
                        Console.Write(codedLetters[i, j]);
                    }
                }
                Console.WriteLine();
            }

            double I1 = textLength * 8;
            double I2 = codedTextList.Count;
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