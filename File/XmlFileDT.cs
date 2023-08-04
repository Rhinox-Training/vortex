using System;
using System.IO;
using System.Xml.Serialization;
using Rhinox.Lightspeed.IO;

namespace Rhinox.Vortex.File
{
    public abstract class XmlFileDT<T> : FileDataTableSerializer<T>
    {
        public XmlFileDT(FileEndPoint endPoint, string tableName) 
            : base(endPoint, tableName)
        {
        }
        
        protected override bool TryDeserialize(string path, out T[] dataObjs)
        { 
            XmlSerializer serializer = new XmlSerializer(typeof(T[]));
            using (StreamReader reader = new StreamReader(path))
            {
                dataObjs = (T[])serializer.Deserialize(reader.BaseStream);
            }
            return true;
        }

        protected override bool Serialize(string path, T[] dataObjs)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T[]));
            using (StreamWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer.BaseStream, dataObjs);
            }
            return true;
        }
    }
}