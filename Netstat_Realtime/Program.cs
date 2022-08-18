using System;

namespace Netstat_Realtime
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Parse command line arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Netstat_Realtime.exe <poll_interval_milliseconds> <log_to_console (true/false)");
                Console.WriteLine("\tExample 1 - Poll 10 times per second with console logging:\r\n\t\tNetstat_Realtime.exe 100 true");
                Console.WriteLine("\tExample 2 - Poll every 5 seconds without console logging:\r\n\t\tNetstat_Realtime.exe 5000 false");
                Environment.Exit(1);
            }
            var parseInterval = int.TryParse(args[0], out int intervalMs);
            if (!parseInterval) Console.WriteLine($"Failed to parse interval: '{args[0]}'");
            var parseLogToConsole = bool.TryParse(args[1], out bool logToConsole);
            if (!parseLogToConsole) Console.WriteLine($"Failed to parse interval: '{args[1]}'");

            // Set CSV log file name
            var dateTime = DateTime.Now.ToString("s").Replace(":", "-");
            var logFile = $".\\netstat_realtime_{dateTime}.csv";
            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"Logging to file: {logFile}");
            Console.WriteLine($"Press any key to exit");
            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine("");

            // Setup realtime poller and start
            var netstat = new NetstatLogger(intervalMs, logFile, logToConsole);
            netstat.Start();

            // Wait for keystroke and exit
            Console.ReadKey();
            netstat.Stop();
        }
    }
}