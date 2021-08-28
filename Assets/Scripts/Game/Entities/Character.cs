using Game.Overlay;
using Models.Static;

namespace Game.Entities
{
    public class Character : Entity
    {
        private HealthBar _healthBar;

        protected override void Init(ObjectDesc desc, int objectId, bool isMyPlayer, Map map, bool rotating = true)
        {
            base.Init(desc, objectId, isMyPlayer, map, rotating);

            if (_healthBar == null)
                _healthBar = map.Overlay.GetHealthBar();
            
            _healthBar.Init(this);
            _healthBar.gameObject.SetActive(true);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            // because this is attached to the overlay canvas, it does not get disabled when this does
            _healthBar.gameObject.SetActive(false);
        }

        public override void Draw()
        {
            base.Draw();
            
            _healthBar.Draw();
        }
    }
}