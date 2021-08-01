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

        //TODO extract out into shadow drawer class
        [SerializeField]
        protected SpriteRenderer ShadowRenderer;
        protected Transform ShadowTransform;
        private int currentShadowScale;

        protected static MainCameraManager CameraManager;
        
        private void Awake()
        {
            if (!Renderer)
                Renderer = GetComponent<SpriteRenderer>();

            if (!ShadowRenderer)
            {
                throw new Exception("ShadowRenderer not assigned");
            }
            ShadowTransform = ShadowRenderer.transform;
            
            if (!CameraManager)
                CameraManager = Camera.main.GetComponent<MainCameraManager>();
        }

        public virtual void Init(Entity child, bool rotating)
        {
            Entity = child;

            SetPositionAndRotation();
            RedrawShadow();

            if (rotating)
                CameraManager.AddRotatingEntity(Entity);
            
            if (child.IsMyPlayer)
                CameraManager.SetFocus(gameObject);
        }

        protected virtual void Update()
        {
            Entity.Tick(GameTime.Time, GameTime.DeltaTime, CameraManager.Camera);
            
            var shadowSize = Entity.Size * Entity.Desc.ShadowSize * Entity.SizeMult;
            if (shadowSize != currentShadowScale)
            {
                currentShadowScale = shadowSize;
                RedrawShadow();
            }
                

            Renderer.sprite = Entity.TextureProvider.GetTexture(GameTime.Time);

            SetPositionAndRotation();
        }

        private void RedrawShadow()
        {
            var scale = Entity.Size / 100f * (Entity.Desc.ShadowSize / 100f) * Entity.SizeMult;
            ShadowTransform.localScale = new Vector3(scale, scale, 1);

            var color = Entity.Desc.ShadowColor;
            color.a = .25f;
            ShadowRenderer.color = color;
        }

        private void SetPositionAndRotation()
        {
            transform.position = Entity.Position;
            transform.rotation = Quaternion.Euler(0, 0, Entity.Rotation * Mathf.Rad2Deg);
        }
    }
}