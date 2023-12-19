using System;
using System.Reflection;
using Rhinox.GUIUtils.Attributes;
using Rhinox.Utilities;
using Rhinox.Utilities.Attributes;
using Rhinox.Vortex.File;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rhinox.Vortex
{
    [CustomProjectSettings(RuntimeSupported = true)]
    public class VortexSettings : CustomProjectSettings<VortexSettings>
    {
        [DisableInPlayMode, SerializeReference, DrawAsReference]
        public EndPointConfiguration Configuration;
    }
}