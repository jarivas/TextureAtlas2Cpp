using System;
using System.Xml;
using System.IO;
using System.Text;

namespace TextureAtlas2Cpp
{
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string ToSnakeCase(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Length < 2)
            {
                return text;
            }

            var sb = new StringBuilder();

            sb.Append(char.ToLowerInvariant(text[0]));

            for (int i = 1; i < text.Length; ++i)
            {
                char c = text[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }

    class ClassGenerator
    {
        public static XmlDocument ValidateFile(string xmlFile)
        {
            if (!File.Exists(xmlFile))
            {
                Console.WriteLine("The file does not exists");
                return null;
            }

            XmlDocument doc = LoadFile(xmlFile);

            return (doc.HasChildNodes) ? doc : null;
        }

        private static XmlDocument LoadFile(string xmlFile)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xmlFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                doc = null;
            }

            return doc;
        }

        public static void GenerateHeader(string className, string file)
        {
            string headerFile = file + ".h";
            string fileContent = @"#pragma once
#include ""raylib.h""
#include <map>
#include <string>
" + $"class {className}\r\n{{\r\npublic:\r\n\t{className}();\r\n\tRectangle GetRectangle(std::string key);\r\nprivate:\r\n\tstd::map<std::string, Rectangle> tiles;\r\n}};";

            if (File.Exists(headerFile))
            {
                File.Delete(headerFile);
            }

            using (FileStream fs = File.Open(headerFile, FileMode.CreateNew))
            {
                fs.Write(Encoding.ASCII.GetBytes(fileContent));
            }
        }

        public static void GenerateCpp(XmlDocument doc, string className, string file)
        {
            string headerFile = className.ToSnakeCase() + ".h";
            string cppFile = file + ".cpp";
            string fileContent = $"#include \"{headerFile}\"\r\n#include \"raylib.h\"\r\n#include <map>\r\n#include <string>\r\n"
                + $"\r\n{className}::{className}()\r\n{{";


            if (File.Exists(cppFile))
            {
                File.Delete(cppFile);
            }

            using (FileStream fs = File.Open(cppFile, FileMode.CreateNew))
            {
                foreach (XmlNode node in doc.ChildNodes.Item(0))
                {
                    fileContent += String.Format("\r\n\ttiles[\"{0}\"] = {{{1}, {2}, {3}, {4}}};", node.Attributes["name"].Value.Replace(".png", ""),
                        node.Attributes["x"].Value, node.Attributes["y"].Value, node.Attributes["width"].Value, node.Attributes["height"].Value);
                }

                fileContent += "\r\n}\r\n";

                fileContent += $"\r\nRectangle {className}::GetRectangle(std::string key)\r\n{{\r\n\treturn tiles[key];\r\n}}";
                fs.Write(Encoding.ASCII.GetBytes(fileContent));
            }
        }
    }

}
