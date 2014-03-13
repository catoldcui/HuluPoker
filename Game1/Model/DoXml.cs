using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using System.IO.IsolatedStorage;

namespace Game1.Model
{
    class DoXml
    {
        public static void LoadDataFromXml()
        {
            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("Config.xml", System.IO.FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Config));
                        Config data = (Config)serializer.Deserialize(stream);
                        Config.setConfig(data);
                        Config.IsXmlExist = true;
                    }
                }
            } catch{
                SaveDataToXml();
                LoadDataFromXml();
                Config.IsXmlExist = false;
            }
        }

        
        public static void SaveDataToXml()
        {
            Config c = new Config();
            c.GenerateData();

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("Config.xml", System.IO.FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Config));
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, c);
                    }
                }
            }
        }
    }
}
