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

            if (args.Length > 1 && bool.TryParse(args[1], out var dryrun))
            {
                ProjectConverter.ConvertProject(args[0], dryrun).Wait();
            }
            else
            {
                ProjectConverter.ConvertProject(args[0]).Wait();
            }


            return 0;
        }
    }
}
