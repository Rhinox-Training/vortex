using System.Net;
using Rhinox.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rhinox.Vortex
{
    public class DataLayerDefaults : ConfigFile<DataLayerDefaults>
    {
        public override string IniRelativePath => "data-layer.ini";
        
        [DisableInPlayMode, SerializeReference]
        public EndPointConfiguration Configuration;
    }
}
