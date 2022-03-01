using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rhinox.Lightspeed.IO;
using Rhinox.Perceptor;
using UnityEngine;

namespace Rhinox.Vortex.File
{
    public abstract class FileDataTable<T> : DataTable<T>
    {
        protected abstract string _tableName { get; }
        private string _filePath;

        public override bool Initialize(DataEndPoint endPoint)
        {
            FileEndPoint fileEndPoint = (FileEndPoint) endPoint;
            if (!FileHelper.IsPathRooted(fileEndPoint.Path))
            {
                PLog.Error<VortexLogger>(
                    $"Something went wrong, FileEndPoint<{typeof(T).Name}> was initialized with an invalidPath. Path: {fileEndPoint.Path} should be rooted.");
                return false;
            }

            _filePath = Path.Combine(fileEndPoint.Path, $"{_tableName}.datatable");
            
            // Initialize needs to be called last (initializes tables, might load data)
            return base.Initialize(endPoint);
        }

        protected override ICollection<T> LoadData(bool createIfNotExists = false)
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

        protected override bool SaveData(ICollection<T> dataObjs)
        {
            string path = _filePath;
            if (string.IsNullOrWhiteSpace(path))
                return false;

            Serialize(path, dataObjs != null ? dataObjs.ToArray() : Array.Empty<T>());
            PLog.Trace<VortexLogger>($"Data {typeof(T).Name} saved to '{path}' (count: {dataObjs.Count})");
            
            return true;
        }
        
        protected abstract bool TryDeserialize(string path, out T[] infos);
        protected abstract bool Serialize(string path, T[] infos);
    }
}