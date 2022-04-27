using Rhinox.GUIUtils.Editor;
using Rhinox.GUIUtils.Odin.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Rhinox.Vortex.Editor
{
    public class DataLayerWindow : OdinPagerEditorWindow<DataLayerWindow>
    {
        [SerializeField, HideInInspector] private DataLayerMainPage _rootPage;
        protected override object RootPage => _rootPage ?? (_rootPage = new DataLayerMainPage(_pager));
        protected override string RootPageName => "Home";
    
        [MenuItem("Rhinox/Data Layer", priority = 200)]
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
        }

        protected override int DrawHeaderEditor()
        {
            int defaultHeight = 22;
            SirenixEditorGUI.BeginHorizontalToolbar(defaultHeight);
            GUILayout.FlexibleSpace();
            
            if (SirenixEditorGUI.IconButton(EditorIcons.Refresh))
                _rootPage.InitializeDataLayer(true);
            
            SirenixEditorGUI.EndHorizontalToolbar();
            return defaultHeight;
        }
    }
}