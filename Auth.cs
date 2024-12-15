using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SUMS
{
    public static class Authenticate
    {
        public static void AuthenticateGUI()
        {
            bool isAuthenticated = false;

            while (!isAuthenticated)
            {
                Util.Header("AUTHENTICATE [ USERNAME / PASSWORD ]", ConsoleColor.DarkGreen, ConsoleColor.White);
                var username = Util.Input("USERNAME", ConsoleColor.DarkGreen, ConsoleColor.White);
                var password = Util.Input("PASSWORD", ConsoleColor.DarkGreen, ConsoleColor.White);

                isAuthenticated = AuthenticateInput(username, password);

                if (!isAuthenticated)
                {
                    Console.Clear();
                    Util.Header("INVALID CREDENTIALS. PLEASE TRY AGAIN.", ConsoleColor.Red, ConsoleColor.White);
                }
                else
                {
                    Console.Clear();
                    Util.Header("AUTHENTICATION SUCCESSFUL", ConsoleColor.Green, ConsoleColor.White);
                }
            }
        }

        public static bool AuthenticateInput(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Util.Header("INVALID USERNAME OR PASSWORD", ConsoleColor.Red, ConsoleColor.White);
                return false;
            }

            if (Users.users.TryGetValue(username, out string storedHash))
            {
                Console.Clear();
                Users.loggedIn = username;
                return Encryption.VerifyPassword(password, storedHash);
            }

            Util.Header("USER NOT FOUND", ConsoleColor.Red, ConsoleColor.White);
            return false;
        }

        public static void Logout()
        {
            Kernel kernel = new Kernel();
            Users.loggedIn = String.Empty;
            
        }

    }

    public static class Encryption
    {
        public static string EncryptPassword(string plainText)
        {
            // Big poopoo icky sticky encwyption
            int hash = 0;
            foreach (char c in plainText)
            {
                hash = (hash * 31) + c; // Simple hashing logic
            }
            return hash.ToString(); // Return hash as string
        }

        public static bool VerifyPassword(string plainText, string storedHash)
        {
            string computedHash = EncryptPassword(plainText);
            return computedHash == storedHash;
        }
    }
}
