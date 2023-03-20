using System;
using System.Reflection;
using Rhinox.GUIUtils.Attributes;
using Rhinox.Utilities;
using Rhinox.Vortex.File;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rhinox.Vortex
{
    public class VortexSettings : ConfigFile<VortexSettings>
    {
        [DisableInPlayMode, SerializeReference, DrawAsReference]
        public EndPointConfiguration Configuration;
    }
}