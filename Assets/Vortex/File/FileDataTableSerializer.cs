using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rhinox.Lightspeed.IO;
using Rhinox.Perceptor;

namespace Rhinox.Vortex.File
{
    public abstract class FileDataTableSerializer<T> : IDataTableSerializer<T>
    {
        private string _filePath;

        protected FileDataTableSerializer(FileEndPoint endPoint, string tableName)
        {
            FileEndPoint fileEndPoint = (FileEndPoint) endPoint;
            if (!FileHelper.IsPathRooted(fileEndPoint.Path))
            {
                PLog.Error<VortexLogger>(
                    $"Something went wrong, FileEndPoint<{typeof(T).Name}> was initialized with an invalidPath. Path: {fileEndPoint.Path} should be rooted.");
                //return false;
                throw new ArgumentException(nameof(endPoint));
            }

            _filePath = Path.Combine(fileEndPoint.Path, $"{tableName}.datatable");
        }

        public virtual ICollection<T> LoadData(bool createIfNotExists = false)
        {
            string path = _filePath;
            
            if (string.IsNullOrWhiteSpace(path))
            {
                PLog.Error<VortexLogger>($"Given path for '{nameof(LoadData)}' in FileDataTable<{typeof(T).Name}> is null.");
                return Array.Empty<T>();
            }
            
            if (!FileHelper.Exists(path))
            {
                if (createIfNotExists)
                {
                    SaveData(Array.Empty<T>());
                }
                else
                    PLog.Error<VortexLogger>($"FileDataTable<{typeof(T).Name}>.{nameof(LoadData)}: Could not find a file at path '{path}'");
                
                return Array.Empty<T>();
            }
            
            if (!TryDeserialize(path, out T[] infos))
            {
                PLog.Error<VortexLogger>($"FileDataTable<{typeof(T).Name}>.{nameof(LoadData)}: Could not properly deserialize {typeof(T).FullName}[] from path '{path}'");
                return Array.Empty<T>();
            }

            return infos;
        }

        public virtual bool SaveData(ICollection<T> dataObjs)
        {
            string path = _filePath;
            if (string.IsNullOrWhiteSpace(path) || dataObjs == null)
                return false;

            Serialize(path, dataObjs.ToArray());
            PLog.Trace<VortexLogger>($"Data {typeof(T).Name} saved to '{path}' (count: {dataObjs.Count})");
            
            return true;
        }
        
        protected abstract bool TryDeserialize(string path, out T[] dataObjs);
        protected abstract bool Serialize(string path, T[] dataObjs);
    }
}