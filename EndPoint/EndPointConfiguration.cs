using System;
using Rhinox.GUIUtils.Odin;

namespace Rhinox.Vortex
{
    [Serializable, AssignableTypeFilter]
    public abstract class EndPointConfiguration
    {
        public abstract DataEndPoint CreateEndPoint();
    }
}