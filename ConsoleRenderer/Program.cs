using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ConsoleRenderer
{
 

    class Program
    {
       
        static void Main(string[] args)
        {
          
            Game.ShowIntroScreen();
            Game.InitializeGame();
            Game.Run();
            Game.ShutDown();

        }
    }
}
