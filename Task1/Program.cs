using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace Task1
{
    internal class Program
    {
        const int numberOfTrials = 10000;
        static void Main(string[] args)
        {
            var passwordText = "Test123!abcabcatbTestTest123Test123Test123";
            var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 1, 8, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            for (int i = 0; i < numberOfTrials; i++)
            {
                var hash = GeneratePasswordHashUsingSaltOriginal(passwordText, salt);
            }

            stopwatch.Stop();

            Console.WriteLine($"Original: {stopwatch.ElapsedMilliseconds}");

            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 0; i < numberOfTrials; i++)
            {
                var hash = GeneratePasswordHashUsingSaltUpdated1(passwordText, salt);
            }

            stopwatch.Stop();

            Console.WriteLine($"Updated with BlockCopy: {stopwatch.ElapsedMilliseconds}");

            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 0; i < numberOfTrials; i++)
            {
                var hash = GeneratePasswordHashUsingSaltUpdated2(passwordText, salt);
            }

            stopwatch.Stop();

            Console.WriteLine($"Updated with List: {stopwatch.ElapsedMilliseconds}");
        }

        public static string GeneratePasswordHashUsingSaltOriginal(string passwordText, byte[] salt)
        {

            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;

        }

        public static string GeneratePasswordHashUsingSaltUpdated1(string passwordText, byte[] salt)
        {

            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, 16);
            Buffer.BlockCopy(hash, 0, hashBytes, 16, 20);
            //This class provides better performance for manipulating primitive types than similar methods in the System.Array class.
            //https://docs.microsoft.com/en-us/dotnet/api/system.buffer?redirectedfrom=MSDN&view=net-6.0
            //Array.Copy(salt, 0, hashBytes, 0, 16);
            //Array.Copy(hash, 0, hashBytes, 16, 20);

            var passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;

        }

        public static string GeneratePasswordHashUsingSaltUpdated2(string passwordText, byte[] salt)
        {

            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            List<byte> hash2 = new List<byte>();
            hash2.AddRange(pbkdf2.GetBytes(20));
            hash2.AddRange(salt);

            var passwordHash = Convert.ToBase64String(hash2.ToArray());

            return passwordHash;

        }
    }
}
