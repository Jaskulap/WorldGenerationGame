using System;
using System.Security.Cryptography;
using System.Text;

public class SeedFromText
{
    public static int GenerateSeedFromText(string text)//0-1000
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(text);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            int seed = BitConverter.ToInt32(hashBytes, 0);
            seed = seed % 1001;
            if (seed < 0) seed = -seed;
            return seed;
        }
    }
}
