using System;
using System.Xml;
using System.IO;
using System.Text;

namespace TextureAtlas2Cpp
{
    class HeaderGenerator
    {
        const string headerFile = "tiles_map.h";

        public static XmlDocument ValidateFile(string xmlFile, out string headerFile)
        {
            headerFile = null;

            if (!File.Exists(xmlFile))
            {
                Console.WriteLine("The file does not exists");
                return null;
            }

            XmlDocument doc = LoadFile(xmlFile);

            if (doc.HasChildNodes)
            {
                headerFile = GetHeaderFile(xmlFile);
                return doc;
            }

            return null;
        }

        private static XmlDocument LoadFile(string xmlFile)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xmlFile);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                doc = null;
            }

            return doc;
        }

        private static string GetHeaderFile(string xmlFile)
        {
            return Path.GetDirectoryName(xmlFile) + "/" + HeaderGenerator.headerFile ;
        }

        public static bool GenerateHeader(XmlDocument doc, string headerFile)
        {
            bool result = false;
            string fileContent = @"#pragma once
#include <map>
#include <string>
#include ""raylib.h""

std::map<std::string, Rectangle> TilesMap;

void InitTilesMap()
{
";

            if (doc == null)
            {
                Console.WriteLine("Invalid XmlDocument");
                return false;
            }

            using (FileStream fs = File.Open(headerFile, FileMode.OpenOrCreate))
            {
                foreach (XmlNode node in doc.ChildNodes.Item(0))
                {
                    fileContent += String.Format("\tTilesMap[\"{0}\"] = {{{1}, {2}, {3}, {4}}}; \r\n", node.Attributes["name"].Value.Replace(".png", ""),
                        node.Attributes["x"].Value, node.Attributes["y"].Value, node.Attributes["width"].Value, node.Attributes["height"].Value);
                }

                fileContent += "}";
                fs.Write(Encoding.ASCII.GetBytes(fileContent));

                result = true;
            }

            return result;
        }
    }
}
