using System;
using System.IO;
using Rhinox.Lightspeed;
using Rhinox.Lightspeed.IO;
using Rhinox.Perceptor;
using UnityEngine;

namespace Rhinox.Vortex.File
{
    public class FileEndPoint : DataEndPoint
    {
        public string Path => _fullPathAbs;
        private string _fullPathAbs;
        private string _namespace;
        public FileEndPoint(string basePath, string nameSpace)
        {
            if (string.IsNullOrWhiteSpace(nameSpace))
                throw new ArgumentNullException(nameof(nameSpace));
            _fullPathAbs = System.IO.Path.Combine(FileEndPointConfig.ROOT_PATH, string.IsNullOrWhiteSpace(basePath) ? "" : basePath, nameSpace);
            _namespace = nameSpace;
            
            PLog.Info<VortexLogger>($"DataLayer FileEndPoint created: '{_fullPathAbs}'");

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (!FileHelper.DirectoryExists(_fullPathAbs))
            {
                try
                {
                    Directory.CreateDirectory(_fullPathAbs);
                }
                catch (Exception e)
                {
                    PLog.Error<VortexLogger>($"Failed to create directory {_fullPathAbs} \n since {e.ToString()}");
                }
            }
        }

        protected override bool CheckData(DataEndPoint other)
        {
            if (other is FileEndPoint otherFileEndPoint)
            {
                if (_fullPathAbs != null && _fullPathAbs.Equals(otherFileEndPoint._fullPathAbs))
                {
                    if (string.IsNullOrWhiteSpace(_namespace))
                        return string.IsNullOrWhiteSpace(otherFileEndPoint._namespace);
                    else
                        return _namespace.Equals(otherFileEndPoint._namespace);
                }

                return false;
            }
            return base.CheckData(other);
        }
    }
}