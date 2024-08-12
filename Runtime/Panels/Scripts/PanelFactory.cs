using UnityEngine;
using UnityEngine.UIElements;

namespace OC.UI.Panel
{
    public static class PanelFactory
    {
        public static Panel Create(PanelHandler panelHandler)
        {
            if (panelHandler.Component == null) return null;

            return panelHandler.Component switch
            {
                MaterialFlow.Source source => Create(panelHandler, source),
                MaterialFlow.Payload entity => Create(panelHandler, entity),
                MaterialFlow.Sink sink => Create(panelHandler, sink),
                MaterialFlow.TypeChanger changer => Create(panelHandler, changer),
                Components.Cylinder cylinder => Create(panelHandler, cylinder),
                Components.DriveSimple drive => Create(panelHandler, drive),
                Components.DriveSpeed drive => Create(panelHandler, drive),
                Components.DrivePosition drive => Create(panelHandler, drive),
                Components.SensorBinary binary => Create(panelHandler, binary),
                Components.SensorAnalog analog => Create(panelHandler, analog),
                Components.TagReader tagReader => Create(panelHandler, tagReader),
                Components.DataReader dataReader => Create(panelHandler, dataReader),
                OC.Interactions.Lock doorLock => Create(panelHandler, doorLock),
                _ => null
            };
        }

