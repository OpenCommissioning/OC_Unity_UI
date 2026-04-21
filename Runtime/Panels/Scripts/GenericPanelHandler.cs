using System.Collections.Generic;
using System.Reflection;

namespace OC.UI.Panel
{
    public class GenericPanelHandler : PanelHandler
    {
        public List<FieldBinding> Bindings = new ();

        public IList<string> PossibleBindings => _possibleBindings;

        private List<string> _possibleBindings;

        protected virtual void OnValidate()
        {
            //base.OnValidate();

            _possibleBindings = new List<string>();

            var componentProperties = Component.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in componentProperties)
            {
                if (propertyInfo.PropertyType.IsGenericType)
                {
                    if (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Property<>)
                        || propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IProperty<>))
                    {
                        _possibleBindings.Add(propertyInfo.Name);
                    }
                }
            }
        }

        protected override void Create()
        {
            _panel ??= PanelFactory.Create(this);
        }

        public new void Close()
        {
            _panel?.Close();
        }

        public new void Delete()
        {
            _panel?.Delete();
            _panel = null;
        }
    }
}
