using System;

namespace Rhinox.Vortex
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataEndPointAttribute : Attribute
    {
        public Type EndPointType { get; }
        public int LoadOrder = 0;
        
        public DataEndPointAttribute(Type t, int loadOrder = 0)
        {
            if (t == null || !typeof(DataEndPoint).IsAssignableFrom(t))
                throw new ArgumentException(nameof(t));
            EndPointType = t;
            LoadOrder = loadOrder;
        }
    }
}