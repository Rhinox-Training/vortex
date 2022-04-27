using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rhinox.Lightspeed.IO;
using Rhinox.Vortex.File;
using UnityEngine;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rhinox.Vortex
{
    public abstract class ProjectSettingsProvider<T> : ScriptableObject
        where T : ProjectSettingsProvider<T>
    {
        private static T _instance;
        public static T Instance => _instance ?? (_instance = GetOrCreateSettings());

        protected static string GetSettingsPath()
        {
            return $"Assets/Editor/{typeof(T).Name}.asset";
        }
        
        protected static T GetOrCreateSettings()
        {
            var assetPath = GetSettingsPath();
            var settings = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            
            if (settings == null)
            {
                settings = CreateInstance<T>();
                settings.Init();

                var directory = Path.GetDirectoryName(assetPath);
                FileHelper.CreateDirectoryIfNotExists(directory);

                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        protected abstract void Init();
        
#if UNITY_EDITOR && ODIN_INSPECTOR
        /// <summary>
        /// Add the following to your implementation
        /// #if UNITY_EDITOR
        /// [SettingsProvider]
        /// public static SettingsProvider RegisterProvider() => Instance.CreateSettingsProvider();
        /// #endif
        /// Reason: This needs to be static and cannot be done with inheritance
        /// </summary>
        
        private static PropertyTree _tree;
        
        protected SettingsProvider CreateSettingsProvider()
        {
            _tree = PropertyTree.Create(GetOrCreateSettings());

            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/VortexSettings", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Vortex",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    _tree.Draw(true);
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = GetSearchKeywordsFromPropertyTree(_tree)
            };

            return provider;
        }
        
        public static IEnumerable<string> GetSearchKeywordsFromPropertyTree(PropertyTree tree)
        {
            if (tree == null)
                return Array.Empty<string>();
            
            return tree.EnumerateTree(true)
                .Select(x => x.Name.ToLowerInvariant())
                .Distinct();
        }
#endif
    }
}