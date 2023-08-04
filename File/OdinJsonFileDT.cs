#if ODIN_INSPECTOR
using System;
using System.Linq;
using Rhinox.Lightspeed.IO;
using Sirenix.Serialization;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace Rhinox.Vortex.File
{
    public class OdinJsonFileDT<T> : FileDataTableSerializer<T>
    {
        public OdinJsonFileDT(FileEndPoint endPoint, string tableName) 
            : base(endPoint, tableName)
        {
        }

        protected override bool TryDeserialize(string path, out T[] infos)
        {
            var fileContent = FileHelper.ReadAllBytes(path);
            infos = SerializationUtility.DeserializeValue<T[]>(fileContent, DataFormat.JSON);
            return infos != null;
        }

        protected override bool Serialize(string path, T[] infos)
        {
            byte[] json = SerializationUtility.SerializeValue(infos ?? Array.Empty<T>(), DataFormat.JSON);
            
            System.IO.File.WriteAllBytes(path, json);
            return true;
        }
    }
}
#endif