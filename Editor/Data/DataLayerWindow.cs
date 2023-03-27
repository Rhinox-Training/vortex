using Rhinox.GUIUtils;
using Rhinox.GUIUtils.Editor;
using Rhinox.Lightspeed;
using Rhinox.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerWindow : PagerEditorWindow<DataLayerWindow>
    {
        [SerializeField, HideInInspector] private DataLayerMainPage _rootPage;
        protected override object RootPage => _rootPage ?? (_rootPage = new DataLayerMainPage(_pager));
        protected override string RootPageName => "Home";
        
        private HoverTexture _refreshIcon;
        
        [MenuItem(WindowHelper.ToolsPrefix + "Vortex/Data Layer", priority = 200)]
        public static void OpenWindow()
        {
            DataLayerWindow window;
            if (!GetOrCreateWindow(out window)) return;
        
            window.name = "Data Layer";
            window.titleContent = new GUIContent("Data Layer", UnityIcon.InternalIcon("d_GameObject Icon"));
        }

        protected override void Initialize()
        {
            base.Initialize();
            _rootPage.Initialize();
            _refreshIcon = new HoverTexture(UnityIcon.AssetIcon("Fa_Redo"));
        }

        protected override int DrawHeaderEditor()
        {
            int defaultHeight = 22;

            var rect = CustomEditorGUI.BeginHorizontalToolbar(defaultHeight);
            GUILayout.FlexibleSpace();

            if (rect.IsValid())
                rect = rect.AlignRight(18).AlignCenterVertical(16);
            
            if (CustomEditorGUI.IconButton(rect, _refreshIcon, tooltip: "Reinitialize data cache"))
                _rootPage.InitializeDataLayer(true);
            
            CustomEditorGUI.EndHorizontalToolbar();
            return defaultHeight;
        }
    }
}