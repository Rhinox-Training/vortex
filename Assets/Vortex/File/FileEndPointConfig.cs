using System;
using System.IO;
using Rhinox.Lightspeed;
using Rhinox.Lightspeed.IO;
using Rhinox.Perceptor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rhinox.Vortex.File
{
    public class FileEndPointConfig : EndPointConfiguration
    {
        public static string ROOT_PATH => Path.Combine(Application.streamingAssetsPath, "Data");
        
        private string _parentPath
        {
            get
            {
                string absPath = ROOT_PATH;
                if (!FileHelper.DirectoryExists(absPath))
                {
                    try
                    {
                        Directory.CreateDirectory(absPath);
                    }
                    catch (Exception e)
                    {
                        PLog.Error<VortexLogger>($"[_parentPath] Failed to create directory {absPath} \n since {e.ToString()}");
                    }
                }

                return absPath;
            }
        }
        
        [FolderPath(ParentFolder = "$_parentPath", RequireExistingPath = true)]
        public string BasePath = "";

        #if UNITY_EDITOR
        private string _infoBoxMessage => $"DataTables will be saved at: streamingAssetsPath/{Path.Combine("Data", BasePath, Namespace).Replace('\\', '/')}/";
        #endif    
        
        [Required, InfoBox("$_infoBoxMessage", InfoMessageType.None)]
        public string Namespace = "NewFileEndPoint";
        
        public override DataEndPoint CreateEndPoint()
        {
            if (string.IsNullOrWhiteSpace(Namespace))
                return null;
            PLog.Info<VortexLogger>($"Creating DataLayer endpoint from [{Namespace}::{BasePath}].");
            return new FileEndPoint(BasePath, Namespace);
        }
    }
}