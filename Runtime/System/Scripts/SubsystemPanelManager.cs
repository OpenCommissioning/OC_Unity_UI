using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI
{
    [RequireComponent(typeof(UIDocument))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public class SubsystemPanelManager : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _sidebar;
        
        private const string Uxml = "UXML/toolbar_subsystem";
        private const string StyleSheet = "StyleSheet/toolbar";
        
        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            _sidebar = Resources.Load<VisualTreeAsset>(Uxml).Instantiate().Q("toolbar");
            _sidebar.AddDefaultTheme();
            _sidebar.styleSheets.Add(Resources.Load<StyleSheet>(StyleSheet));
            uiDocument.rootVisualElement.Add(_sidebar);
            PopulateVisualTree();
        }

        private void PopulateVisualTree()
        {
            foreach (var item in GetComponentsInChildren<IPopulateVisualTree>(false))
            {
                item.Populate(_sidebar);
            }
        }
    }
}