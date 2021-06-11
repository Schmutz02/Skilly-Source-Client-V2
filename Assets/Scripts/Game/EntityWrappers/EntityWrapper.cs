using System;
using Game.Entities;
using UnityEngine;
using Action = Models.Static.Action;

namespace Game.EntityWrappers
{
    public class EntityWrapper : MonoBehaviour
    {
        protected Entity Entity;
        
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
            
            Renderer.sprite = Entity.Desc.TextureData.Texture ? 
                Entity.Desc.TextureData.Texture : 
                Entity.Desc.TextureData.Animation.GetFrame(Facing.Down, Action.Stand, 0);
            
            SetPositionAndRotation();
            
            if (rotating)
                CameraManager.AddRotatingEntity(Entity);
            
            if (child.IsMyPlayer)
                CameraManager.SetFocus(gameObject);
        }

        protected virtual void Update()
        {
            Entity.Tick(GameTime.Time, GameTime.DeltaTime, CameraManager.Camera);

            SetPositionAndRotation();
        }

        private void SetPositionAndRotation()
        {
            transform.position = Entity.Position;
            transform.rotation = Quaternion.Euler(0, 0, Entity.Rotation * Mathf.Rad2Deg);
        }
    }
}