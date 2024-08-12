namespace OC.UI.Panel
{
    public abstract class CustomPanelHandler : PanelHandler
    {
        protected override void Create()
        {
            if (Component == null) return;

            _panel = new Panel(Target.name, true, true, true);
            _panel.OnFocus += base.Focus;
            _panel.OnClose += base.Delete;
            CreateContent();
        }

        private protected abstract void CreateContent();
    }
}