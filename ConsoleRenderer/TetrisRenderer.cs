using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleRenderer
{
    class BoardRenderer
    {

        static int[] SINGLE = { 0, 1, 1, 1 };
        public static void DrawSingle(int x, int y,short col)
        {
            Buffer.DrawXY((char)Block.Middle, col, SINGLE[0]+x, SINGLE[1]+y);
            Buffer.DrawXY((char)Block.Middle, col, SINGLE[2]+x, SINGLE[3]+y);
        }

        public static void Initialize(short x, short y, short width, short height)
        {
            DrawWindow(x-2, y-1, width+4, height+2, "BOARD", ConsoleColor.DarkGray, ConsoleColor.White);
            Buffer.Initialize(x,y,width, height);
        }

        public static void DrawFrame(List<Cell> activeCells)
        {
            Console.SetCursorPosition(0, 0);
            Buffer.Clear();
            foreach(Cell c in activeCells)
            {
                DrawSingle(c.X*2, c.Y,(short)c.Colour);
            }
            Buffer.Swap();
            Thread.Sleep(30);
            //Console.ReadLine();

        }
    
        public static void DrawDebug(int [,] boardStates)
        {
           
            Buffer.Clear();
            Console.SetCursorPosition(0, 4);
            for (int i = 0;i < boardStates.GetLength(0);++i)
            {
                for (int j = 0; j < boardStates.GetLength(1); ++j)
                {
                    if(boardStates[i,j] != 0)
                        DrawSingle(i * 2, j, 1);
                }
            }

           
            Buffer.Swap();
            Thread.Sleep(50);
            //Console.ReadLine();

        }

        public static void DrawWindow(int x, int y, int w, int h, string title, ConsoleColor bkgCol, ConsoleColor foreCol)
        {
            Console.BackgroundColor = bkgCol;
            Console.ForegroundColor = foreCol;

            Console.SetCursorPosition(x, y);
            int yTemp = y;
            for (int i = yTemp; i < yTemp + h; ++i)
            {
                Console.SetCursorPosition(x, i);
                if (i == yTemp)
                {
                    Console.Write("╔" + new String('═', w - 2) + "╗");
                }
                else if (i == (yTemp + h - 1))
                {
                    Console.Write("╚" + new String('═', w - 2) + "╝");
                }
                else
                {
                    Console.Write("║" + new String(' ', w - 2) + "║");
                }
            }
            Console.SetCursorPosition(x + w / 2 - (title.Length + 2) / 2, y);
            Console.Write(" " + title + " ");
        }
    }
}
