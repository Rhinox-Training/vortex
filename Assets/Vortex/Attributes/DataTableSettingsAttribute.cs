using System;

namespace Rhinox.Vortex
{
    public class DataTableSettingsAttribute : Attribute
    {
        public int LoadOrder = 0;
        
        public DataTableSettingsAttribute(int loadOrder = 0)
        {
            LoadOrder = loadOrder;
        }
    }
}