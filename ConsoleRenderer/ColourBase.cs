

namespace ConsoleRenderer
{
    class ColourBase // ALL MATERIAL RELATED CONSTANTS
    {
        public const short BACKGROUND_INTENSITY = 0x0080;
        public const short FOREGROUND_INTENSITY = 0x0008;
        //Bitmasks for colors 

        //Green
        public const short BACKGROUND_GREEN = 0x0020;
        public const short FOREGROUND_GREEN = 0x0002;

        //Red
        public const short BACKGROUND_RED = 0x0040;
        public const short FOREGROUND_RED = 0x0004;

        //Blue
        public const short BACKGROUND_BLUE = 0x0010;
        public const short FOREGROUND_BLUE = 0x0001;

        //Red
        public const short BACKGROUND_YELLOW = 0x0040 | 0x0020;
        public const short FOREGROUND_YELLOW = 0x0004 | 0x0002;

        public const short BACKGROUND_CYAN = 0x0020 | 0x0010;
        public const short FOREGROUND_CYAN = 0x0002 | 0x0001;

        public const short BACKGROUND_MAGENTA = 0x0040 | 0x0010;
        public const short FOREGROUND_MAGENTA = 0x0004 | 0x0001;

        public const short BACKGROUND_WHITE = 0x0040 | 0x0010 | 0x0020;
        public const short FOREGROUND_WHITE = 0x0004 | 0x0001 | 0x0002;
    }
}