        private static Panel Create(PanelHandler panelHandler, Components.DataReader target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Collision", target.Collision));
            groupStatus.Add(new StringField("Value", target.TargetData));
            
            panel.Add(groupStatus);
            
            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, MaterialFlow.TypeChanger target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Collision", target.Collision));

            var groupControl = new GroupContainer("Control");
            groupControl.Add(new IntegerField("Type ID", target.TargetTypeID));
            groupControl.Add(new Button("Replace", target.Replace));
            
            panel.Add(groupStatus);
            panel.Add(groupControl);

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, Components.TagReader target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;
            
            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Collision", target.Collision));
            groupStatus.Add(new ULongField("Value", target.Value));
            
            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Override", target.Override));
            groupControl.Add(new ULongField("Value", target.Value));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new LinkStatus(target.Link));

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, Components.SensorAnalog target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new FloatField("Value", target.Value, true));

            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Override", target.Override));
            groupControl.Add(new FloatField("Target", target.Value, target.Override));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new LinkStatus(target.Link));

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, Components.SensorBinary target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Value", target.Value));

            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Override", target.Override));
            groupControl.Add(new ToggleSlide("State", target.State, target.Override));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new LinkStatus(target.Link));

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, MaterialFlow.Source target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Collision", target.Collision));

            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Auto", target.Auto));
            groupControl.Add(new IntegerField("TypeId", target.TypeId));
            groupControl.Add(new ULongField("UniqueId", (IProperty<ulong>)target.UniqueId));
            groupControl.Add(new Button("Create", target.Create));
            groupControl.Add(new Button("Delete", target.Delete));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            
            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, MaterialFlow.Payload target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupData = new GroupContainer("Data");
            groupData.Add(new IntegerField("TypeId", target.TypeId));
            groupData.Add(new ULongField("UniqueId", target.UniqueId));
            
            panel.Add(groupData);
            AddEntityTagContainer(panel, target);
            
            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, MaterialFlow.Sink target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;
            
            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Collision", target.Collision));

            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Auto", target.Auto));
            groupControl.Add(new Button("Delete", target.Delete));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            
            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, Components.Cylinder target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Is Active", target.IsActive));
            groupStatus.Add(new ProgressBarWithLimits("Progress", target.Progress));

            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Override", target.Override));
            var hoizontalGroup = new HorizontalGroup();
            hoizontalGroup.Add(new PushButton("Minus", target.Minus, target.Override));
            hoizontalGroup.Add(new PushButton("Plus", target.Plus, target.Override));
            groupControl.Add(hoizontalGroup);

            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new LinkStatus(target.Link));

            return panel;
        }
        
        private static Panel Create(PanelHandler panelHandler, Components.DriveSimple target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Is Active", target.IsActive));
            groupStatus.Add(new FloatField("Speed", target.Value));
            
            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Override", target.Override));
            var hoizontalGroup = new HorizontalGroup();
            hoizontalGroup.Add(new ToggleButton("Backward", target.Backward, target.Override));
            hoizontalGroup.Add(new ToggleButton("Forward", target.Forward, target.Override));
            groupControl.Add(hoizontalGroup);
            
            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new LinkStatus(target.Link));

            return panel;
        }
        
        private static Panel Create(PanelHandler panelHandler, Components.DriveSpeed target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Is Active", target.IsActive));
            groupStatus.Add(new FloatField("Acceleration", target.Value));

            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Override", target.Override));
            groupControl.Add(new FloatField("Target", target.Target, target.Override));
            
            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new LinkStatus(target.Link));

            return panel;
        }
        
        private static Panel Create(PanelHandler panelHandler, Components.DrivePosition target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Is Active", target.IsActive));
            groupStatus.Add(new FloatField("Position", target.Value));

            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Override", target.Override));
            groupControl.Add(new FloatField("Target", target.Target, target.Override));
            
            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new LinkStatus(target.Link));

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, OC.Interactions.Lock target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new GroupContainer("Status");
            groupStatus.Add(new BinaryStatusField("Closed", target.Closed));
            groupStatus.Add(new BinaryStatusField("Locked", target.Locked));
            
            var groupControl = new GroupContainer("Control");
            groupControl.Add(new ToggleSlide("Override", target.Override));
            var hoizontalGroup = new HorizontalGroup();
            hoizontalGroup.Add(new ToggleButton("Lock", target.LockSignal, target.Override));
            groupControl.Add(hoizontalGroup);
            
            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new LinkStatus(target.Link));

            return panel;
        }

        public static Panel Create(GenericPanelHandler genericPanelHandler)
        {
            if (genericPanelHandler.Component == null) return null;

            var panel = new Panel(genericPanelHandler.Target.name, true, true, true);
            panel.OnFocus += genericPanelHandler.Focus;
            panel.OnClose += genericPanelHandler.Delete;

            foreach (var binding in genericPanelHandler.Bindings)
            {
                if (!genericPanelHandler.PossibleBindings.Contains(binding.Property))
                {
                    Debug.Log($"Can't find Property of name <b>{binding.Property}</b> in <b>{genericPanelHandler.Component.name}</b>");
                    continue;
                }

                var propertyInfo = genericPanelHandler.Component.GetType().GetProperty(binding.Property);

                if (propertyInfo == null) continue;

                var isReadOnly = !propertyInfo.CanWrite || binding.IsReadOnly;

                var property = (IProperty)propertyInfo.GetValue(genericPanelHandler.Component);

                if (property == null) continue;

                panel.Add(property switch
                {
                    IProperty<float> prop => new FloatField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<int> prop => new IntegerField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<uint> prop => new UIntegerField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<long> prop => new LongField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<ulong> prop => new ULongField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<Vector2> prop => new Vector2Field(propertyInfo.Name, prop, isReadOnly),
                    IProperty<Vector2Int> prop => new Vector2IntField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<Vector3> prop => new Vector3Field(propertyInfo.Name, prop, isReadOnly),
                    IProperty<Vector3Int> prop => new Vector3IntField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<bool> prop => MakeBoolField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<string> prop => new StringField(propertyInfo.Name, prop, isReadOnly),
                    _ => null
                });
            }

            return panel;
        }

        private static VisualElement MakeBoolField(string name, IProperty<bool> prop, bool isReadOnly)
        {
            if (isReadOnly)
            {
                return new BinaryStatusField(name, prop);
            }
            else
            {
                return new ToggleSlide(name, prop);
            }
        }

        private static void AddEntityTagContainer(Panel panel, MaterialFlow.Payload entity)
        {
            if (!entity.TryGetComponent(out MaterialFlow.PayloadTag entityTag)) return;

            foreach (var directoryId in entityTag.DirecotryId)
            {
                panel.Add(new EntityTagContainer(entityTag, directoryId));
            }
        }
    }
}