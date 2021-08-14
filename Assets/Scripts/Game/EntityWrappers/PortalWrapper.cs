using Game.Entities;
using UI.GameScreen.Panels;
using UnityEngine;

namespace Game.EntityWrappers
{
    public class PortalWrapper : EntityWrapper, IInteractiveObject
    {
        private Portal _portal;
        
        [SerializeField]
        private PortalPanel _portalPanel;

        public override void Init(Entity portal, bool rotating = true)
        {
            base.Init(portal, rotating);
            
            _portal = portal as Portal;
        }

        public Panel GetPanel(InteractPanel interactPanel)
        {
            var panel = interactPanel.GetPanel<PortalPanel>();
            if (panel == null)
            {
                panel = Instantiate(_portalPanel, interactPanel.transform);
            }
            panel.Init(_portal);
            return panel;
        }
    }
}