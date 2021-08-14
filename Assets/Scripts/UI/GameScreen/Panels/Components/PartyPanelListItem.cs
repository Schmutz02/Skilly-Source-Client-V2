using Game.Entities;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen.Panels.Components
{
    public class PartyPanelListItem : MonoBehaviour
    {
        [SerializeField]
        private Image _portraitRenderer;
        [SerializeField]
        private TextMeshProUGUI _nameText;

        private Color _color;
        private Color _tint;
        private bool _longVersion;
        private Entity _entity;

        public void Init(Color color, bool longVersion, Entity entity)
        {
            _color = color;
            _longVersion = longVersion;

            _nameText.fontStyle = _longVersion ? 
                FontStyles.Normal : 
                FontStyles.Bold;
            
            Draw(entity);
        }
        
        public void Draw(Entity entity) => Draw(entity, Color.white);

        public void Draw(Entity entity, Color tint)
        {
            _entity = entity;
            gameObject.SetActive(_entity != null);
            if (!gameObject.activeSelf)
                return;

            _portraitRenderer.sprite = entity.TextureProvider.GetPortrait();
            var nameColor = _color;
            var nameText = entity.Name;
            if (entity is Player player)
            {
                //TODO guild color
                nameColor = Settings.NameColor;
            }

            if (_longVersion)
            {
                if (!string.IsNullOrEmpty(entity.Name))
                {
                    nameText = $"<b>{entity.Name}</b> ({entity.Desc.DisplayId})";
                    //TODO level things
                }
                else
                {
                    nameText = $"<b>{entity.Desc.DisplayId}</b>";
                }
            }
            else if (string.IsNullOrEmpty(entity.Name))
            {
                nameText = entity.Desc.DisplayId;
            }

            InternalDraw(nameColor, nameText, tint);
        }

        private void InternalDraw(Color nameColor, string nameText, Color tint)
        {
            if (nameColor == _color && nameText == _nameText.text && tint == _tint)
                return;

            _nameText.color = nameColor;
            _nameText.text = nameText;

            _nameText.color *= tint;
            _portraitRenderer.color *= tint;

            _color = nameColor;
            _tint = tint;
        }
    }
}