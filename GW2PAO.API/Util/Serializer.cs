using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GW2PAO.API.Util
{
    /// <summary>
    /// Static helper methods for serializing objects to disk
    /// </summary>
    public static class Serializer
    {
        public static void SerializeToXml<T>(T obj, string filename)
        {
            // Create the output directory if it doesn't exist
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextWriter textWriter = new StreamWriter(filename);
            try
            {
                serializer.Serialize(textWriter, obj);
            }
            finally
            {
                textWriter.Close();
            }
        }
    }
}
