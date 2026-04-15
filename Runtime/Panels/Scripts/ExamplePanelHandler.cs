using OC.Components;
using UnityEngine;

namespace OC.UI.Panel
{
    public class ExamplePanelHandler : CustomPanelHandler
    {
        // declare reference to other objects
        public DriveSimple Drive => _drive;

        [SerializeField]
        private DriveSimple _drive;

        private protected override void CreateContent()
        {
            // cast Component to type of Interaction class - IMPORTANT
            var target = (GenericPanelTestInteraction)Component;

            // add panelfields directly to panel using target from above
            _panel.Add(new PanelGroupContainer("Custom Panel Example"));
            _panel.Add(new PanelFloatField("Float", target.Float));

            // add panelfield for Properties of referenced object
            _panel.Add(new PanelFloatField(_drive.name + "_Speed", _drive.Speed));
        }
    }

}