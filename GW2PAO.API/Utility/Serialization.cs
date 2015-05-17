using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace GW2PAO.API.Util
{
    /// <summary>
    /// Static helper methods for serializing objects to disk
    /// </summary>
    public static class Serialization
    {
        public static void SerializeToXml<T>(T obj, string filename)
        {
            // Create the output directory if it doesn't exist
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            var serializer = new XmlSerializer(typeof(T));
            TextWriter textWriter = new StreamWriter(filename, false, Encoding.Unicode);
            try
            {
                serializer.Serialize(textWriter, obj);
            }
            finally
            {
                textWriter.Close();
            }
        }

        public static void SerializeToJson<T>(T obj, string filename)
        {
            // Create the output directory if it doesn't exist
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            var serializer = new JsonSerializer();
            TextWriter textWriter = new StreamWriter(filename, false, Encoding.Unicode);
            try
            {
                serializer.Serialize(textWriter, obj);
            }
            finally
            {
                textWriter.Close();
            }
        }

        public static T DeserializeFromXml<T>(string filename)
        {
            var loadedData = default(T);

            if (!File.Exists(filename))
                return loadedData;

            var deserializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StreamReader((filename)))
            {
                loadedData = (T)deserializer.Deserialize(reader);
            }

            return loadedData;
        }

        public static T DeserializeFromJson<T>(string filename)
        {
            var loadedData = default(T);
            
            if (!File.Exists(filename))
                return loadedData;

            var deserializer = new JsonSerializer();
            using (TextReader reader = new StreamReader((filename)))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    loadedData = deserializer.Deserialize<T>(jsonReader);
                }
            }

            return loadedData;
        }
    }
}
