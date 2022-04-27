using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rhinox.Vortex
{
    //[ExecutionOrder(-8000)]
    public class DataEndPointOverride : MonoBehaviour
    {
        [DisableInPlayMode, SerializeReference]
        public DataLayerConfig Override;

        private void Awake()
        {
            if (Override != null)
                DataLayer.PushEndPoint(Override?.Config.CreateEndPoint());
        }

        private void OnDestroy()
        {
            if (Override != null)
                DataLayer.PopEndPoint();
        }
    }
}