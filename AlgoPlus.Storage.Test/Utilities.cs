using System;
using System.Linq;

public static class Utilities
{
    public static string GenerateString(int sizeInKb)
    {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(characters, sizeInKb * 1024)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}