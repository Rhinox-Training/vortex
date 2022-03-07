using System.Reflection;
using Rhinox.Vortex.File;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rhinox.Vortex
{
    public class VortexSettings : ProjectSettingsProvider<VortexSettings>
    {
        [DisableInPlayMode, SerializeReference]
        public EndPointConfiguration Configuration;
        
        protected override void Init()
        {
            Configuration = new FileEndPointConfig();
        }
        
#if UNITY_EDITOR
        [SettingsProvider]
        public static SettingsProvider RegisterProvider() => Instance.CreateSettingsProvider();
#endif
    }
}