using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    class Board
    {
        public delegate void OnLinesCompleted(int lineCount);
        private OnLinesCompleted m_OnLinesCompleted;

        public delegate void OnDeletingLine();
        private OnDeletingLine m_OnDeletingLine;

        private int m_SizeX;
        private int m_SizeY;
        private int[,] m_States;
        private List<Cell> m_Cells;
        private DateTime m_NextDrop;

        /******************** Private Methods *******************/

        private void DeleteRow(int rowNo)
        {
            InvokeOnDeletingLine();
            for (int j = 0; j < m_SizeX; ++j)
            {
                m_Cells.RemoveAll(o => o.Y == rowNo && o.X == (rowNo%2 == 0? m_SizeX-j:j));
                
                BoardRenderer.DrawFrame(Cells);
                
            }

            //m_Cells.RemoveAll(o => o.Y == rowNo);
            for (int i = 0; i < m_SizeX; ++i)
            {
                m_States[i, rowNo] = 0;
            }

            List<Cell> aboveCells = m_Cells.FindAll(o => o.Y < rowNo);
            foreach (Cell c in aboveCells)
            {
                //c.Active = true;
                // m_States[c.X, c.Y] = m_States[c.X, c.Y - 1];
                c.MoveTo(c.X, c.Y + 1);
                // m_States[c.X, c.Y] = c.ID;
            }

            m_States = new int[m_SizeX, m_SizeY];
            foreach (Cell c in m_Cells)
            {
                m_States[c.X, c.Y] = c.ID;
            }

        }

        private void InvokeLinesCompleteEvent(int lineCount)
        {
            if (m_OnLinesCompleted != null) m_OnLinesCompleted(lineCount);
        }

        private void InvokeOnDeletingLine()
        {
            if (m_OnDeletingLine != null) m_OnDeletingLine();
        }

        /******************** Public Interface *******************/

        public List<Cell> Cells { get { return m_Cells; } }
        public int[,] States { get { return m_States; } }
        private float m_DropRate;


        public Board(int sizeX, int sizeY)
        {
            m_SizeX = sizeX;
            m_SizeY = sizeY;
            m_States = new int[sizeX, sizeY];
            m_Cells = new List<Cell>();
            m_DropRate = 0.5f;
            m_NextDrop = DateTime.Now;
        }

        public void Reset()
        {
            if (m_OnLinesCompleted != null)
            {
                foreach (Delegate d in m_OnLinesCompleted.GetInvocationList())
                {
                    m_OnLinesCompleted -= (OnLinesCompleted)d;
                }
            }
            if (m_OnDeletingLine != null)
            {
                foreach (Delegate d in m_OnDeletingLine.GetInvocationList())
                {
                    m_OnDeletingLine -= (OnDeletingLine)d;
                }
            }

            m_States = new int[m_SizeX, m_SizeY];
            m_Cells = new List<Cell>();
            m_DropRate = 0.5f;
            m_NextDrop = DateTime.Now;
        
        }

        public Cell AddCell(int x, int y, int id,int col)
        {
            if (m_States[x, y]!=0) return null;
            if (x > (m_SizeX - 1)) return null;
            if (y > (m_SizeY - 1)) return null;
            Cell c = new Cell(x, y, col ,id);
            m_Cells.Add(c);
            m_States[x, y] = c.ID;
            return c;          
        }

        public bool MoveActiveBlockBy(int x, int y)
        {
            List<int> positions = new List<int>();
            List<Cell> cellsToMove = new List<Cell>();

            foreach (Cell c in m_Cells) //Find all cells to move
            {
                if (!c.Active) continue;
                int newX = c.X + x;
                int newY = c.Y + y;

                if (newX > (m_SizeX - 1) || newX < 0) return false;
                if (newY > (m_SizeY - 1) || newY < 0) return false;

                if (m_States[newX,newY]==0 || m_States[newX, newY] == c.ID)
                {
                    positions.Add(newX);
                    positions.Add(newY);
                    cellsToMove.Add(c);
                }
                else 
                {
                    return false; 
                }
                
            }

            int ind = 0;
            foreach (Cell c in cellsToMove)
            {
                m_States[c.X, c.Y] = 0;
            }

            foreach (Cell c in cellsToMove)
            {
                c.MoveTo(positions[ind], positions[ind + 1]);
                m_States[c.X, c.Y] = c.ID;
                ind += 2;
            }
            return true;
        }

        public bool MoveCurrentBlockDown()
        {
            if (m_NextDrop <= DateTime.Now)
            {
                m_NextDrop = DateTime.Now + TimeSpan.FromSeconds(m_DropRate);
                return MoveActiveBlockBy(0, 1);
            }
            return true;
        }

        public void SetDropRate(int level)
        {
            m_DropRate = 1.0f/level * 0.5f;
        }

        public bool MoveCurrentBlockDownFast()
        { 
        
            return MoveActiveBlockBy(0, 1);
          
        }

        public void SetCurrentBlockAsStatic()
        {
            foreach (Cell c in m_Cells)
            {
                if (c.Active) c.Active = false;
            }
        }

        public bool RotateBlock(Cell pivot)
        {
            List<int> positions = new List<int>();
            List<Cell> cellsToMove = new List<Cell>();
            foreach (Cell c in m_Cells)
            {
                if (!c.Active) continue;
                int x = c.X - pivot.X;
                int y = c.Y - pivot.Y;
                float si = (float)Math.Sin(1.57f);
                float co = (float)Math.Cos(1.57f);

                float xfnew = x * co + y * -si;
                float yfnew = x * si + y * co;

                int newX = (int)Math.Round(xfnew) + pivot.X;
                int newY = (int)Math.Round(yfnew) + pivot.Y;

                if (newX > (m_SizeX - 1) || newX < 0) return false;
                if (newY > (m_SizeY - 1) || newY < 0) return false;
                if (m_States[newX, newY] == 0 || m_States[newX, newY] == c.ID)
                {
                    positions.Add(newX);
                    positions.Add(newY);
                    cellsToMove.Add(c);
                }
                else
                {
                    return false;
                }
            }
            int ind = 0;
            foreach (Cell c in cellsToMove)
            {
                m_States[c.X, c.Y] = 0;
            }
            foreach (Cell c in cellsToMove)
            {
                c.MoveTo(positions[ind], positions[ind + 1]);
                m_States[c.X, c.Y] = c.ID;
                ind += 2;
            }
            return true;


        }

        public int DetectAndHandleCopmleteRows()
        {
            List<int> fullNumbers = new List<int>();
            for(int i = 0; i < m_SizeY; ++i)
            {
                bool rowFull = true;
                for(int j = 0; j < m_SizeX;++j)
                {
                    if(m_States[j,i] == 0)
                    {
                        rowFull = false;
                        break;
                    }
                }
                if (rowFull)
                {
                    
                    fullNumbers.Add(i);
                    //Console.Beep(900 * fullNumbers.Count, 500);
                   

                }
            }

            foreach(int i in fullNumbers)
            {
                DeleteRow(i);
            }
            if (fullNumbers.Count > 0)
            { InvokeLinesCompleteEvent(fullNumbers.Count); }
            //fullNumbers.Clear();
            return fullNumbers.Count;
        }

        public void Subscribe_OnLineCompleted(OnLinesCompleted listener)
        {
            m_OnLinesCompleted += listener;
        }

        public void Unsubscribe_OnLineCompleted(OnLinesCompleted listener)
        {
            m_OnLinesCompleted -= listener;
        }

        public void Subscribe_OnDeletingLine(OnDeletingLine listener)
        {
            m_OnDeletingLine += listener;
        }

        public void Unsubscribe_OnDeletingLine(OnDeletingLine listener)
        {
            m_OnDeletingLine -= listener;
        }
    }
}
