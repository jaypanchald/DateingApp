using System;

namespace Dating.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            var test = GetGroupName("lola", "roy");
            var test2 = GetGroupName("roy", "lola");
        }

        private static string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}~{other}" : $"{other}~{caller}";
        }
    }
}
