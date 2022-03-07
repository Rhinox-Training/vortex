using System;
using System.Collections.Generic;
using System.Linq;
using Rhinox.GUIUtils.Odin;
using Rhinox.Lightspeed;
using Rhinox.Perceptor;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Rhinox.Vortex
{
    public static class DataLayer // TODO: scene transition and singleton management
    {
        [ShowInPlayMode, HideReferenceObjectPicker, HideLabel]
        private static DataEndPoint _endPoint;

        private static Stack<DataEndPoint> _endPointStack;

        public static DataEndPoint DefaultInit(EndPointConfiguration defaultConfiguration)
        {
            if (defaultConfiguration == null)
            {
                PLog.Warn<VortexLogger>($"DataLayer was not initialized, DataLayerConfig failed to load...");
                return null;
            }

            var endpoint = defaultConfiguration.CreateEndPoint();
            PushEndPoint(endpoint);

            if (_endPoint == null)
            {
                PLog.Error<VortexLogger>(
                    $"DataLayer failed to initialize, no endpoint was created with the current configuration. Check DataLayerConfig and try again.");
                return null;
            }

            _endPoint.Initialize();

            return _endPoint;
        }

        public static IDataTable<T> GetTable<T>() => _endPoint.GetTable<T>();

        public static void PushEndPoint(DataEndPoint endPoint)
        {
            if (_endPointStack == null)
                _endPointStack = new Stack<DataEndPoint>();
            _endPointStack.Push(endPoint);
            _endPoint = endPoint;
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
    }
}


/*
#if UNITY_EDITOR
    public class EditorDataLayer
    {
        [InitializeOnLoadMethod]
        private static void OnUnityEditorLoad()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            TryInitialize();
            if (obj == PlayModeStateChange.EnteredEditMode)
            {
                _instance.Initialize();
            }
            else if (obj == PlayModeStateChange.ExitingEditMode)
            {
                _instance._endPoints.Clear();
                _instance._endPointOverrides.Clear();
            }
        }

        private static EditorDataLayer _instance;

        private static void TryInitialize()
        {
            if (_instance != null)
                return;
            
            _instance = new EditorDataLayer();
            _instance.Initialize();

        }

        private readonly Stack<DataEndPoint> _endPoints;
        private readonly Dictionary<DataLayerConfig, DataEndPoint> _endPointOverrides;

        public EditorDataLayer()
        {
            _endPoints = new Stack<DataEndPoint>();
            _endPointOverrides = new Dictionary<DataLayerConfig, DataEndPoint>();
        }
        
        private void Initialize()
        {
            if (!Application.isPlaying)
                LoadEditorEndpoint();
        }

        public static void PushEndPoint(DataLayerConfig config)
        {
            var endPoint = _instance.TryGetEndPoint(config);
            _instance._endPoints.Push(endPoint);
        }

        public static void PopEndPoint()
        {
            if (_instance._endPoints.Count <= 1)
            {
                PLog.Warn<VortexLogger>($"Tried to pop original EditorDatalayer Endpoint, this is not allowed...");
                return;
            }
            _instance._endPoints.Pop();
        }

        [Button("Reload Endpoint")]
        private void LoadEditorEndpoint()
        {
            if (_endPoints.Count > 0)
                return;
            
            if (DataLayerDefaults.Instance == null)
            {
                PLog.Warn<VortexLogger>($"DataLayer was not initialized, DataLayerConfig failed to load...");
                return;
            }

            var config = DataLayerDefaults.Instance.Configuration;
            if (config == null)
            {
                PLog.Error<VortexLogger>(
                    $"DataLayer was not initialized, DataLayerConfig doesn't contain a valid configuration. Check the Resources folder");
                return;
            }

            var endPoint = config.CreateEndPoint();
            if (endPoint == null)
            {
                PLog.Error<VortexLogger>(
                    $"DataLayer failed to initialize, no endpoint was created with the current configuration. Check DataLayerConfig and try again.");
                return;
            }

            endPoint.Initialize();
            _endPoints.Push(endPoint);
        }
        
        /// <summary>
        /// Returns false if multiple endpoints were found
        /// </summary>
        public static bool GetSceneEndPoint(out DataLayerConfig endPoint)
        {
            endPoint = null;
            
            var overrides = Object.FindObjectsOfType<DataEndPointOverride>();
            if (overrides.Length > 1)
            {
                PLog.Error<VortexLogger>("Cannot determine endpoint for datalayer; Too many overrides...");
                return false;
            }

            endPoint = overrides.FirstOrDefault()?.Override;
            return true;
        }
        
        public static IDataTable<T> GetTable<T>(DataLayerConfig overrideConfig = null)
        {
            TryInitialize();
            IDataTable table = null;
            if (!Application.isPlaying)
            {
                table = _instance.TryGetEndPoint(overrideConfig).GetTable<T>();
            }
            else
                table = DataLayer.GetTable<T>();

            return table as IDataTable<T>;
        }

        public static IReadOnlyDataTable<T> ReadTable<T>(DataLayerConfig overrideConfig = null, bool tryFindOverrideInScene = false)
        {
            TryInitialize();
            IDataTable table = null;
            if (!Application.isPlaying)
            {
                if (overrideConfig == null && tryFindOverrideInScene)
                {
                    var endPointOverride = Object.FindObjectOfType<DataEndPointOverride>();
                    if (endPointOverride != null)
                        overrideConfig = endPointOverride.Override;
                }

                var endpoint = _instance.TryGetEndPoint(overrideConfig);
                
                if (endpoint != null)
                    table = endpoint.GetTable<T>();
            }
            else
                table = DataLayer.GetTable<T>();

            return table as IReadOnlyDataTable<T>;
        }

        private DataEndPoint TryGetEndPoint(DataLayerConfig overrideConfig = null)
        {
            if (overrideConfig == null)
                return _endPoints.Peek();

            if (overrideConfig.Config == null)
            {
                Debug.LogWarning($"Override config {overrideConfig.name} is not configured properly.");
                return _endPoints.Peek();
            }
            
            if (_endPointOverrides.ContainsKey(overrideConfig))
                return _endPointOverrides[overrideConfig];

            
            var overrideEndpoint = overrideConfig.Config.CreateEndPoint();
            if (overrideEndpoint == null)
            {
                PLog.Error<VortexLogger>(
                    $"DataLayer override {overrideConfig.name} failed to initialize, no endpoint was created with the current configuration. Returning default endpoint.");
                return _endPoints.Peek();
            }
            overrideEndpoint.Initialize();
            _endPointOverrides.Add(overrideConfig, overrideEndpoint);
            return overrideEndpoint;
        }
        
        [MenuItem("Rhinox/Data Layer Actions/Purge Memory", priority = 200)]
        private static void PurgeDataLayer()
        {
            EditorDataLayer._instance = null;
        }
    }
#endif
}*/