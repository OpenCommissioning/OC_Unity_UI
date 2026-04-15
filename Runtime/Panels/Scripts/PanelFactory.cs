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

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Collision", target.Collision));
            groupStatus.Add(new PanelStringField("Value", target.TargetData));
            
            panel.Add(groupStatus);
            
            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, MaterialFlow.TypeChanger target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Collision", target.Collision));

            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelIntegerField("Type ID", target.TargetTypeID));
            groupControl.Add(new PanelButton("Replace", target.Replace));
            
            panel.Add(groupStatus);
            panel.Add(groupControl);

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, Components.TagReader target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;
            
            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Collision", target.Collision));
            groupStatus.Add(new PanelULongField("Value", target.Value));
            
            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Override", target.Override));
            groupControl.Add(new PanelULongField("Value", target.Value));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new PanelLinkStatus(target.Link));

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, Components.SensorAnalog target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelFloatField("Value", target.Value, true));

            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Override", target.Override));
            groupControl.Add(new PanelFloatField("Target", target.Value, target.Override));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new PanelLinkStatus(target.Link));

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, Components.SensorBinary target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Value", target.Value));

            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Override", target.Override));
            groupControl.Add(new PanelToggleSlide("State", target.State, target.Override));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new PanelLinkStatus(target.Link));

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, MaterialFlow.Source target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Collision", target.Collision));

            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Auto", target.Auto));
            groupControl.Add(new PanelIntegerField("TypeId", target.TypeId));
            groupControl.Add(new PanelULongField("UniqueId", (IProperty<ulong>)target.UniqueId));
            groupControl.Add(new PanelButton("Create", target.Create));
            groupControl.Add(new PanelButton("Delete", target.Delete));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            
            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, MaterialFlow.Payload target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupData = new PanelGroupContainer("Data");
            groupData.Add(new PanelIntegerField("TypeId", target.TypeId));
            groupData.Add(new PanelULongField("UniqueId", target.UniqueId));
            
            panel.Add(groupData);
            AddEntityTagContainer(panel, target);
            
            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, MaterialFlow.Sink target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;
            
            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Collision", target.Collision));

            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Auto", target.Auto));
            groupControl.Add(new PanelButton("Delete", target.Delete));

            panel.Add(groupStatus);
            panel.Add(groupControl);
            
            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, Components.Cylinder target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Is Active", target.IsActive));
            groupStatus.Add(new PanelProgressBarWithLimits("Progress", target.Progress));

            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Override", target.Override));
            var hoizontalGroup = new PanelHorizontalGroup();
            hoizontalGroup.Add(new PanelPushButton("Minus", target.Minus, target.Override));
            hoizontalGroup.Add(new PanelPushButton("Plus", target.Plus, target.Override));
            groupControl.Add(hoizontalGroup);

            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new PanelLinkStatus(target.Link));

            return panel;
        }
        
        private static Panel Create(PanelHandler panelHandler, Components.DriveSimple target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Is Active", target.IsActive));
            groupStatus.Add(new PanelFloatField("Speed", target.Value));
            
            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Override", target.Override));
            var hoizontalGroup = new PanelHorizontalGroup();
            hoizontalGroup.Add(new PanelToggleButton("Backward", target.Backward, target.Override));
            hoizontalGroup.Add(new PanelToggleButton("Forward", target.Forward, target.Override));
            groupControl.Add(hoizontalGroup);
            
            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new PanelLinkStatus(target.Link));

            return panel;
        }
        
        private static Panel Create(PanelHandler panelHandler, Components.DriveSpeed target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Is Active", target.IsActive));
            groupStatus.Add(new PanelFloatField("Acceleration", target.Value));

            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Override", target.Override));
            groupControl.Add(new PanelFloatField("Target", target.Target, target.Override));
            
            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new PanelLinkStatus(target.Link));

            return panel;
        }
        
        private static Panel Create(PanelHandler panelHandler, Components.DrivePosition target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Is Active", target.IsActive));
            groupStatus.Add(new PanelFloatField("Position", target.Value));

            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Override", target.Override));
            groupControl.Add(new PanelFloatField("Target", target.Target, target.Override));
            
            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new PanelLinkStatus(target.Link));

            return panel;
        }

        private static Panel Create(PanelHandler panelHandler, OC.Interactions.Lock target)
        {
            var panel = new Panel(panelHandler.Target.name, true, true, true);
            panel.OnFocus += panelHandler.Focus;
            panel.OnClose += panelHandler.Delete;

            var groupStatus = new PanelGroupContainer("Status");
            groupStatus.Add(new PanelBinaryStatusField("Closed", target.Closed));
            groupStatus.Add(new PanelBinaryStatusField("Locked", target.Locked));
            
            var groupControl = new PanelGroupContainer("Control");
            groupControl.Add(new PanelToggleSlide("Override", target.Override));
            var hoizontalGroup = new PanelHorizontalGroup();
            hoizontalGroup.Add(new PanelToggleButton("Lock", target.LockSignal, target.Override));
            groupControl.Add(hoizontalGroup);
            
            panel.Add(groupStatus);
            panel.Add(groupControl);
            panel.Add(new PanelLinkStatus(target.Link));

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
                    IProperty<float> prop => new PanelFloatField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<int> prop => new PanelIntegerField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<uint> prop => new PanelUIntegerField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<long> prop => new PanelLongField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<ulong> prop => new PanelULongField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<Vector2> prop => new PanelVector2Field(propertyInfo.Name, prop, isReadOnly),
                    IProperty<Vector2Int> prop => new PanelVector2IntField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<Vector3> prop => new PanelVector3Field(propertyInfo.Name, prop, isReadOnly),
                    IProperty<Vector3Int> prop => new PanelVector3IntField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<bool> prop => MakeBoolField(propertyInfo.Name, prop, isReadOnly),
                    IProperty<string> prop => new PanelStringField(propertyInfo.Name, prop, isReadOnly),
                    _ => null
                });
            }

            return panel;
        }

        private static VisualElement MakeBoolField(string name, IProperty<bool> prop, bool isReadOnly)
        {
            if (isReadOnly)
            {
                return new PanelBinaryStatusField(name, prop);
            }
            else
            {
                return new PanelToggleSlide(name, prop);
            }
        }

        private static void AddEntityTagContainer(Panel panel, MaterialFlow.Payload entity)
        {
            if (!entity.TryGetComponent(out MaterialFlow.PayloadTag entityTag)) return;

            foreach (var directoryId in entityTag.DirectoryId)
            {
                panel.Add(new PanelEntityTagContainer(entityTag, directoryId));
            }
        }
    }
}