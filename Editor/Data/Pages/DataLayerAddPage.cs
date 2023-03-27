using System;
using Rhinox.GUIUtils.Attributes;
using Rhinox.GUIUtils.Editor.Helpers;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerAddPage : DataLayerBaseDataPage
    {
        [HideReferenceObjectPicker, HideLabel, NonSerialized, DrawAsReference, ShowInEditor, VerticalGroup]
        public object Data;

        public DataLayerAddPage(SlidePagedWindowNavigationHelper<object> pager, GenericDataTable dataTable) : base(pager, dataTable)
        {
            Data = Activator.CreateInstance(dataTable.DataType);
            var id = _dataTable.GetNewID();
            _dataTable.SetID(Data, id);
        }

        // Store
        [InfoBox("Primary Key already taken in DataLayer, choose a different key.", InfoMessageType.Error, nameof(PrimaryKeyTaken))]
        [Button, VerticalGroup, DisableIf(nameof(ShouldDisableStore))]
        private void StoreObject()
        {
            if (!StoreObject(Data))
                return;
            EditorApplication.delayCall += _pager.NavigateBack;
        }

        private bool ShouldDisableStore()
        {
            return Data == null || PrimaryKeyTaken();
        }

        private bool PrimaryKeyTaken()
        {
            if (Data == null)
                return false;
            
            return ContainsKey(Data);
        }
        
        private bool ContainsKey(object storeObject)
        {
            int key = _dataTable.GetID(storeObject); // Fetch ID of new object (not in datalayer)
            return _dataTable.HasData(key); // Check if ID is available in table
        }

        private bool StoreObject(object storeObject)
        {
            return _dataTable.StoreObject(storeObject, false);
        }
        
        [Button, VerticalGroup]
        private void Back()
        {
            EditorApplication.delayCall += _pager.NavigateBack;
        }
    }
}