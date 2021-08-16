using System;
using System.Xml;

namespace TextureAtlas2Cpp
{
    class Program
    {
        static void Main(string[] args)
        {
            string source, headerFile;
            XmlDocument doc;

            if (args.Length == 0)
            {
                Console.WriteLine("No arguments passed");

                return;
            }

            source = args[0];

            doc = HeaderGenerator.ValidateFile(source, out headerFile);

            if (doc == null)
            {
                return;
            }

            if (HeaderGenerator.GenerateHeader(doc, headerFile))
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine("Error!");
            }
        }
    }
}
