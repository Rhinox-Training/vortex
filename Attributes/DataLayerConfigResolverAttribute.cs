using System;

namespace Rhinox.VOLT.Data
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