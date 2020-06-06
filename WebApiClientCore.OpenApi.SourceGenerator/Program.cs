﻿using CommandLine;
using System;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new OpenApiDocOptions();
            if (Parser.Default.ParseArguments(args, options))
            {
                var doc = new OpenApiDoc(options);
                doc.GenerateFiles();
            }
            else
            {
                Console.Read();
            }
        }
    }
}
