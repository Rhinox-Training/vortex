using UnityEngine;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerBaseDataPage : OdinPagerPage
    {
        protected GenericDataTable _dataTable;

        public DataLayerBaseDataPage(SlidePagedWindowNavigationHelper<object> pager, GenericDataTable dataTable) : base(pager)
        {
            _dataTable = dataTable;
        }

        protected override void OnDrawBottom()
        {
            base.OnDrawBottom();
            GUILayout.FlexibleSpace();
        }
    }
}