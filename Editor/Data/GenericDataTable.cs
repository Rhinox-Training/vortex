using System;
using System.Collections.Generic;
using System.Reflection;

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
        private MethodInfo _setIDMethod;
        private MethodInfo _getNewIDMethod;
        private MethodInfo _storeMethod;
        private MethodInfo _hasDataMethod;
        private MethodInfo _getDataMethod;
        private MethodInfo _removeDataMethod;

		public delegate void DataChangedEventHandler(GenericDataTable sender);
		public event DataChangedEventHandler DataChanged;

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
                    _setIDMethod = null;
                    _storeMethod = null;
                    _hasDataMethod = null;
                    _getDataMethod = null;
                    _removeDataMethod = null;
                    _tableType = null;
                }

                _loaded = false;
                return;
            }

            _tableType = typeof(DataTable<>).MakeGenericType(_selectedType);
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            _getIDsMethod = _tableType.GetMethod("GetIDs", flags);
            _getIDMethod = _tableType.GetMethod("GetID", flags);
            _setIDMethod = _tableType.GetMethod("SetID", flags);
            _getNewIDMethod = _tableType.GetMethod("GetNewID", flags);
            _storeMethod = _tableType.GetMethod("StoreData", flags);
            _hasDataMethod = _tableType.GetMethod("HasData", flags);
            _getDataMethod = _tableType.GetMethod("GetData", flags);
            _removeDataMethod = _tableType.GetMethod("RemoveData", flags);
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
        
        public object SetID(object dataObject, int id)
        {
            if (!_loaded)
                return -1;
            return (object) _setIDMethod.Invoke(_tableInstance, new[] {dataObject, id});
        }

        public int GetNewID()
        {
            if (!_loaded)
                return -1;
            return (int) _getNewIDMethod.Invoke(_tableInstance, null);
        }
        
        
        public bool HasData(int id)
        {
            if (!_loaded)
                return false;
            return (bool) _hasDataMethod.Invoke(_tableInstance, new object[] {id});
        }
        
        public bool StoreObject(object storeObject, bool overwrite = false)
        {
            if (!_loaded)
                return false;
            bool result = (bool)_storeMethod.Invoke(_tableInstance, new[] {storeObject, overwrite});
			if (result)
				DataChanged?.Invoke(this);
			return result;
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

        public bool RemoveData(int id)
        {
            if (!_loaded)
                return false;
            bool result = (bool)_removeDataMethod.Invoke(_tableInstance, new object[] {id});
			if (result)
				DataChanged?.Invoke(this);
			return result;
        }
    }
}