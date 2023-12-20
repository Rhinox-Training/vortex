using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhinox.GUIUtils.Editor;
using Rhinox.GUIUtils.Editor.Helpers;
using Sirenix.OdinInspector;
using Rhinox.Lightspeed.IO;
using Rhinox.Perceptor;
using Rhinox.Vortex.File;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;
#endif

namespace Rhinox.Vortex.Editor
{
    public class DataLayerMainPage : PagerPage
    {
        [ValueDropdown(nameof(GetDataLayerOverrides)), OnValueChanged(nameof(OnDataLayerOptionChanged)), InlineButton(nameof(SetDefaultConfig), "Default")]
        public DataLayerConfig OverrideConfig;

        [ValueDropdown(nameof(GetDataTables)), OnValueChanged(nameof(UpdateType)), NonSerialized, ShowInInspector, VerticalGroup]
        public Type Type;
        
        private bool ValidType => Type != null;

        private ICollection<Type> _dataTableTypes;
        private DataEndPoint _selectedEndPoint;
        private EndPointConfiguration _cachedConfig;
        private GenericDataTable _genericTable;

        protected EndPointConfiguration Configuration
        {
            get
            {
                // TODO: move to DataLayerDefaults.Instance.Configuration; (File)
                return OverrideConfig != null ? OverrideConfig.Config : VortexSettings.Instance.Configuration;
            }
        }

        private ValueDropdownList<DataLayerConfig> GetDataLayerOverrides() => DataLayerHelper.GetConfigList(false);

        public DataLayerMainPage(SlidePageNavigationHelper<object> pager) 
            : base(pager) { }

        public void Initialize()
        {
            OnDataLayerOptionChanged(); // TODO: check order of execution, to see if we can change this to _selectedEndPoint
        }
        
        [PropertySpace(10)]
        [Button(ButtonSizes.Large), EnableIf("ValidType"), VerticalGroup]
        private void AddEntry()
        {
            _pager.PushPage(new DataLayerAddPage(_pager, _genericTable), "Add Database Entry");
        }
        
        [Button("$ViewEntriesName", ButtonSizes.Large), EnableIf("ValidType"), VerticalGroup]
        private void ViewEntries()
        {
            _pager.PushPage(new DataLayerViewPage(_pager, _genericTable), "View Database Entries");
        }
        
        [PropertySpace(10)]
        [Button(ButtonSizes.Large), EnableIf("ValidTypeAndNotEmpty"), VerticalGroup]
        private void RemoveEntry()
        {
            _pager.PushPage(new DataLayerRemovePage(_pager, _genericTable), "Remove Database Entry");
            
        }

        private ICollection<ValueDropdownItem> GetDataTables()
        {
            List<ValueDropdownItem> dropdownItems = new List<ValueDropdownItem>();
            foreach (var type in _dataTableTypes)
                dropdownItems.Add(new ValueDropdownItem(type.Name, type));
            return dropdownItems;
        }

        private string ViewEntriesName
        {
            get
            {
                if (!ValidType)
                    return "View Entries (0)";
                
                int count = _genericTable != null ? _genericTable.ElementCount : 0;
                return $"View Entries ({count})";
            }
        }

        private bool ValidTypeAndNotEmpty
        {
            get
            {
                if (!ValidType)
                    return false;
                return _genericTable != null ? _genericTable.GetIDs().Count > 0 : false;
            }
        }

        private void OnDataLayerOptionChanged()
        {
            InitializeDataLayer();
        }

        public void InitializeDataLayer(bool forceRefresh = false)
        {
            if (Configuration == _cachedConfig && !forceRefresh)
                return;

            if (Configuration != null)
            {
                _cachedConfig = Configuration;
                _selectedEndPoint = Configuration.CreateEndPoint();
                _selectedEndPoint.Initialize();
                _dataTableTypes = _selectedEndPoint.GetStoreableDataTypes();
                _genericTable = new GenericDataTable(_selectedEndPoint, Type);
            }
            else
            {
                _cachedConfig = null;
                _selectedEndPoint = null;
                _genericTable = null;
            }
        }

        private void UpdateType()
        {
            _genericTable.SetType(Type);
        }
        
        private void SetDefaultConfig()
        {
            OverrideConfig = null;
            OnDataLayerOptionChanged();
        }

#if ODIN_INSPECTOR
        // TODO: how to add these dynamically as an extension
#region LEGACY_SUPPORT
        [PropertySpace, Button(ButtonSizes.Large), LabelText("Import Legacy Data (.rxpi/.rxti/...) from File"), EnableIf("@ValidType"), VerticalGroup]
        private void ImportDataFromFile()
        {
            string path = EditorUtility.OpenFilePanel($"Import {Type.Name} from File", FileEndPointConfig.ROOT_PATH, "*");
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogWarning("No path to open, import cancelled.");
                return;
            }

            object fileData = null;
            try
            {
                var fileContent = FileHelper.ReadAllBytes(path);
                fileData = SerializationUtility.DeserializeValueWeak(fileContent, DataFormat.JSON);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                return;
            }

            IEnumerable fileEnumeration = fileData as IEnumerable;
            if (fileData == null || fileEnumeration == null)
            {
                PLog.Error<VortexLogger>($"Could not properly extract data from path '{path}'");
                return;
            }

            List<object> dataObjs = new List<object>();
            foreach (object o in fileEnumeration)
            {
                dataObjs.Add(o);
            }

            var endPoint = Configuration.CreateEndPoint();
            endPoint.Initialize();
            Debug.Log($"Loading {dataObjs.Count} objects from file at {path}");
            foreach (var data in dataObjs)
            {
                if (data == null)
                {
                    Debug.LogError("Failed to load data, instance object is null.");
                    continue;
                }

                int id = _genericTable.GetID(data);
                
                if (_genericTable.StoreObject(data, false))
                {
                    Debug.Log($"Written data with ID {id} ({data})");
                }
                else
                {
                    Debug.LogWarning($"Failed to write data with ID {id} ({data}). ID probably already exists");
                }
            }
        }
#endregion
#endif
    }
}