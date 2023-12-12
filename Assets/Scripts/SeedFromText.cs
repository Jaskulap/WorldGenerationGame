using System;
using System.Security.Cryptography;
using System.Text;

public class SeedFromText
{
    public static int GenerateSeedFromText(string text)//0-1000
    {
        if (text == null)
        {
            Random random = new Random();
            return random.Next(1000);
        }
        // Create an instance of the MD5 hash algorithm
        using (MD5 md5 = MD5.Create())
        {
            // Convert the input text to ASCII bytes
            byte[] inputBytes = Encoding.ASCII.GetBytes(text);

            // Compute the hash of the input bytes
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the hash bytes to an integer seed
            int seed = BitConverter.ToInt32(hashBytes, 0);

            // Ensure the seed is within the range of 0 to 1000
            seed = seed % 1001;

            // Ensure the seed is non-negative
            if (seed < 0) seed = -seed;

            return seed;
        }
    }
}
