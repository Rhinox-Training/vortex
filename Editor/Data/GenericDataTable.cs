using System;
using System.Collections.Generic;
using System.Reflection;
using Rhinox.VOLT.Data;

namespace Rhinox.Vortex.Editor
{
    public class GenericDataTable
    {
        private DataEndPoint _endPoint;
        public DataEndPoint EndPoint => _endPoint;
        private Type _selectedType;
        public Type DataType => _selectedType;
        public string TableTypeName => _selectedType?.Name;
        private bool _loaded;

        // Calculated
        private Type _tableType;
        private IDataTable _tableInstance;
        private MethodInfo _getIDsMethod;
        private MethodInfo _getIDMethod;
        private MethodInfo _storeMethod;
        private MethodInfo _getDataMethod;
        private MethodInfo _removeDataMethod;

        public GenericDataTable(DataEndPoint endPoint, Type t = null)
        {
            if (endPoint == null) throw new ArgumentException(nameof(endPoint));
            _endPoint = endPoint;
            _selectedType = t;

            RefreshTableCache();
        }

        public void SetType(Type t)
        {
            if (_selectedType == t)
                return;
            _selectedType = t;
            RefreshTableCache();
        }

        private void RefreshTableCache()
        {
            if (_selectedType == null)
            {
                if (_loaded) // Unload
                {
                    _tableInstance = null;
                    _getIDsMethod = null;
                    _getIDMethod = null;
                    _storeMethod = null;
                    _getDataMethod = null;
                    _removeDataMethod = null;
                    _tableType = null;
                }

                _loaded = false;
                return;
            }

            _tableType = typeof(DataTable<>).MakeGenericType(_selectedType);
            _getIDsMethod = _tableType.GetMethod("GetIDs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            _getIDMethod = _tableType.GetMethod("GetID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            _storeMethod = _tableType.GetMethod("StoreData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            _getDataMethod = _tableType.GetMethod("GetData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            _removeDataMethod = _tableType.GetMethod("RemoveData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            _tableInstance = _endPoint.GetTable(_selectedType);
            
            _loaded = true;
        }

        public int ElementCount
        {
            get
            {
                if (!_loaded)
                    return -1;
                int count = GetIDs().Count;
                return count;
            }
        }
        
        
        public int GetID(object dataObject)
        {
            if (!_loaded)
                return -1;
            return (int)_getIDMethod.Invoke(_tableInstance, new[] {dataObject});
        }
        
        public bool StoreObject(object storeObject, bool overwrite = false)
        {
            if (!_loaded)
                return false;
            return (bool)_storeMethod.Invoke(_tableInstance, new[] {storeObject, overwrite});
        }

        public object GetStoredObject(int key)
        {
            if (!_loaded)
                return null;
            return _getDataMethod.Invoke(_tableInstance, new object[] {key});
        }

        public ICollection<int> GetIDs()
        {
            if (!_loaded)
                return Array.Empty<int>();
            return ((ICollection<int>) _getIDsMethod?.Invoke(_tableInstance, null)) ?? Array.Empty<int>();
        }

        public void RemoveData(int id)
        {
            if (!_loaded)
                return;
            _removeDataMethod.Invoke(_tableInstance, new object[] {id});
        }
    }
}