using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SUMS
{
    public static class UI
    {
        private static int selectedIndex = 0;
        private static List<string> menuItems = new List<string> { "File Explorer", "Config", "Console", "Exit" };

        public static void Mode()
        {
            while (true)
            {
                switch (Config.mode)
                {
                    case "GUI":
                        GUI();
                        break;
                    case "CMD":
                        Cmd();
                        break;
                    default:
                        return; // Exit if the mode is invalid.
                }
            }
        }

        public static void Cmd()
        {
            bool running = true;
            Commands.InitializeCommands(); // Ensure commands are initialized

            while (running)
            {
                Console.Clear();
                Util.Header("COMMAND PROMPT", ConsoleColor.DarkBlue, ConsoleColor.White);

                Console.WriteLine($"Welcome {Users.loggedIn},");
                Console.WriteLine();
                Console.Write("> ");

                string commandInput = Console.ReadLine()?.Trim();
                if (commandInput?.ToLower() == "exit")
                {
                    running = false; // Exit the CMD loop
                }
                else
                {
                    Commands.CheckCommands(commandInput); // Use your existing Commands class
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        public static void GUI()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                DisplayMenu();
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + menuItems.Count) % menuItems.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % menuItems.Count;
                        break;
                    case ConsoleKey.Enter:
                        running = HandleSelection();
                        break;
                }
            }
        }

        private static void DisplayMenu()
        {
            DrawHeader($"{Config.name} - {Config.version}");
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"  > {menuItems[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"    {menuItems[i]}");
                }
            }
            DrawFooter();
        }

        private static bool HandleSelection()
        {
            switch (menuItems[selectedIndex])
            {
                case "File Explorer":
                    OpenFileExplorer();
                    break;
                case "Config [ DISABLED ]":
                    // OpenConfig();
                    break;
                case "Console":
                    Console.Clear();
                    Config.mode = "CMD";
                    return false; // Switch to CMD mode.
                case "Exit":
                    Environment.Exit(0);
                    break;
            }

            return true; // Continue running the GUI.
        }

        private static void OpenFileExplorer()
        {
            int selectedFileIndex = 0;
            bool explorerRunning = true;
            const int maxVisibleItems = 5;

            if (Kernel.fs == null)
            {
                Console.WriteLine("File system is not initialized. Ensure VFS is set up.");
                Console.ReadKey(true);
                return;
            }

            var currentPath = @"0:\"; // Root directory
            var filesAndFolders = GetDirectoryListing(currentPath);

            while (explorerRunning)
            {
                Console.Clear();
                DrawHeader($"File Explorer - {currentPath}");

                // Calculate visible range for scrolling
                int startIndex = Math.Max(0, selectedFileIndex - maxVisibleItems / 2);
                int endIndex = Math.Min(filesAndFolders.Count, startIndex + maxVisibleItems);

                for (int i = startIndex; i < endIndex; i++)
                {
                    if (i == selectedFileIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"  > {filesAndFolders[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"    {filesAndFolders[i]}");
                    }
                }

                DrawFooter("Arrows to navigate, Enter to open, Backspace to go back, [TAB] New Dir, [C] New File, [E] Edit File");

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedFileIndex = (selectedFileIndex - 1 + filesAndFolders.Count) % filesAndFolders.Count;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedFileIndex = (selectedFileIndex + 1) % filesAndFolders.Count;
                        break;

                    case ConsoleKey.Enter:
                        var selectedPath = currentPath + filesAndFolders[selectedFileIndex];
                        var file = Kernel.fs.GetFile(selectedPath);
                        var directory = Kernel.fs.GetDirectory(selectedPath);

                        if (file != null)
                        {
                            ViewFileContent(selectedPath);
                        }
                        else if (directory != null)
                        {
                            currentPath = selectedPath + @"\"; // Navigate into the directory
                            filesAndFolders = GetDirectoryListing(currentPath);
                            selectedFileIndex = 0;
                        }
                        break;

                    case ConsoleKey.Backspace:
                        if (currentPath != @"0:\")
                        {
                            currentPath = currentPath.Substring(0, currentPath.LastIndexOf('\\'));
                            if (!currentPath.EndsWith(@"\")) currentPath += @"\"; // Ensure trailing backslash
                            filesAndFolders = GetDirectoryListing(currentPath);
                            selectedFileIndex = 0;
                        }
                        else
                        {
                            explorerRunning = false; // Exit the explorer
                        }
                        break;

                    case ConsoleKey.Tab:
                        CreateNewDirectory(currentPath);
                        filesAndFolders = GetDirectoryListing(currentPath); // Refresh
                        break;

                    case ConsoleKey.C:
                        CreateNewFile(currentPath);
                        filesAndFolders = GetDirectoryListing(currentPath); // Refresh
                        break;

                    case ConsoleKey.E:
                        var editPath = currentPath + filesAndFolders[selectedFileIndex];
                        var editFile = Kernel.fs.GetFile(editPath);

                        if (editFile != null)
                        {
                            EditFileContent(editPath);
                        }
                        break;
                }
            }
        }

        private static void ViewFileContent(string filePath)
        {
            var file = Kernel.fs.GetFile(filePath);
            if (file == null)
            {
                Console.WriteLine("File does not exist.");
                Console.ReadKey(true);
                return;
            }

            Console.Clear();
            DrawHeader($"Viewing File - {filePath}");
            using (var stream = file.GetFileStream())
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                var content = Encoding.ASCII.GetString(buffer);

                Console.WriteLine(content);
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }

        private static void EditFileContent(string filePath)
        {
            var file = Kernel.fs.GetFile(filePath);
            if (file == null)
            {
                Console.WriteLine("File does not exist.");
                Console.ReadKey(true);
                return;
            }

            Console.Clear();
            DrawHeader($"Editing File - {filePath}");
            string originalContent;

            using (var stream = file.GetFileStream())
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                originalContent = Encoding.ASCII.GetString(buffer);
            }

            var editedContent = new StringBuilder(originalContent);

            Console.WriteLine("Edit below (Press ESC to save and exit):");
            Console.WriteLine(editedContent);

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    using (var stream = file.GetFileStream())
                    {
                        stream.SetLength(0); // Clear existing content
                        var buffer = Encoding.ASCII.GetBytes(editedContent.ToString());
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    Console.WriteLine("\nFile saved.");
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (editedContent.Length > 0)
                        editedContent.Length -= 1; // Remove last character
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    editedContent.Append(Environment.NewLine);
                }
                else if (key.KeyChar != 0)
                {
                    editedContent.Append(key.KeyChar); // Add typed character
                }

                Console.Clear();
                DrawHeader($"Editing File - {filePath}");
                Console.WriteLine(editedContent);
            }
        }

        private static List<string> GetDirectoryListing(string path)
        {
            if (Kernel.fs == null)
                return new List<string>();

            var entries = Kernel.fs.GetDirectoryListing(path);
            return entries.Select(entry => entry.mName).ToList();
        }

        private static void CreateNewDirectory(string currentPath)
        {
            Console.Clear();
            DrawHeader("Create New Directory");
            Console.WriteLine("Enter the name of the new directory: ");
            var dirName = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(dirName))
            {
                try
                {
                    Kernel.fs.CreateDirectory(currentPath + dirName);
                    Console.WriteLine($"Directory '{dirName}' created successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Directory name cannot be empty.");
            }

            Console.WriteLine("Press any key to return...");
            Console.ReadKey(true);
        }

        private static void CreateNewFile(string currentPath)
        {
            Console.Clear();
            DrawHeader("Create New File");
            Console.WriteLine("Enter the name of the new file (with extension): ");
            var fileName = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    var file = Kernel.fs.CreateFile(currentPath + fileName);
                    Console.WriteLine($"File '{fileName}' created successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("File name cannot be empty.");
            }

            Console.WriteLine("Press any key to return...");
            Console.ReadKey(true);
        }


        private static void OpenConfig()
        {
            int selectedSettingIndex = 0;
            bool configRunning = true;

            while (configRunning)
            {
                Config.ReadSettings(Config.settings);

                Console.Clear();
                Util.SysMsg("FUCKING UNSTABLE DON'T USE YOU HAVE TO RESTART YOUR MACHINE JESUS CHRIST");
                DrawHeader("Config");

                var settingsKeys = new List<string>(Config.settings.Keys);
                for (int i = 0; i < settingsKeys.Count; i++)
                {
                    var key = settingsKeys[i];
                    var value = Config.settings[key] ? "ON" : "OFF";

                    if (i == selectedSettingIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"  > {key}: {value}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"    {key}: {value}");
                    }
                }

                DrawFooter("Use Arrows to navigate, Enter to toggle, Backspace to exit.");

                var keyInput = Console.ReadKey(true).Key;

                switch (keyInput)
                {
                    case ConsoleKey.UpArrow:
                        selectedSettingIndex = (selectedSettingIndex - 1 + settingsKeys.Count) % settingsKeys.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedSettingIndex = (selectedSettingIndex + 1) % settingsKeys.Count;
                        break;
                    case ConsoleKey.Enter:
                        var selectedKey = settingsKeys[selectedSettingIndex];
                        Config.settings[selectedKey] = !Config.settings[selectedKey];
                        break;
                    case ConsoleKey.Backspace:
                        Config.WriteSettings(Config.settings);
                        configRunning = false;
                        break;
                }
            }
        }

        private static void DrawHeader(string title)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string('=', Console.WindowWidth));
            Console.WriteLine(title.PadRight(Console.WindowWidth - 1));
            Console.WriteLine(new string('=', Console.WindowWidth));
            Console.ResetColor();
        }

        private static void DrawFooter(string message = "Use Up/Down to navigate, Enter to select.")
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string('=', Console.WindowWidth));
            Console.WriteLine(message.PadRight(Console.WindowWidth - 1));
            Console.WriteLine(new string('=', Console.WindowWidth));
            Console.ResetColor();
        }
    }
}