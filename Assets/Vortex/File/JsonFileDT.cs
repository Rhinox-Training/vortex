using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Rhinox.Vortex.File
{
    public class JsonFileDT<T> : FileDataTableSerializer<T>
    {
        private JsonSerializerSettings _settings;

        public JsonFileDT(FileEndPoint endPoint, string tableName) : base(endPoint, tableName)
        {
            _settings = CreateSettings();
        }

        protected virtual JsonSerializerSettings CreateSettings()
        {
            return new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented,
            };
        }
        
        protected override bool TryDeserialize(string path, out T[] dataObjs)
        {
            try
            {
                string json = System.IO.File.ReadAllText(path);
                dataObjs = JsonConvert.DeserializeObject<T[]>(json, _settings);
                return true;
            }
            catch (Exception)
            {
                dataObjs = Array.Empty<T>();
                return false;
            }
        }

        protected override bool Serialize(string path, T[] dataObjs)
        {
            try
            {
                string json = JsonConvert.SerializeObject(dataObjs, _settings);
                System.IO.File.WriteAllText(path, json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}