namespace OC.UI.ComponentLayout
{
    /// <summary>
    /// Default runtime hooks for toolbar/menu. Wire button clicks to these methods.
    /// </summary>
    public class LayoutRuntimeActions : ILayoutRuntimeActions
    {
        public void OnSaveLayoutRequested()
        {
            LayoutSaveSystem.Instance?.Save();
        }

        public void OnOpenImportLayoutRequested()
        {
            LayoutSaveSystem.Instance?.Load();
        }
    }
}
