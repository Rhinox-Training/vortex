using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhinox.GUIUtils.Attributes;
using Rhinox.Lightspeed.Reflection;
using Rhinox.Perceptor;
using Sirenix.OdinInspector;

namespace Rhinox.Vortex
{
    public abstract class DataEndPoint : IEquatable<DataEndPoint>
    {
        #if UNITY_EDITOR
        private string _EDITOR_endPointName => GetType().Name;
        
        [Title("@_EDITOR_endPointName")]
        [ShowInPlayMode, ListDrawerSettings(ShowPaging = true, NumberOfItemsPerPage = 1, IsReadOnly = true), LabelText("Tables")]
        private ICollection<IDataTable> _EDITOR_dataView => _dataTables.Values;
        #endif
        
        private readonly IDictionary<Type, IDataTable> _dataTables;

        protected DataEndPoint()
        {
            _dataTables = new Dictionary<Type, IDataTable>();
        }

        public ICollection<Type> GetTableTypes()
        {
            List<Type> types = new List<Type>();
            foreach (var type in AppDomain.CurrentDomain.GetDefinedTypesOfType<IDataTable>())
            {
                if (!type.InheritsFrom(typeof(IDataTable)))
                    continue;
                types.Add(type);
            }

            types = types.OrderBy(x => x.GetCustomAttribute<DataTableSettingsAttribute>()?.LoadOrder ?? 0).ToList();
            
            PLog.Info<VortexLogger>($"Initialized {GetType().Name} - Contains {types.Count} Tables:\n\t{string.Join("\n\t", types)}");

            return types;
        }

        public ICollection<Type> GetStoreableDataTypes()
        {
            List<Type> types = new List<Type>();
            foreach (Type t in GetTableTypes())
            {
                Type dataType = GetDataTypeFromTableType(t);
                if (dataType == null)
                    continue;

                types.Add(dataType);
            }

            return types;
        }

        private Type GetDataTypeFromTableType(Type tableType)
        {
            Type interfaceType = tableType.GetInterfaces().FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataTable<>));
            if (interfaceType == null)
                return null;

            return interfaceType.GetGenericArguments()[0];
        }

        public void Initialize()
        {
            foreach (var type in GetTableTypes())
            {
                Type dataType = GetDataTypeFromTableType(type);
                if (dataType == null)
                {
                    PLog.Warn<VortexLogger>($"Failed to register type {type.Name} for EndPoint {GetType().Name}. DataType not found.");
                    continue;
                }
                var instance = Activator.CreateInstance(type) as IDataTable;
                if (!Register(dataType, instance))
                    PLog.Warn<VortexLogger>($"DataLayer - Failed to register type {type.Name} for EndPoint {GetType().Name}");
                else
                    PLog.Info<VortexLogger>($"Registered {type.Name} table for EndPoint {GetType().Name} [Data: {dataType.Name}]");
            }
            
            OnInitialize();

            foreach (var table in _dataTables.Values)
            {
                if (!table.Initialize(this))
                    PLog.Error<VortexLogger>($"Failed to load table {table.GetType().Name}");
            }
        }

        protected virtual void OnInitialize()
        {
            
        }
        
        private bool Register(Type t, IDataTable dataTable)
        {
            if (_dataTables.ContainsKey(t))
                return false;
            
            _dataTables.Add(t, dataTable);
            return true;
        }

        protected bool Register<T>(IDataTable<T> dataTable)
        {
            Type key = typeof(T);
            if (_dataTables.ContainsKey(key))
                return false;
            
            _dataTables.Add(key, dataTable);
            return true;
        }
        
        protected bool Deregister<T>(IDataTable<T> dataTable)
        {
            return Deregister<T>();
        }
        
        protected bool Deregister<T>()
        {
            Type key = typeof(T);
            if (!_dataTables.ContainsKey(key))
                return false;
            
            return _dataTables.Remove(key);
        }

        // TODO: This API method can be used to provide cached versions of tables, for now just coupled to GetTable
        public IReadOnlyDataTable<T> ReadTable<T>() => GetTable<T>();
        
        public IDataTable<T> GetTable<T>()
        {
            Type key = typeof(T);
            if (_dataTables.ContainsKey(key))
                return _dataTables[key] as IDataTable<T>;
            
            PLog.Warn<VortexLogger>($"Requested table for '{typeof(T).Name}' but none was found in '{this.GetType().Name}'. Available Types:\n" +
                                       $"\t{string.Join("\n\t", _dataTables.Keys)}");
            return null;
        }

        public IDataTable GetTable(Type key)
        {
            if (!_dataTables.ContainsKey(key))
                return null;

            return _dataTables[key];
        }

        public void Refresh()
        {
            if (_dataTables == null)
                return;

            foreach (var table in _dataTables.Values)
            {
                if (table == null)
                    continue;
                table.RefreshDataCache();
                // TODO: Jorian suggested to work with invalidate instead so that only if it is needed, the data will be reloaded (lazy vs eager)
            }
        }

        public bool Equals(DataEndPoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CheckData(other);
        }

        protected virtual bool CheckData(DataEndPoint other)
        {
            return Equals(_dataTables, other._dataTables);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataEndPoint) obj);
        }

        public override int GetHashCode()
        {
            return (_dataTables != null ? _dataTables.GetHashCode() : 0);
        }

        public static bool operator ==(DataEndPoint left, DataEndPoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DataEndPoint left, DataEndPoint right)
        {
            return !Equals(left, right);
        }

        public abstract IDataTableSerializer<T> CreateSerializer<T>(string tableName);
    }
}