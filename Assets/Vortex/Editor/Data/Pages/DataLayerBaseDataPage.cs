using Rhinox.GUIUtils.Editor;
using Rhinox.GUIUtils.Editor.Helpers;
using UnityEngine;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerBaseDataPage : PagerPage
    {
        protected GenericDataTable _dataTable;

        public DataLayerBaseDataPage(SlidePageNavigationHelper<object> pager, GenericDataTable dataTable) : base(pager)
        {
            _dataTable = dataTable;
            _dataTable.DataChanged += OnDataChanged;
        }

        ~DataLayerBaseDataPage()
        {
            if (_dataTable != null)
                _dataTable.DataChanged -= OnDataChanged;
        }

        private void OnDataChanged(GenericDataTable sender)
        {
            DataLayer.RefreshEndPoint(sender.EndPoint);
        }
    }
}