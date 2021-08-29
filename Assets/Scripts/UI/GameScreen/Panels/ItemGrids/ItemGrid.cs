using Game.Entities;

namespace UI.GameScreen.Panels.ItemGrids
{
    public class ItemGrid : Panel
    {
        protected Entity Owner;
        protected Player CurrentPlayer;
        protected int IndexOffset;
        
        public void Init(Entity owner, Player currentPlayer, int itemIndexOffset)
        {
            Owner = owner;
            CurrentPlayer = currentPlayer;
            IndexOffset = itemIndexOffset;
            
        }
    }
}