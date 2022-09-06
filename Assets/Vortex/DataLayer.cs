using System;
using System.Collections.Generic;
using System.Linq;
using Rhinox.GUIUtils.Attributes;
using Rhinox.Lightspeed;
using Rhinox.Perceptor;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace Rhinox.Vortex
{
    public static class DataLayer // TODO: scene transition and singleton management
    {
        private static bool _initialized;

        public static bool IsInitialized => _initialized;

        [ShowInPlayMode, HideReferenceObjectPicker, HideLabel]
        private static DataEndPoint _endPoint;

        private static Stack<DataEndPoint> _endPointStack;
        private static Dictionary<DataLayerConfig, DataEndPoint> _endPointOverrides;
        private static DataEndPoint _defaultEndPoint;

        public static DataEndPoint DefaultInit(EndPointConfiguration defaultConfiguration)
        {
            if (_defaultEndPoint != null)
            {
                // TODO: check if defaultConfig settings have changed + should we log a warning?
                return _defaultEndPoint;
            }

            if (defaultConfiguration == null)
            {
                PLog.Warn<VortexLogger>($"DataLayer was not initialized, DataLayerConfig failed to load...");
                return null;
            }

            _defaultEndPoint = defaultConfiguration.CreateEndPoint();
            if (_defaultEndPoint == null)
            {
                PLog.Error<VortexLogger>(
                    $"DataLayer failed to initialize, no endpoint was created with the current configuration. Check DataLayerConfig and try again.");
                return null;
            }

            _defaultEndPoint.Initialize();

            _initialized = true;

            _endPointOverrides = new Dictionary<DataLayerConfig, DataEndPoint>();
            _endPointStack = new Stack<DataEndPoint>();
            PushEndPoint(_defaultEndPoint);

            var overrider = Object.FindObjectOfType<DataEndPointOverride>();
            if (overrider)
                overrider.Activate();

            return _endPoint;
        }

        public static void Shutdown()
        {
            if (!_initialized)
                return;

            _endPoint = null;
            _endPointStack.Clear();

            _initialized = false;
        }

        public static IDataTable<T> GetTable<T>()
        {
            if (!_initialized) // TODO: do we want this? if not what about editor?
                DefaultInit(VortexSettings.Instance.Configuration);

            return _endPoint.GetTable<T>();
        }

        public static IReadOnlyDataTable<T> ReadTable<T>()
        {
            if (!_initialized) // TODO: do we want this? if not what about editor?
                DefaultInit(VortexSettings.Instance.Configuration);

            return _endPoint.ReadTable<T>();
        }

        public static void PushEndPointFromConfigOrDefault(DataLayerConfig config = null)
        {
            if (config != null)
                PushEndPointFromConfig(config);
            else
                PushDefaultEndPoint();
        }

        public static void PushEndPointFromSceneOrDefault()
        {
            DataLayerConfig config = null;
            if (!DataLayer.FindSceneEndPointConfig(out config))
                config = null;
            DataLayer.PushEndPointFromConfigOrDefault(config);
        }

        public static void PushEndPointFromConfig(DataLayerConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (!_initialized) // TODO: do we want this? if not what about editor?
                DefaultInit(VortexSettings.Instance.Configuration);

            DataEndPoint endPoint;
            if (!TryGetEndPoint(config, out endPoint))
            {
                PLog.Error<VortexLogger>($"Could not find/create EndPoint for config {config.name}, pushing default endpoint...");
                endPoint = _defaultEndPoint;
            }

            _endPointStack.Push(endPoint);
            _endPoint = endPoint;
        }

        public static void PushEndPoint(DataEndPoint endPoint)
        {
            if (!_initialized) // TODO: do we want this? if not what about editor?
                DefaultInit(VortexSettings.Instance.Configuration);

            _endPointStack.Push(endPoint);
            _endPoint = endPoint;
        }

        public static void PushDefaultEndPoint()
        {
            if (!_initialized) // TODO: do we want this? if not what about editor?
                DefaultInit(VortexSettings.Instance.Configuration);

            _endPointStack.Push(_defaultEndPoint);
            _endPoint = _defaultEndPoint;
        }

        public static DataEndPoint PopEndPoint()
        {
            if (_endPointStack.IsNullOrEmpty())
            {
                PLog.Error<VortexLogger>("PopEndPoint could not be executed, are you sure you properly pushed an endpoint?");
                return null;
            }

            _endPointStack.Pop();
            _endPoint = _endPointStack.Peek();
            return _endPoint;
        }

        /// <summary>
        /// Returns false if multiple endpoints were found
        /// </summary>
        public static bool FindSceneEndPointConfig(out DataLayerConfig endPoint)
        {
            endPoint = null;

            var overrides = UnityEngine.Object.FindObjectsOfType<DataEndPointOverride>();
            if (overrides.Length > 1)
            {
                PLog.Error<VortexLogger>("Cannot determine endpoint for datalayer; Too many overrides...");
                return false;
            }

            endPoint = overrides.FirstOrDefault()?.Override;
            return true;
        }

        private static bool TryGetEndPoint(DataLayerConfig overrideConfig, out DataEndPoint endPoint)
        {
            if (overrideConfig == null)
            {
                endPoint = _defaultEndPoint;
                return false;
            }

            if (overrideConfig.Config == null)
            {
                PLog.Warn<VortexLogger>($"Override config {overrideConfig.name} is not configured properly, using default instead...");
                endPoint = _defaultEndPoint;
                return false;
            }

            if (_endPointOverrides.ContainsKey(overrideConfig))
            {
                endPoint = _endPointOverrides[overrideConfig];
                return true;
            }

            var overrideEndpoint = overrideConfig.Config.CreateEndPoint();
            if (overrideEndpoint == null)
            {
                PLog.Error<VortexLogger>(
                    $"DataLayer override {overrideConfig.name} failed to initialize, no endpoint was created with the current configuration. Returning default endpoint.");
                endPoint = _defaultEndPoint;
                return false;
            }

            overrideEndpoint.Initialize();
            _endPointOverrides.Add(overrideConfig, overrideEndpoint);
            endPoint = overrideEndpoint;
            return true;
        }
    }
}