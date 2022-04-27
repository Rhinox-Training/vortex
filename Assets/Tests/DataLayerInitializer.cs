using System.Collections;
using System.Collections.Generic;
using Rhinox.Perceptor;
using Rhinox.Vortex;
using UnityEngine;

public static class DataLayerInitializer
{
    [RuntimeInitializeOnLoadMethod]
    private static void InitDataLayer()
    {
        if (DataLayer.IsInitialized)
            PLog.Warn<VortexLogger>("DataLayer was already initialized... Spill from editor mode?");
        
        DataLayer.DefaultInit(VortexSettings.Instance.Configuration);
    }
}
