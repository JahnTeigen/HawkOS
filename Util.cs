using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SUMS
{
    public static class Util
    {
        public static void Header(string header, ConsoleColor BG, ConsoleColor FG)
        {
            Console.BackgroundColor = BG;
            Console.ForegroundColor = FG;
            Console.WriteLine(header);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static string Input(string header, ConsoleColor BG, ConsoleColor FG)
        {
            Console.BackgroundColor = BG;
            Console.ForegroundColor = FG;
            Console.Write($"{header} > ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            var input = Console.ReadLine();

            return input;
        }

        public static void CommandInput(string header)
        {
            Console.Write($"{header} ==> ");
            string input = Console.ReadLine();
            Commands.CheckCommands(input);
        }

        public static void SysMsg(string header)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"SYS : {header}");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
