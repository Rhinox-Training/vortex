using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhinox.VOLT.Data;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerRemovePage : DataLayerBaseDataPage
    {
        [Title("Remove by ID")]
        [ShowInInspector, ValueDropdown(nameof(GetIDValues)), OnValueChanged(nameof(IDChanged)), VerticalGroup("Yes")]
        public int IDToRemove = -1;

        [NonSerialized, ReadOnly, HideLabel, ShowInInspector, VerticalGroup("Yes")]
        public object Element = null;
        
        private enum RemovalCheckType
        {
            Equals,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual
        }

        [ShowInInspector, SerializeField, HideLabel, VerticalGroup("Yes")] 
        private RemovalCheckType _checkType = RemovalCheckType.Equals;

        private bool _validID => IDToRemove != -1;

        private List<int> _ids;
        
        private List<ValueDropdownItem> GetIDValues()
        {
            return _ids.Select(x => new ValueDropdownItem(x.ToString(), x)).ToList();
        }

        private void IDChanged()
        {
            if (IDToRemove == -1)
            {
                Element = null;
                return;
            }

            Element = _dataTable.GetStoredObject(IDToRemove);
        }

        public DataLayerRemovePage(SlidePagedWindowNavigationHelper<object> pager, GenericDataTable dataTable) : base(pager, dataTable)
        {
            _ids = _dataTable.GetIDs().ToList();
        }

        [Button, EnableIf("@_validID"), VerticalGroup("Yes")]
        private void Remove()
        {
            if (!EditorUtility.DisplayDialog("Confirm", 
                $"Are you sure you want to remove entry {IDToRemove} by {_checkType.ToString()} from the DataLayer?)",
                "Confirm", "Cancel"))
                return;

            var idsToRemove = FindIDs(IDToRemove, _checkType);
            foreach (int id in idsToRemove)
            {
                _dataTable.RemoveData(id);
                _ids.Remove(id);
            }
            RefreshValueDropdown();
            Element = null;
            IDToRemove = -1;
        }

        private ICollection<int> FindIDs(int idToRemove, RemovalCheckType checkType)
        {
            Func<int, bool> selector;
            switch (checkType)
            {
                case RemovalCheckType.Equals:
                    selector = x => x == idToRemove;
                    break;
                case RemovalCheckType.LessThan:
                    selector = x => x < idToRemove;
                    break;
                case RemovalCheckType.LessThanOrEqual:
                    selector = x => x <= idToRemove;
                    break;
                case RemovalCheckType.GreaterThan:
                    selector = x => x > idToRemove;
                    break;
                case RemovalCheckType.GreaterThanOrEqual:
                    selector = x => x >= idToRemove;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(checkType), checkType, null);
            }

            return _ids.Where(selector).ToList();
        }


        [Title("Other"), Button, VerticalGroup("Yes")]
        private void RemoveAll()
        {
            if (!EditorUtility.DisplayDialog("Confirm", 
                $"Are you sure you want to remove all entries of {_dataTable.TableTypeName} from the DataLayer?)",
                "Confirm", "Cancel"))
            return;

            var idsToRemove = _ids.ToList();
            foreach (var id in idsToRemove)
            {
                _dataTable.RemoveData(id);
                _ids.Remove(id);
            }
            RefreshValueDropdown();
            Element = null;
            IDToRemove = -1;
        }

        [PropertySpace, Button, VerticalGroup("Yes")]
        private void Back()
        {
            EditorApplication.delayCall += _pager.NavigateBack;
        }

        // Yes, not even kidding...
        private void RefreshValueDropdown()
        {
            var old = Selection.objects;
            Selection.objects = null;
            Selection.objects = old;
            _checkType = RemovalCheckType.Equals;
        }
    }
}