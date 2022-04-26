using System;
using Rhinox.GUIUtils.Attributes;
using Rhinox.GUIUtils.Odin;
using Rhinox.GUIUtils.Odin.Editor;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerAddPage : DataLayerBaseDataPage
    {
        [HideReferenceObjectPicker, NonSerialized, ShowInEditor, VerticalGroup("Yes")]
        public object Data;

        public DataLayerAddPage(SlidePagedWindowNavigationHelper<object> pager, GenericDataTable dataTable) : base(pager, dataTable)
        {
            Data = Activator.CreateInstance(dataTable.DataType);
            // Data.Id = _dataTable.GetNewID();
        }

        // Store
        [InfoBox("Primary Key already taken in DataLayer, choose a different key.", InfoMessageType.Error, nameof(PrimaryKeyTaken))]
        [Button, VerticalGroup("Yes"), DisableIf(nameof(ShouldDisableStore))]
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
            return _dataTable.GetStoredObject(key) != null; // Check if ID is available in table
        }

        private bool StoreObject(object storeObject)
        {
            return _dataTable.StoreObject(storeObject, false);
        }
        
        [Button, VerticalGroup("Yes")]
        private void Back()
        {
            EditorApplication.delayCall += _pager.NavigateBack;
        }
    }
}