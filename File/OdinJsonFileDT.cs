#if ODIN_INSPECTOR
using System;
using Rhinox.Lightspeed.IO;

namespace Rhinox.Vortex.File
{
    public abstract class OdinJsonFileDT<T> : FileDataTable<T>
    {
        protected override bool TryDeserialize(string path, out T[] infos)
        {
            var fileContent = FileHelper.ReadAllBytes(path);
            infos = SerializationUtility.DeserializeValue<T[]>(fileContent, DataFormat.JSON);
            return true;
        }

        protected override bool Serialize(string path, T[] infos)
        {
            byte[] json = SerializationUtility.SerializeValue(dataObjs != null ? dataObjs.ToArray() : Array.Empty<T>(), DataFormat.JSON);
            
            System.IO.File.WriteAllBytes(path, json);
            return true;
        }
    }
}
#endif