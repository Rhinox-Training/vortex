using System;
using Rhinox.GUIUtils.Attributes;

namespace Rhinox.Vortex
{
    [Serializable, AssignableTypeFilter]
    public abstract class EndPointConfiguration
    {
        public abstract DataEndPoint CreateEndPoint();
    }
}