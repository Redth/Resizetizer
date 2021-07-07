using System;
using System.IO;

namespace ResizetizerCLI
{
    class Program
    {
        static string GetBasePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("Please pass path to yaml file");
                return;
            }
            string path = args[0];
            Console.WriteLine("trying path: " + path);
            if (!File.Exists(path))
            {
                Console.WriteLine("Help !! " + args[0] + " not found");
            }
            var r = new Resizetizer.Engine();

            //var configs = r.DeserializeConfigFile(path);

            string basePath = GetBasePath();
            r.Run(path);

        }
    }
}
