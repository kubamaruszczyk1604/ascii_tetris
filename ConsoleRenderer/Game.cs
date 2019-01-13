using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace ConsoleRenderer
{
    //htp://patorjk.com/software/taag/#p=display&f=Graffiti&t=Type%20Something%20
    enum BlockType
    {
        Long = 0, Square = 1, LShapedL = 2, LShapedR = 3,
        SShapedL = 4, SShapedR = 5,Piramid = 6
    }

    enum ActionType { NoAction, MoveLeft, MoveRight, FastDown, Rotate}

   
    class Game
    {

        static int[] COL_TABLE =
       {
            ColourBase.BACKGROUND_BLUE | ColourBase.FOREGROUND_RED,
           ColourBase.BACKGROUND_BLUE, 
           ColourBase.BACKGROUND_BLUE | ColourBase.FOREGROUND_BLUE,
            ColourBase.BACKGROUND_RED | ColourBase.FOREGROUND_BLUE//,

        };
        const int c_SizeX = 16;//24
        const int c_SizeY = 30;

        static Board s_Board;
        static ActionType s_RequestedAction = ActionType.NoAction;
        static Random s_Random;
        static bool s_Running = false;
        static Queue<BlockType> s_NextBlocks;
        static ScoreManager m_ScoreManager;

        /******************** Private Methods *******************/

        private static int GenRandomColour()
        {
            return COL_TABLE[s_Random.Next(0, COL_TABLE.Length)];
        }

        private static void GeneratePutSound()
        {
            Console.Beep(200, 5);
        }

        private static void GenerateRowSound()
        {
            //int[] frequencies = { 523, 587, 659, 698, 783 };
            //in C major scale
            int[] frequencies = { 523, 659, 783, 659, 698, 783 };
            for (int i = 0; i < frequencies.Length; ++i)
            {
                Console.Beep(frequencies[i], 20); 
            }
        }

        private static bool CreateBlock(int id, BlockType type, out Cell pivotCell)
        {
            int col = GenRandomColour();
            pivotCell = null;
            if(s_Board.AddCell(10, 0, id,col)==null) return false;
            pivotCell = s_Board.AddCell(10, 1, id,col);
            if (pivotCell == null) return false;

            if (type == BlockType.Long)
            {         
                if(s_Board.AddCell(10, 2, id, col) == null)
                    return false;
                if(s_Board.AddCell(10, 3, id, col) == null)
                    return false;
                return true;
            }
            else if (type == BlockType.LShapedR)
            {
                if(s_Board.AddCell(10, 2, id, col) == null) return false;
                if(s_Board.AddCell(11, 2, id, col) == null) return false;
                return true;
            }
            else if (type == BlockType.LShapedL)
            {
                if(s_Board.AddCell(10, 2, id, col) == null) return false;
                if(s_Board.AddCell(9, 2, id, col) == null) return false;
                return true;
            }
            else if (type == BlockType.Piramid)
            {
                if(s_Board.AddCell(10, 2, id, col) == null) return false;
                if(s_Board.AddCell(11, 1, id, col) == null) return false;
                return true;
            }
            else if (type == BlockType.SShapedL)
            {
                if(s_Board.AddCell(9, 1, id, col) == null) return false;
                if(s_Board.AddCell(9, 2, id, col) == null) return false;
                return true;
            }
            else if (type == BlockType.SShapedR)
            {
                if(s_Board.AddCell(11, 1, id, col) == null) return false;
                if(s_Board.AddCell(11, 2, id, col) == null) return false; 
                return true;
            }
            else if (type == BlockType.Square)
            {
                if(s_Board.AddCell(11, 0, id, col) == null) return false;
                if(s_Board.AddCell(11, 1, id, col) == null) return false;
                return true;
            }
            return false;
        }

        private static void DrawNextBlockPreview(int x, int y, BlockType type)
        {

            Console.SetCursorPosition(x, y);
            BoardRenderer.DrawWindow(x-2, y-1, 12, 6, "NEXT", ConsoleColor.Black, ConsoleColor.White);
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            if (type == BlockType.Long)
            {
                Console.SetCursorPosition(x, y + 1);
                Console.Write("▒▒▒▒▒▒▒▒");
            }
            else if (type == BlockType.LShapedR)
            {
                y += 1;
                Console.SetCursorPosition(x + 2, y);
                Console.WriteLine("▒▒");
                Console.SetCursorPosition(x + 2, y + 1);
                Console.WriteLine("▒▒");
                Console.SetCursorPosition(x + 2, y + 2);
                Console.WriteLine("▒▒▒▒");

            }
            else if (type == BlockType.LShapedL)
            {
                y += 1;
                Console.SetCursorPosition(x + 4, y);
                Console.WriteLine("▒▒");
                Console.SetCursorPosition(x + 4, y + 1);
                Console.WriteLine("▒▒");
                Console.SetCursorPosition(x + 2, y + 2);
                Console.WriteLine("▒▒▒▒");
               // char c = '▒';
                //byte cr = (byte)(c);
            }
            else if (type == BlockType.Piramid)
            {

                Console.SetCursorPosition(x + 3, y + 1);
                Console.WriteLine("▒▒");
                Console.SetCursorPosition(x + 1, y + 2);
                Console.WriteLine("▒▒▒▒▒▒");
            }
            else if (type == BlockType.SShapedL)
            {
                y += 1;
                Console.SetCursorPosition(x + 4, y);
                Console.WriteLine("▒▒");
                Console.SetCursorPosition(x + 2, y + 1);
                Console.WriteLine("▒▒▒▒");
                Console.SetCursorPosition(x + 2, y + 2);
                Console.WriteLine("▒▒");
            }
            else if (type == BlockType.SShapedR)
            {
                y += 1;
                Console.SetCursorPosition(x + 2, y);
                Console.WriteLine("▒▒");
                Console.SetCursorPosition(x + 2, y + 1);
                Console.WriteLine("▒▒▒▒");
                Console.SetCursorPosition(x + 4, y + 2);
                Console.WriteLine("▒▒");
            }
            else if (type == BlockType.Square)
            {
                Console.SetCursorPosition(x + 2, y + 1);
                Console.WriteLine("▒▒▒▒");
                Console.SetCursorPosition(x + 2, y + 2);
                Console.WriteLine("▒▒▒▒");
            }
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.Red;
        }

        private static void DrawStatsBoard(int x, int y, int score, int newGoal, int level)
        {
            BoardRenderer.DrawWindow(x, y, 36, 6, "STATS", ConsoleColor.DarkRed, ConsoleColor.Black);
            x += 2;
            y += 2;
            BoardRenderer.DrawWindow(x, y-1, 11, 4, "Level", ConsoleColor.DarkRed, ConsoleColor.Gray);
            Console.SetCursorPosition(x + 4, y );
            Console.Write("L " + level.ToString());
            Console.SetCursorPosition(x + 4, y + 1);
            Console.Write(m_ScoreManager.LevelProgressPercent.ToString() + " %");
            //Console.Write(level.ToString());
            BoardRenderer.DrawWindow(x+12,  y-1, 20, 4, "Score", ConsoleColor.DarkRed, ConsoleColor.Gray);
            Console.SetCursorPosition(x + 13, y);
            Console.Write(" Your: " + new string(' ',9 - score.ToString().Length) + score.ToString());
            Console.SetCursorPosition(x + 13, y + 1);
            Console.Write(" Next Lv: " + new string(' ', 6 - newGoal.ToString().Length) + newGoal.ToString());

        }

        private static void OnKeyPressed(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Spacebar)
            {
                s_RequestedAction = ActionType.Rotate;
            }
            else if (key.Key == ConsoleKey.LeftArrow)
            {
                s_RequestedAction = ActionType.MoveLeft;
            }
            else if (key.Key == ConsoleKey.RightArrow)
            {
                s_RequestedAction = ActionType.MoveRight;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                s_RequestedAction = ActionType.FastDown;
            }
        }

        private static BlockType GenerateRandomBlock()
        {
            return (BlockType)s_Random.Next(0, 7); 
        }

        private static void OnGameOver()
        {
            s_Running = false;
        }

        private static void OnLinesCompleted(int lines)
        {

            m_ScoreManager.AddScore(lines);
            //Console.Beep(900 , 100);
        }

        private static void OnDeletingLine(bool completed)
        {
            if (completed)
            {
                Thread t = new Thread(GenerateRowSound);
                t.Start();
            }
            else
            {
               GeneratePutSound();
               
            }
        }

        private static void OnNextLevel(int level)
        {
            s_Board.SetDropRate(level);
            s_Board.ClearCells();
        }

        /******************** Public Interface *******************/

        public static void ShowIntroScreen()
        {
            Console.SetWindowSize(144, 30 + 4);
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            string s = ASCIIEffects.LoadAsString(@"Assets\Intro.txt");
            ASCIIEffects.DisplayScreen(s);

            //string [] s = ASCIIEffects.LoadAsStringArray(@"C:\Users\Kuba\Desktop\ac.txt");
            //ASCIIEffects.Animate(s,144, 34);

            Console.ReadKey();
            Console.Clear();
        }

        public static void InitializeGame()
        {
            s_Board = new Board(c_SizeX, c_SizeY);
            s_Random = new Random();
            s_NextBlocks = new Queue<BlockType>();

            Console.SetWindowSize(78, 30 + 5);

            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
           
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            string s = ASCIIEffects.LoadAsString(@"Assets\Gameon.txt");
            ASCIIEffects.DisplayScreen(s);
            BoardRenderer.Initialize(4, 2, c_SizeX * 2, c_SizeY + 1);
           // 
            Console.CursorVisible = false;
            InputEventGenerator.ev_KeyPressed += OnKeyPressed;
            InputEventGenerator.Start();
            s_Board.Subscribe_OnLineCompleted(OnLinesCompleted);
            s_Board.Subscribe_OnDeletingLine(OnDeletingLine);
        }

        public static void ShutDown()
        {
            InputEventGenerator.Stop();
            ResetGame();
        }
    
        public static void ResetGame()
        {
            s_Board.Reset();
            s_RequestedAction = ActionType.NoAction;
            s_NextBlocks.Clear();
            s_Board.Subscribe_OnLineCompleted(OnLinesCompleted);
            s_Board.Subscribe_OnDeletingLine(OnDeletingLine);
        }

        public static void Run()
        {
            s_Running = true;
            int blockID = 1;
            Cell pivot = null;
            s_NextBlocks.Enqueue(GenerateRandomBlock());
            s_NextBlocks.Enqueue(GenerateRandomBlock());
            CreateBlock(blockID, s_NextBlocks.Dequeue(), out pivot);
            int nextBlockWindowPosX = 53;
            int nextBlockWindowPosY = 24;
            int statsBoardPosX = 40;
            int statsBoardPosY = 15;
            m_ScoreManager = new ScoreManager();
            m_ScoreManager.Subscribe_OnNextLevel(OnNextLevel);
            DrawStatsBoard(statsBoardPosX, statsBoardPosY, m_ScoreManager.CurrentScore, m_ScoreManager.m_NextLevelTreshold, m_ScoreManager.Level);
            DrawNextBlockPreview(nextBlockWindowPosX, nextBlockWindowPosY, s_NextBlocks.Peek());
            while (s_Running)
            {
                BoardRenderer.DrawFrame(s_Board.Cells);
                // BoardRenderer.DrawDebug(s_Board.States);
                if (!s_Board.MoveCurrentBlockDown())
                {
                    s_Board.SetCurrentBlockAsStatic();
                    BlockType nextBlock = s_NextBlocks.Dequeue();
                    s_NextBlocks.Enqueue(GenerateRandomBlock());
                    GeneratePutSound();
                    s_Board.DetectAndHandleCopmleteRows();
                    blockID++;
                    if(!CreateBlock(blockID, nextBlock, out pivot))
                    {
                        OnGameOver();   
                    }
                    DrawStatsBoard(statsBoardPosX, statsBoardPosY, m_ScoreManager.CurrentScore, m_ScoreManager.m_NextLevelTreshold, m_ScoreManager.Level);
                    DrawNextBlockPreview(nextBlockWindowPosX, nextBlockWindowPosY, s_NextBlocks.Peek());
                }


                if(s_RequestedAction == ActionType.Rotate)
                {
                    s_Board.RotateBlock(pivot);
                }
                else if(s_RequestedAction == ActionType.MoveLeft)
                {
                    s_Board.MoveActiveBlockBy(-1, 0);
                }
                else if (s_RequestedAction == ActionType.MoveRight)
                {
                    s_Board.MoveActiveBlockBy(1, 0);
                }
                else if(s_RequestedAction == ActionType.FastDown)
                {
                    if (!s_Board.MoveCurrentBlockDownFast())
                    {
                        s_Board.SetCurrentBlockAsStatic();
                        BlockType nextBlock = s_NextBlocks.Dequeue();
                        s_NextBlocks.Enqueue(GenerateRandomBlock());
                        GeneratePutSound();
                        s_Board.DetectAndHandleCopmleteRows();
                        blockID++;
                        if (!CreateBlock(blockID, nextBlock, out pivot))
                        {
                            OnGameOver();
                        }
                        DrawStatsBoard(statsBoardPosX, statsBoardPosY, m_ScoreManager.CurrentScore, m_ScoreManager.m_NextLevelTreshold, m_ScoreManager.Level);
                        DrawNextBlockPreview(nextBlockWindowPosX, nextBlockWindowPosY, s_NextBlocks.Peek());

                    }
                   
                }

                s_RequestedAction = ActionType.NoAction;
  
            }

        }
    }
}
