using Models.Static;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen.Panels.ItemGrids.ItemTiles
{
    public class EquipmentTile : InteractiveItemTile
    {
        [SerializeField]
        private Image _detailImage;

        private ItemType _slotType;

        public void SetType(ItemType type)
        {
            Sprite bg = null;
            switch (type)
            {
                case ItemType.All:
                    break;
                case ItemType.Sword:
                    bg = AssetLibrary.GetImage("lofiObj5", 48);
                    break;
                case ItemType.Dagger:
                    bg = AssetLibrary.GetImage("lofiObj5", 96);
                    break;
                case ItemType.Bow:
                    bg = AssetLibrary.GetImage("lofiObj5", 80);
                    break;
                case ItemType.Tome:
                    bg = AssetLibrary.GetImage("lofiObj6", 80);
                    break;
                case ItemType.Shield:
                    bg = AssetLibrary.GetImage("lofiObj6", 112);
                    break;
                case ItemType.Leather:
                    bg = AssetLibrary.GetImage("lofiObj5", 0);
                    break;
                case ItemType.Plate:
                    bg = AssetLibrary.GetImage("lofiObj5", 32);
                    break;
                case ItemType.Wand:
                    bg = AssetLibrary.GetImage("lofiObj5", 64);
                    break;
                case ItemType.Ring:
                    bg = AssetLibrary.GetImage("lofiObj", 44);
                    break;
                case ItemType.Spell:
                    bg = AssetLibrary.GetImage("lofiObj6", 64);
                    break;
                case ItemType.Seal:
                    bg = AssetLibrary.GetImage("lofiObj6", 160);
                    break;
                case ItemType.Cloak:
                    bg = AssetLibrary.GetImage("lofiObj6", 32);
                    break;
                case ItemType.Robe:
                    bg = AssetLibrary.GetImage("lofiObj5", 16);
                    break;
                case ItemType.Quiver:
                    bg = AssetLibrary.GetImage("lofiObj6", 48);
                    break;
                case ItemType.Helm:
                    bg = AssetLibrary.GetImage("lofiObj6", 96);
                    break;
                case ItemType.Staff:
                    bg = AssetLibrary.GetImage("lofiObj5", 112);
                    break;
                case ItemType.Poison:
                    bg = AssetLibrary.GetImage("lofiObj6", 128);
                    break;
                case ItemType.Skull:
                    bg = AssetLibrary.GetImage("lofiObj6", 0);
                    break;
                case ItemType.Trap:
                    bg = AssetLibrary.GetImage("lofiObj6", 16);
                    break;
                case ItemType.Orb:
                    bg = AssetLibrary.GetImage("lofiObj6", 144);
                    break;
                case ItemType.Prism:
                    bg = AssetLibrary.GetImage("lofiObj6", 176);
                    break;
                case ItemType.Scepter:
                    bg = AssetLibrary.GetImage("lofiObj6", 192);
                    break;
                case ItemType.Katana:
                    bg = AssetLibrary.GetImage("lofiObj3", 540);
                    break;
                case ItemType.Shuriken:
                    bg = AssetLibrary.GetImage("lofiObj3", 555);
                    break;
            }

            _detailImage.sprite = bg;
            _slotType = type;
        }
    }
}