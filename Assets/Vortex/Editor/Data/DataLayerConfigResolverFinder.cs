#if ODIN_INSPECTOR
using System;
using Rhinox.GUIUtils.Odin.Editor;
using Rhinox.Lightspeed.Reflection;
using Sirenix.OdinInspector.Editor;

namespace Rhinox.Vortex.Editor
{
    public static class DataLayerConfigResolverFinder
    {
        public static DataLayerConfig FindDataLayerResolver(this InspectorProperty searchProperty)
        {
            DataLayerConfigResolverAttribute processorAttribute = null;
            Type searchType = searchProperty.Info.TypeOfValue;

            while (searchProperty != null)
            {
                processorAttribute = searchType.GetCustomAttribute<DataLayerConfigResolverAttribute>();
                if (processorAttribute != null)
                    break;

                if (searchProperty.Parent == null)
                    break;
                searchType = searchProperty.ParentType;
                searchProperty = searchProperty.Parent;
            }

            if (processorAttribute == null)
                return null;

            var propertyHelper =
                new PropertyMemberHelper<DataLayerConfig>(searchProperty, processorAttribute.MemberName);
            var config = propertyHelper.GetValue();
            return config;
        }
    }
}
#endif