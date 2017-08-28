using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace MsTestToXunitConverter
{
    public class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: mstestconvert <path-to-csproj>");
                return 1;
            }

            ProjectConverter.ConvertProject(args[0]).Wait();

            return 0;
        }
    }
}
