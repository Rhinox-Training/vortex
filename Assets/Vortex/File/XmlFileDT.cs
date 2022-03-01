using System;
using System.IO;
using System.Xml.Serialization;
using Rhinox.Lightspeed.IO;

namespace Rhinox.Vortex.File
{
    public abstract class XmlFileDT<T> : FileDataTable<T>
    {
        protected override bool TryDeserialize(string path, out T[] infos)
        { 
            XmlSerializer serializer = new XmlSerializer(typeof(T[]));
            using (StreamReader reader = new StreamReader(path))
            {
                infos = (T[])serializer.Deserialize(reader.BaseStream);
            }
            return true;
        }

        protected override bool Serialize(string path, T[] infos)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T[]));
            using (StreamWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer.BaseStream, infos);
            }
            return true;
        }
    }
}