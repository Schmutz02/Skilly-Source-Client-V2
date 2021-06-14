using System;
using Game.Entities;
using UnityEngine;

namespace Game.EntityWrappers
{
    public class EntityWrapper : MonoBehaviour
    {
        public Entity Entity { get; private set; }
        
        [SerializeField]
        protected SpriteRenderer Renderer;

        protected static MainCameraManager CameraManager;
        
        private void Awake()
        {
            if (!Renderer)
                Renderer = GetComponent<SpriteRenderer>();
            
            if (!CameraManager)
                CameraManager = Camera.main.GetComponent<MainCameraManager>();
        }

        public virtual void Init(Entity child, bool rotating)
        {
            Entity = child;

            SetPositionAndRotation();
            
            if (rotating)
                CameraManager.AddRotatingEntity(Entity);
            
            if (child.IsMyPlayer)
                CameraManager.SetFocus(gameObject);
        }

        protected virtual void Update()
        {
            Entity.Tick(GameTime.Time, GameTime.DeltaTime, CameraManager.Camera);

            Renderer.sprite = Entity.TextureProvider.GetTexture(GameTime.Time);

            SetPositionAndRotation();
        }

        private void SetPositionAndRotation()
        {
            transform.position = Entity.Position;
            transform.rotation = Quaternion.Euler(0, 0, Entity.Rotation * Mathf.Rad2Deg);
        }
    }
}