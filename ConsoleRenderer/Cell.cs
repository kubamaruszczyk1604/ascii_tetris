using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRenderer
{
    class Cell
    {
        private int m_ID;
        private int m_X;
        private int m_Y;

        public int Colour { get; set; }
        public int X { get { return m_X; } }
        public int Y { get { return m_Y; }  }
        public int ID { get { return m_ID; } }
        public bool Active { get; set; }


        public Cell(int x, int y, int colour,int id)
        {
            m_X = x;
            m_Y = y;
            Colour = colour;
            Active = true;
            m_ID = id;
        }
          
        public void MoveTo(int x ,int y)
        {
            m_X = x;
            m_Y = y;
        }
     

    }
}
