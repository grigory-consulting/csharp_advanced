// 3 Interfaces ->
// UppercaseInterface -> string to Uppercase
// ReplaceInterface -> Replaces text
// (HashInterface) -> compute SHA-256 on the input
#nullable enable
using System;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Exercises
{
    public interface IProcessor
    {
        string Name { get; }
        string Description { get; }
        string Run(string input, string[] args);

    }

    public sealed class UppercaseInterface : IProcessor
    {

        public string Name => "upper";
        public string Description => "Convert string to uppercase.";
        public string Run(string input, string[] args) => input.ToUpper();

    }
    public sealed class HashSha256Processor : IProcessor
    {
        public string Name => "hash";
        public string Description => "Compute hash of the input.";
        public string Run(string input, string[] args)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant() + Environment.NewLine;
        }
    }


    public class ReplaceProcessor : IProcessor
    {
        public string Name => "replace";
        public string Description => "Replace text";

        public string Run(string input, string[] args)
        {
            // processor(input, old, new) 
            if (args.Length < 2) return "Error: requires <old> <new>";
            return input.Replace(args[0], args[1]);

        }

    }

    public sealed class MyPlugin : IProcessor
    {
        public string Name => "myplugin";
        public string Description => "My Plugin";

        public string Run(string input, string[] args) => "Hi, Plugin!";
       
    }
    


    // 
    public static class InterfaceRegistry
    {

        private static readonly Dictionary<string, IProcessor> _processors;

        static InterfaceRegistry(){
            _processors = new Dictionary<string, IProcessor>();
            var processorTypes = typeof(InterfaceRegistry).Assembly
                .GetTypes()
                .Where(t => typeof(IProcessor)
                .IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in processorTypes)
            {
                var instance = (IProcessor)Activator.CreateInstance(type);
                _processors[instance.Name] = instance;
            }
        
        }




        public static IReadOnlyDictionary<string, IProcessor> All => _processors;
        // Expose a read-only view of all processors
        public static bool TryGet(string name, out IProcessor proc) => _processors.TryGetValue(name, out proc!);
        // Looks up a processor by name, returns true if found

    }

   

      

    public class DataProcessingEx
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0 || args[0].Equals("list"))
            {
                Console.WriteLine("Available interfaces:");
                foreach(var p in InterfaceRegistry.All.Values.OrderBy(p => p.Name))
                {
                    Console.WriteLine($" {p.Name} - {p.Description}");
                }
                return;
            }

            var name = args[0];

            //if name not in the list of interfaces then error

            if(!InterfaceRegistry.TryGet(name, out var proc))
            {
                Console.WriteLine($"Unknown processor '{name}'. Use 'list' to see available interfaces.");
                return;
            }


            // Parse optional --in <file> 
            //
            string? file = null; // string or null
            
            var procArgs = new List<string>();
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "--in" && i + 1 < args.Length)
                {
                    file = args[++i];
                    continue;
                }
                procArgs.Add(args[i]);

            }



            string input = file is not null ? File.ReadAllText(file) : ReadAllStdIn(); 
            string output = proc.Run(input,procArgs.ToArray());

        }

        private static string ReadAllStdIn()
        {
            if (!Console.IsInputRedirected) return ""; // optional 
            using var sr = new StreamReader(Console.OpenStandardInput());
            return sr.ReadToEnd();
        }

    }

}
