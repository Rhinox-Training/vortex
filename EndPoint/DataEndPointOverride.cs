using System;
using Rhinox.GUIUtils.Attributes;
using Rhinox.Perceptor;
using Rhinox.Utilities.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Rhinox.Vortex
{
    [ExecutionOrder(-8000)]
    public class DataEndPointOverride : MonoBehaviour
    {
        [DisableInPlayMode, SerializeReference]
        public DataLayerConfig Override;

        [ShowReadOnly] private bool _activated;

        private void Awake()
        {
            if (!_activated)
                Activate();
        }

        public bool Activate()
        {
            if (_activated) return true;

            if (Override == null)
                return false;
            
            _activated = true;

            PLog.Info<VortexLogger>($"Initializing Endpoint Override {Override.name}");
            DataLayer.PushEndPointFromConfig(Override);

            return true;
        }

        private void OnDestroy()
        {
            if (_activated)
                DataLayer.PopEndPoint();
        }
    }
}