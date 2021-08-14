using UI.GameScreen.Panels.Components;
using UnityEngine;

namespace UI.GameScreen.Panels
{
    public class PartyPanel : Panel
    {
        [SerializeField]
        private PartyPanelListItem[] _members;

        private void Awake()
        {
            foreach (var member in _members)
            {
                member.Init(Color.white, false, null);
            }
        }

        private void Update()
        {
            foreach (var member in _members)
            {
                member.Draw(null);
            }
        }
    }
}