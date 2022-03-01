using System;
using Rhinox.OdinInspector.Attributes;

namespace Rhinox.Vortex
{
    [Serializable, AssignableTypeFilter]
    public abstract class EndPointConfiguration
    {
        public abstract DataEndPoint CreateEndPoint();
    }
}