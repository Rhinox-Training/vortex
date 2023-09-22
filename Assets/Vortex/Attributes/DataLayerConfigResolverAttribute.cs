using System;

namespace Rhinox.Vortex
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataLayerConfigResolverAttribute : Attribute
    {
        public string MemberName { get; }

        public DataLayerConfigResolverAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}