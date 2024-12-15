using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SUMS
{
    // Static
    public static class Config
    {
        // Program info
        public static string logo = @"
      ___           ___           ___           ___     
     /\  \         /\  \         /\  \         /|  |    
     \:\  \       /::\  \       _\:\  \       |:|  |    
      \:\  \     /:/\:\  \     /\ \:\  \      |:|  |    
  ___ /::\  \   /:/ /::\  \   _\:\ \:\  \   __|:|  |    
 /\  /:/\:\__\ /:/_/:/\:\__\ /\ \:\ \:\__\ /\ |:|__|____
 \:\/:/  \/__/ \:\/:/  \/__/ \:\ \:\/:/  / \:\/:::::/__/
  \::/__/       \::/__/       \:\ \::/  /   \::/~~/~    
   \:\  \        \:\  \        \:\/:/  /     \:\~~\     
    \:\__\        \:\__\        \::/  /       \:\__\    
     \/__/         \/__/         \/__/         \/__/    
";
        public static string name = "HawkOS";
        public static string nameFull = "High-Altitude Warfare and Kombat Operating System";
        public static string version = "1.3.0";
        public static string copyright = "Copyright (c) 2024-2100 Hawk Two Systems";

        // Program data
        public static bool standbyMode = false;
        public static string mode = "GUI";

        // Settings
        public static Dictionary<string, bool> settings = new Dictionary<string, bool>
        {
            { "Enable Logging", true },
            { "Auto Save", true },
            { "Debug Mode", false },
            { "I/O", false },
            { "Audio", false },
            { "Boot to terminal", false },
            { "Rizz Protocol", false },
            { "Hawk Two Media Encoder (TM)", false },
            { "Hawk Two Memory Optimisation Unit (TM)", false },
        };


        public const string FilePath = "config.txt";

        // Methods
        public static void WriteSettings(Dictionary<string, bool> settings)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    foreach (var setting in settings)
                    {
                        writer.WriteLine($"{setting.Key}={setting.Value}");
                    }
                }
                Console.WriteLine("Settings successfully written to file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

        public static void ReadSettings(Dictionary<string, bool> settings)
        {
            try
            {
                // Check if the file exists
                if (!File.Exists(FilePath))
                {
                    Console.WriteLine($"File '{FilePath}' does not exist. Creating a new one...");

                    // Create the file in the root directory
                    using (StreamWriter writer = new StreamWriter(FilePath))
                    {
                        // Optionally, write default settings into the new file
                        foreach (var setting in settings)
                        {
                            writer.WriteLine($"{setting.Key}={setting.Value}");
                        }
                    }
                    Console.WriteLine($"File '{FilePath}' created with default settings.");
                    return;
                }

                // Read the settings from the file
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            var key = parts[0].Trim();
                            var value = parts[1].Trim().ToLower() == "true";

                            if (settings.ContainsKey(key))
                            {
                                settings[key] = value;
                            }
                            else
                            {
                                Console.WriteLine($"Warning: Key '{key}' in the file is not in the current settings.");
                            }
                        }
                    }
                }
                Console.WriteLine("Settings successfully read from file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling file: {ex.Message}");
            }
        }
    }

    public static class Users
    {
        public static string loggedIn = "";

        public static Dictionary<string, string> users = new Dictionary<string, string>();

        public static void InitializeUsers()
        {
            users.Add("Bridge", Encryption.EncryptPassword("Bruker99!"));

            Console.WriteLine("Users Initialized");
        }
    }

    public static class Systems
    {
        public static void START()
        {
            // DEBUG
            var volumes = Kernel.fs.GetVolumes();

            foreach (var volume in volumes)
            {
                var rootPath = volume.mName; // This is the disk identifier like "0:\"
                Console.WriteLine($"Volume: {rootPath}");

                var entries = Kernel.fs.GetDirectoryListing(rootPath);

                foreach (var entry in entries)
                {
                    Console.WriteLine($@"{rootPath}{entry.mName}");
                }
            }

            // Initialization
            Console.WriteLine("Cosmos booted successfully.");
            Users.InitializeUsers();
            Stations.InitializeStations();
            InitializeFilesystem();

            // Kernel logo
            Util.Header($"{Config.logo}", ConsoleColor.Black, ConsoleColor.Magenta);
            Util.Header($"{Config.nameFull} {Config.version} - {Config.copyright}", ConsoleColor.Magenta, ConsoleColor.White);

            Authenticate.AuthenticateGUI();
        }

        public static void InitializeFilesystem()
        {
            try
            {
                string baseDirectory = @"0:\HawkOS\Users\";

                // Check if the base directory exists, if not, create it
                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                }

                // Loop through users and ensure their directories exist
                foreach (var user in Users.users)
                {
                    string userDirectory = $@"{baseDirectory}{user.Key}";
                    if (!Directory.Exists(userDirectory))
                    {
                        Directory.CreateDirectory(userDirectory);
                    }
                }

                Console.WriteLine("File system initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize file system: {ex.Message}");
            }
        }
    }

}
