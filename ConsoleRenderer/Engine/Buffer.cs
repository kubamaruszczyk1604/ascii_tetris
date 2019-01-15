using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Text;

namespace ConsoleRenderer
{
    class Buffer
    {

        /* TEXT FRAME BUFFER FOR FAST RENDERING */
        /* by Kuba Maruszczyk */
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)]
            public char UnicodeChar;
            [FieldOffset(0)]
            public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)]
            public CharUnion Char;
            [FieldOffset(2)]
            public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        static SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
        static int m_sWidth;
        static int m_sHeight;
        static CharInfo[] buf;
        static SmallRect rect;

        static int m_sBuffPtr;

        static public void Initialize(short posX, short posY, short width, short height)
        {
            m_sWidth = width;
            m_sHeight = height;
            m_sBuffPtr = 0;
            if (!h.IsInvalid)
            {
                buf = new CharInfo[width * height];
                rect = new SmallRect() { Left = posX, Top = posY, Right = (short)(width + posX), Bottom = (short)(height + posY) };

            }
        }

        static public void AddSequentialy(char c, short color)
        {

            buf[m_sBuffPtr].Attributes = color;
            buf[m_sBuffPtr].Char.AsciiChar = (byte)c;
            m_sBuffPtr++;
            if (m_sBuffPtr >= buf.Length) m_sBuffPtr = 0;

        }

        static public void ClearA(string [] bkg)
        {
            bool addingStr = false;
            int ind = 0;
            int indc = 0;
            var e = Encoding.GetEncoding("437");
            Array.Clear(buf, 0, buf.Length);
            for (int i = 0; i < buf.Length; ++i)
            {
                if (i % (m_sWidth) == 0)
                {
                    addingStr = true;
                    ind++;
                    if (ind >= bkg.Length) addingStr = false;
                    indc = 0;
                    AddSequentialy((char)176, 0x0007);
                }
                else
                {
                    if(addingStr)
                    {
                        if (indc >= bkg[ind].Length - 1)
                        {
                            addingStr = false;
                            AddSequentialy((char)176, 0x0007);
                        }
                        else
                        {
                            char c = bkg[ind][indc];
                            byte[] charByte = e.GetBytes(c.ToString());
                            indc++;
                            byte f = charByte[0];
                            if (f == 32) f = 176;
                            AddSequentialy((char)f, 0x0007);
                            

                        }
                    }
                    else AddSequentialy((char)176, 0x0007);
                }
            }
        }
        static public void Clear()
        {
            Array.Clear(buf, 0, buf.Length);
            for (int i = 0; i < buf.Length; ++i)
            {
              AddSequentialy((char)176, 0x0007);
            }
        }
     
        static public void DrawXY(char c, short color, int x, int y)
        {
            int index = m_sWidth * (y) + x;
            if (index >= buf.Length)
            {
                index = 0;
                //throw new Exception("DLUGOSC JEST: " + index.ToString());
            }

            buf[index].Attributes = color;
            buf[index].Char.AsciiChar = (byte)c;
        }

        static public void Swap()
        {
            WriteConsoleOutput(h, buf,
                         new Coord() { X = (short)m_sWidth, Y = (short)m_sHeight },
                         new Coord() { X = 0, Y = 0 },
                         ref rect);

            m_sBuffPtr = 0;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COLORREF
    {
        private uint ColorDWORD;

        internal COLORREF(uint r, uint g, uint b)
        {
            ColorDWORD = r + (g << 8) + (b << 16);
        }

        public override string ToString()
        {
            return ColorDWORD.ToString();
        }
    }
}
