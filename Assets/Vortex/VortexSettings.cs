using System;
using System.Reflection;
using Rhinox.Utilities;
using Rhinox.Vortex.File;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rhinox.Vortex
{
    public class VortexSettings : ConfigFile<VortexSettings>
    {
        [DisableInPlayMode, SerializeReference]
        public EndPointConfiguration Configuration;
    }
}