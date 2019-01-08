using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace ConsoleRenderer
{
    class ASCIIEffects
    {

        public static ConsoleColor GetConsoleColor(int colID)
        {
            return (ConsoleColor)colID;
        }

        public static string LoadAsString(string path)
        {
            StreamReader reader = new StreamReader(path);
            string read = reader.ReadToEnd();
            reader.Close();
            return read;
        }

        public static string[] LoadAsStringArray(string path)
        {
            List<string> output = new List<string>();
            StreamReader reader = new StreamReader(path);

            while (!reader.EndOfStream)
            {
                output.Add(reader.ReadLine());

            }
            reader.Close();
            return output.ToArray();
        }

        public static void DisplayScreen(string screenStr)
        {
            int i = 0;
            bool scanlineStyle = false;
            int currentCol = 15;
            while (true)
            {
                Console.CursorVisible = false;
                if (screenStr[i] == 'W')
                {
                    Console.Write(' ');//replace control char with space
                    i++;
                    string numStr = string.Empty;
                    while (char.IsNumber(screenStr[i]))
                    {
                        numStr += screenStr[i];
                        Console.Write(' ');//replace control char with space
                        i++;
                        if (i >= screenStr.Length) break;
                    }

                    Thread.Sleep(int.Parse(numStr));
                    continue;
                }
                if (screenStr[i] == 'S')
                {
                    Console.Write(' ');//replace control char with space
                    i++;
                    scanlineStyle = true;
                    continue;
                }
                if (screenStr[i] == 'O')
                {
                    Console.Write(' ');//replace control char with space
                    i++;
                    scanlineStyle = false;
                }
                if (screenStr[i] == 'C')
                {
                    Console.Write(' ');//replace control char with space
                    i++;
                    string numStr = string.Empty;
                    while (char.IsNumber(screenStr[i]))
                    {
                        numStr += screenStr[i];
                        Console.Write(' ');//replace control char with space
                        i++;
                        if (i >= screenStr.Length) break;
                    }
                    currentCol = int.Parse(numStr);
                    Console.ForegroundColor = GetConsoleColor(currentCol);

                    continue;
                }
                if (i >= screenStr.Length) break;
                if (scanlineStyle && !char.IsWhiteSpace(screenStr[i]) && i%3==0) Thread.Sleep(5);
                Console.Write(screenStr[i]);
                i++;
                if (i >= screenStr.Length) break;

            }
        }


        static public void Animate(string[] strings, int bufferWidth, int bufferHigh)
        {
            List<string> left = new List<string>();
            List<string> right = new List<string>();

            for (int y = 0; y < strings.Length; ++y)
            {


                if (y % 2 == 0) Console.WriteLine(strings[y]);
                else Console.WriteLine();
            }
        }
    }
}
