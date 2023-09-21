#if ODIN_INSPECTOR
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rhinox.Lightspeed.IO;
using Rhinox.Lightspeed.Reflection;
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

            if (infos == null && fileContent.Length > 12) // 12 character minimum (contains actual path info)
            {
                var text = FileHelper.ReadAllText(path);
                Regex regex = new Regex("\\\"\\$type\"\\:\\s\\\"([0-9]+)\\|([^\"]+)\"");
                var types = regex.Matches(text);
                foreach (Match match in types)
                {
                    string odinQualifiedName = match.Groups[2].Value;
                    string sanitizedName = ReflectionUtility.SanitizeAssemblyQualifiedName(odinQualifiedName);
                    text = text.Replace(odinQualifiedName, sanitizedName);
                }
                var bytes = ASCIIEncoding.ASCII.GetBytes(text);
                infos = SerializationUtility.DeserializeValue<T[]>(bytes, DataFormat.JSON);
            }
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