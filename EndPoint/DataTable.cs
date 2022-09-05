using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rhinox.GUIUtils.Attributes;
using Rhinox.Perceptor;
using Sirenix.OdinInspector;

namespace Rhinox.Vortex
{
    public interface IDataTable
    {
        bool Initialize(DataEndPoint endPoint);

        void FlushData();
    }

    public interface IReadOnlyDataTable<T> : IReadOnlyCollection<T>
    {
        // READ
        ICollection<int> GetIDs();

        int GetNewID();

        bool HasData(int id);
        
        T GetData(int id);

        ICollection<T> GetAllData();
    }
    
    public interface IDataTable<T> : IDataTable, IReadOnlyDataTable<T>
    {
        // CREATE / UPDATE
        bool StoreData(T dataObj, bool allowOverwrite = true);
        
        // DELETE
        bool RemoveData(int id);
    }

    // [ValueTypeAsTitle]
    [Title("@$property.ValueEntry.TypeOfValue.Name")]
    public abstract class DataTable<T> : IDataTable<T>
    {
        #if UNITY_EDITOR
        private string _EDITOR_title => typeof(T).Name;
        #endif
        
        [Title("@_EDITOR_title")]
        [ShowReadOnlyInPlayMode, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, IsReadOnly = true)]
        protected IDictionary<int, T> _dataCacheByID;

        protected DataEndPoint _currentEndPoint;

        public virtual bool Initialize(DataEndPoint endPoint)
        {
            if (_currentEndPoint != null)
                return true;

            _currentEndPoint = endPoint;
            var dataObjs = LoadData(true).ToArray();

            for (int i = 0; i < dataObjs.Length; ++i)
            {
                T info = dataObjs[i];
                dataObjs[i] = OnLoad(info);
            }
            
            _dataCacheByID = dataObjs.ToDictionary(x => GetID(x));
            return true;
        }

        public virtual void FlushData()
        {
            
        }

        protected abstract int GetID(T dto);

        protected abstract ICollection<T> LoadData(bool createIfNotExists = false);

        protected abstract bool SaveData(ICollection<T> dataObjs);
        
        private bool CheckEndPointLoaded(bool forceRefresh = false)
        {
            return _dataCacheByID != null;
        }

        // CREATE / UPDATE
        public bool StoreData(T dto, bool allowOverwrite = true)
        {
            if (!CheckEndPointLoaded())
                return false;

            int id = GetID(dto);
            
            if (_dataCacheByID.ContainsKey(id) && !allowOverwrite)
                return false;

            if (_dataCacheByID.ContainsKey(id))
                _dataCacheByID[id] = dto;
            else
                _dataCacheByID.Add(id, dto);
            
            // We might have changed an id, check whether the cache is still valid
            foreach (var key in _dataCacheByID.Keys.ToArray())
            {
                if (GetID(_dataCacheByID[key]) != key)
                    _dataCacheByID.Remove(key);
            }
            
            OnSave(dto);
            
            if (!SaveData(_dataCacheByID.Values.Distinct().OrderBy(GetID).ToArray()))
                PLog.Warn<VortexLogger>($"Did not store cached data on store");
            
            return true;
        }
        
        protected virtual void OnSave(T dto) { }
        
        // READ
        public ICollection<int> GetIDs()
        {
            CheckEndPointLoaded();
            
            return _dataCacheByID.Keys;
        }

        public int GetNewID()
        {
            if (Count == 0)
                return 0;
            return _dataCacheByID.Keys.Max() + 1;
        }

        public bool HasData(int id)
        {
            CheckEndPointLoaded();
            return _dataCacheByID.ContainsKey(id);
        }

        public T GetData(int id)
        {
            CheckEndPointLoaded();

            if (_dataCacheByID.ContainsKey(id))
                return _dataCacheByID[id];
            
            PLog.Warn<VortexLogger>($"Did not find data with ID {id} in {GetType().Name}");
            return default(T);
        }

        public ICollection<T> GetAllData()
        {
            CheckEndPointLoaded();
            return _dataCacheByID.Values;
        }

        // DELETE
        public virtual bool RemoveData(int id)
        {
            if (!CheckEndPointLoaded())
                return false;

            if (!_dataCacheByID.ContainsKey(id))
            {
                PLog.Warn<VortexLogger>($"Did not find data to remove with ID {id} in {GetType().Name}");
                return false;
            }

            _dataCacheByID.Remove(id);
            
            if (!SaveData(_dataCacheByID.Values))
                PLog.Warn<VortexLogger>($"Did not store cached data on remove");
            return true;
        }

        protected virtual T OnLoad(T dto)
        {
            return dto;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dataCacheByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _dataCacheByID.Count;
    }
    
    
}