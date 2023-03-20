using Rhinox.GUIUtils.Editor.Helpers;
using Rhinox.GUIUtils.Odin.Editor;
using UnityEngine;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerBaseDataPage : PagerPage
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