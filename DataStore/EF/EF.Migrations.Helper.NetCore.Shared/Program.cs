using System;
using System.Threading.Tasks;

namespace EF.Migrations.Helper
{
    class Program
    {
        const ConsoleColor _ConsoleBackgroundColor = ConsoleColor.Blue;
        const ConsoleColor _ConsoleForegroundColor = ConsoleColor.Yellow;

        static void Main(string[] args)
        {
            Console.BackgroundColor = _ConsoleBackgroundColor;
            Console.ForegroundColor = _ConsoleForegroundColor;

            Console.WriteLine("Migration Helper Test Started");

            bool success = false;

            try
            {
                success = EFMigrationHelper.Launch().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration Helper Test Failed.\n{ex}");
            }

            Console.BackgroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"Migration Helper Test {(success ? "Success" : "Failed")}");

            Console.BackgroundColor = _ConsoleBackgroundColor;
            Console.ForegroundColor = _ConsoleForegroundColor;

            Console.ResetColor();
        }
    }
}
