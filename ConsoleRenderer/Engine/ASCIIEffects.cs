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
                if(Console.KeyAvailable) Console.ReadKey(false);
                Console.CursorVisible = false;
                if (screenStr[i] == 'W')
                {
                    i++; //If possible control character found - advance by one
                    string numStr = string.Empty;
                    //Get numbers that follow the control character
                    while (char.IsNumber(screenStr[i]))
                    {
                        numStr += screenStr[i];
                        Console.Write(' ');//replace control char with space
                        i++;
                        if (i >= screenStr.Length) break;
                    }
                    //if no numbers follow it wasn't really the control character, just ordinary letter..
                    if (numStr == string.Empty)
                    {
                        Console.Write('W');//Write original letter
                    }
                    else //in case it was the control character..
                    {
                        Console.Write(' ');//replace control char with space
                        Thread.Sleep(int.Parse(numStr));
                    }

                    continue;
                }
                if (screenStr[i] == '^') //Enable scanline animation
                {
                    Console.Write(' ');//replace control char with space
                    i++;
                    scanlineStyle = true;
                    continue;
                }
                if (screenStr[i] == '!')//Disable scanile animation
                {
                    Console.Write(' ');//replace control char with space
                    i++;
                    scanlineStyle = false;
                }
                if (screenStr[i] == 'C')
                {
                    i++;
                    string numStr = string.Empty;
                    while (char.IsNumber(screenStr[i]))
                    {
                        numStr += screenStr[i];
                        Console.Write(' ');//replace control char with space
                        i++;
                        if (i >= screenStr.Length) break;
                    }
                    if (numStr == string.Empty)
                    {
                        Console.Write('C');
                    }
                    else
                    {
                        Console.Write(' ');//replace control char with space
                        currentCol = int.Parse(numStr);
                        Console.ForegroundColor = GetConsoleColor(currentCol);
                    }

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

        static public void AnimatedText(string text,int x, int y, int timeInterval_ms)
        {
            Console.SetCursorPosition(x, y);
            for(int i = 0; i < text.Length; ++i)
            {
                Console.Write(text[i]);
                if(text[i] != ' ') Thread.Sleep(timeInterval_ms);
            }
        }
    }
}
