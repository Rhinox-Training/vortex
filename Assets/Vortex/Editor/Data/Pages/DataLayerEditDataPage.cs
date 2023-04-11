using System;
using Rhinox.GUIUtils.Attributes;
using Rhinox.GUIUtils.Editor.Helpers;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerEditDataPage : DataLayerBaseDataPage
    {
        private const string GROUP_FIX = "DoNotRemove_FixesLayout";
        private const string BUTTON_GROUP_FIX = GROUP_FIX + "/Buttons";
        
        [HideReferenceObjectPicker, HideLabel, NonSerialized, ShowInEditor, VerticalGroup(GROUP_FIX)]
        public object Data;

        private Action _onEditEntry;

        public DataLayerEditDataPage(SlidePagedWindowNavigationHelper<object> pager, GenericDataTable dataTable, object data,
            Action onEditEntry = null) 
            : base(pager, dataTable)
        {
            Data = data;
            _onEditEntry = onEditEntry;
        }
        
        [Button(ButtonSizes.Large), VerticalGroup(GROUP_FIX), HorizontalGroup(BUTTON_GROUP_FIX)]
        public void Save()
        {
            bool storeSucceeded = StoreObject(Data);
            if (!storeSucceeded)
                EditorUtility.DisplayDialog("Notice",
                    "Something went wrong during saving this entry. The entry will remain unchanged!", "Confirm");
            EditorApplication.delayCall += _pager.NavigateBack;
            _onEditEntry?.Invoke();
        }

        [Button(ButtonSizes.Large),VerticalGroup(GROUP_FIX), HorizontalGroup(BUTTON_GROUP_FIX)]
        public void Cancel()
        {
            EditorApplication.delayCall += _pager.NavigateBack;
        }
        
        private bool StoreObject(object storeObject)
        {
            return _dataTable.StoreObject(storeObject, true);
        }
    }
}