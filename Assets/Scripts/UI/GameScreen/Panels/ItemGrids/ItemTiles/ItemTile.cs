using UI.GameScreen.Panels.Components;
using UnityEngine;

namespace UI.GameScreen.Panels.ItemGrids.ItemTiles
{
    public class ItemTile : MonoBehaviour
    {
        [SerializeField]
        private Item _item;

        private int _tileId;
        private ItemGrid _parentGrid;
        
        public void Init(int id, ItemGrid parentGrid)
        {
            _tileId = id;
            _parentGrid = parentGrid;
        }
        
        public bool SetItem(int itemType, int itemData)
        {
            if (itemType == _item.ItemType && itemData == _item.ItemData)
                return false;
            
            _item.SetType(itemType, itemData);
            return true;
        }
    }
}