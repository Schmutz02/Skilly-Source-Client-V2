using UI.GameScreen.Panels;
using UnityEngine;

namespace Game.Entities
{
    public partial class Portal
    {
        [SerializeField]
        private PortalPanel _portalPanel;
        
        public Panel GetPanel(InteractPanel interactPanel)
        {
            var panel = interactPanel.GetPanel<PortalPanel>();
            if (panel == null)
            {
                panel = Instantiate(_portalPanel, interactPanel.transform);
            }
            panel.Init(this);
            return panel;
        }
    }
}