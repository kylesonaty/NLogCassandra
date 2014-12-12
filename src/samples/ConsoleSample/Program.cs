using System;
using NLog;

namespace ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Warn("This is a test of the emergency broadcast system");
            Console.WriteLine("Enter a message to log. Type 'exit' to quit!");
            var input = "";
            while (input != null && !input.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(input))
                    logger.Info(input);
                input = Console.ReadLine();
            }
        }
    }
}
