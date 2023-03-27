using System;
using System.Collections.Generic;
using Rhinox.GUIUtils.Attributes;
using Rhinox.GUIUtils.Editor.Helpers;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Rhinox.Vortex.Editor
{
    public struct ObjectEntry
    {
        [ReadOnly, HideLabel, InlineIconButton("Pen", nameof(EditEntry), Tooltip = "Edit Entry", ForceEnable = true)]
        public object Object;

        private SlidePagedWindowNavigationHelper<object> _pager;
        private GenericDataTable _dataTable;
        private Action _onEditEntry;

        public ObjectEntry(object obj, SlidePagedWindowNavigationHelper<object> pager, GenericDataTable dataTable, Action onEditEntry = null)
        {
            Object = obj;
            _pager = pager;
            _dataTable = dataTable;
            _onEditEntry = onEditEntry;
        }
        
        private void EditEntry()
        {
            _pager.PushPage(new DataLayerEditDataPage(_pager, _dataTable, Object, _onEditEntry), "Edit Database Entry");
        }
    }
    public class DataLayerViewPage : DataLayerBaseDataPage
    {
        private const string GROUP_FIX = "DoNotRemove_FixesLayout";
        [ShowInInspector, ListDrawerSettings(Expanded = true, DraggableItems = false, IsReadOnly = true, ShowPaging = true, NumberOfItemsPerPage = 8), VerticalGroup(GROUP_FIX)]
        public List<ObjectEntry> Objects;
        
        public DataLayerViewPage(SlidePagedWindowNavigationHelper<object> pager, GenericDataTable dataTable) : base(pager, dataTable)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            Objects = new List<ObjectEntry>();
            foreach (int id in _dataTable.GetIDs())
            {
                var dataObj = _dataTable.GetStoredObject(id);
                ObjectEntry oe = new ObjectEntry(dataObj, _pager, _dataTable, () =>
                {
                    EditorApplication.delayCall += RefreshList;
                });
                Objects.Add(oe);
            }
        }
        
        [Button, VerticalGroup(GROUP_FIX)]
        private void Back()
        {
            EditorApplication.delayCall += _pager.NavigateBack;
        }
    }
}