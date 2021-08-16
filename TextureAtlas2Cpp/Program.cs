using System;
using System.Xml;
using System.IO;

namespace TextureAtlas2Cpp
{
    class Program
    {
        static void Main(string[] args)
        {
            string source, className, file;
            XmlDocument doc;

            if (args.Length == 0)
            {
                Console.WriteLine("No arguments passed");

                return;
            }

            source = args[0];
            className = args[1];

            doc = ClassGenerator.ValidateFile(source);

            if (doc == null)
            {
                return;
            }

            file = Path.GetDirectoryName(source) + "/" + className.ToSnakeCase();

            ClassGenerator.GenerateHeader(className, file);

            ClassGenerator.GenerateCpp(doc, className, file);
        }
    }
}
