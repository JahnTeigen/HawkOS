using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

namespace SUMS
{
    public static class Commands
    {
        public static Dictionary<string, string> commands = new Dictionary<string, string>();

        public static void InitializeCommands()
        {
            commands.Clear();
            commands.Add("help", "Displays a list of available commands");
            commands.Add("sysinfo", "Displays basic system information");
            commands.Add("logout", "Logs out the current user");
            commands.Add("restart", "Restarts the system");
            commands.Add("shutdown", "Shuts down the system");
            commands.Add("modify", "Modify a user or other parameter (e.g., 'modify user')");
            commands.Add("standby", "Enter standby mode. WARNING: AUTH. CODE REQUIRED TO DISABLE");
            commands.Add("bootgui", "Boot to GUI");
            commands.Add("debugvfs", "VFS Information");

            Console.WriteLine("Commands Initialized");
        }

        public static void CheckCommands(string input)
        {
            if (commands.Count == 0)
            {
                InitializeCommands();
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("No command entered. Type 'help' for a list of commands.");
                return;
            }

            // Split input into command and parameters
            var parts = input.Split(' ', 2);
            var command = parts[0].ToLower();
            var parameters = parts.Length > 1 ? parts[1] : string.Empty;

            // Check if the command exists
            if (commands.ContainsKey(command))
            {
                ExecuteCommand(command, parameters);
            }
            else
            {
                Console.WriteLine($"Unknown command: {command}. Type 'help' for a list of commands.");
            }
        }

        private static void ExecuteCommand(string command, string parameters)
        {
            switch (command)
            {
                case "help":
                    Console.Clear();
                    DisplayHelp();
                    break;

                case "sysinfo":
                    Console.Clear();
                    DisplayInfo();
                    break;

                case "logout":
                    Console.Clear();
                    Authenticate.Logout();
                    break;

                case "restart":
                    Console.Clear();
                    Systems.START();
                    break;

                case "shutdown":
                    Console.Clear();
                    Console.WriteLine("Shutting down...");
                    Environment.Exit(0);
                    break;

                case "modify":
                    Console.Clear();
                    ModifyCommand(parameters);
                    break;

                case "standby":
                    Console.Clear();
                    Config.standbyMode = true;
                    StandbyMode();
                    break;

                case "bootgui":
                    Config.mode = "GUI";
                    UI.GUI();
                    break;

                case "debugvfs":
                    Console.Clear();
                    Console.WriteLine($"FS TYPE : {Kernel.fs.GetFileSystemType(@"0:\")}");
                    Console.WriteLine($"{Kernel.fs.GetAvailableFreeSpace(@"0:\")} B");
                    break;

                default:
                    Util.Header("Command not implemented", ConsoleColor.Red, ConsoleColor.White);
                    break;
            }
        }
        
        // Commands
        private static void ModifyCommand(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("No parameters provided for 'modify' command.");
                return;
            }

            foreach(var user in Users.users)
            {
                var args = parameters.Split(' ');
                if (args[1].ToLower() == user.Key)
                {
                    Util.Header($"Modifying user {user.Key}", ConsoleColor.DarkYellow, ConsoleColor.White);
                    Users.users.Clear();
                    Users.users.Add(Util.Input("New Username", ConsoleColor.Black, ConsoleColor.Yellow), user.Value);
                }
                else
                {
                    Console.WriteLine($"Unknown modify target: {args[0]}.");
                }
            }
        }

        // Helper methods
        public static void StandbyMode()
        {
            while (Config.standbyMode)
            {
                Console.Clear();
                Util.Header("STANDBY MODE ACTIVATED", ConsoleColor.Red, ConsoleColor.White);
                Util.Header("Please hold and await further instructions.", ConsoleColor.Black, ConsoleColor.Red);
                Util.Input("DEACTIVATION CODE", ConsoleColor.Black, ConsoleColor.Red);
                var code = Console.ReadLine();

                foreach(var user in Users.users)
                {
                    if(Authenticate.AuthenticateInput(user.Key, code))
                    {
                        Config.standbyMode = false;
                    }
                }
            }
        }

        public static void DisplayHelp()
        {
            foreach (var command in commands)
            {
                Util.Header($"{command.Key} - {command.Value}", ConsoleColor.DarkGray, ConsoleColor.White);
            }
        }

        public static void DisplayInfo()
        {
            Util.Header($"{Config.nameFull} ({Config.name})", ConsoleColor.Cyan, ConsoleColor.White);
            Util.Header($"{Config.copyright}", ConsoleColor.Cyan, ConsoleColor.White);
        }

        public static void RestartSystem()
        {
            Kernel kernel = new Kernel();
            kernel.Restart();
        }
    }

}