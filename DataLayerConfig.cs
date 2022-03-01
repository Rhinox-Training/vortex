using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Rhinox.Vortex
{
    public class DataLayerConfig : ScriptableObject
    {
        [DisableInPlayMode, SerializeReference]
        public EndPointConfiguration Config;
    }

    public static class DataLayerHelper
    {
        public static ICollection<DataLayerConfig> FindOverrides()
        {
            List<DataLayerConfig> configs = new List<DataLayerConfig>();
#if UNITY_EDITOR
            AssetDatabase.Refresh();
            var assetGuids = AssetDatabase.FindAssets($"t:{nameof(DataLayerConfig)}");
            if (assetGuids != null)
            {
                foreach (var assetGuid in assetGuids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                    configs.Add(AssetDatabase.LoadAssetAtPath<DataLayerConfig>(assetPath));
                }
            }
#else
            configs.AddRange(Resources.FindObjectsOfTypeAll<DataLayerConfig>()); // NOTE: old way, not always loaded (until seen)
#endif
            return configs.Distinct().ToList();
        }
        
        public static ValueDropdownList<DataLayerConfig> GetConfigList(bool includeDefault = true)
        {
            var options = new ValueDropdownList<DataLayerConfig>();
            
            if (includeDefault)
                options.Add("<<Default>>", null);

            foreach (var config in FindOverrides())
                options.Add(config.name, config);

            return options;
        }
    }
}