using System.Globalization;
using Newtonsoft.Json;

namespace Rhinox.Vortex.File
{
    public abstract class JsonFileDT<T> : FileDataTable<T>
    {
        private JsonSerializerSettings _settings;

        public override bool Initialize(DataEndPoint endPoint)
        {
            _settings = CreateSettings();
            return base.Initialize(endPoint);
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
            string json = System.IO.File.ReadAllText(path);
            dataObjs = JsonConvert.DeserializeObject<T[]>(json, _settings);
            return true;
        }

        protected override bool Serialize(string path, T[] dataObjs)
        {
            string json = JsonConvert.SerializeObject(dataObjs, _settings);
            System.IO.File.WriteAllText(path, json);
            return true;
        }
    }
}