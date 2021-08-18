using System;
using Game.Entities;
using UnityEngine;
using Utils;

namespace Game.EntityWrappers
{
    public class EntityWrapper : MonoBehaviour
    {
        public Entity Entity { get; private set; }
        
        [SerializeField]
        protected SpriteRenderer Renderer;
        
        [SerializeField]
        protected SpriteRenderer ShadowRenderer;
        protected Transform ShadowTransform;
        private int currentShadowScale;

        protected static MainCameraManager CameraManager;

        private GameObject _model;
        
        private void Awake()
        {
            if (!Renderer)
                Renderer = GetComponent<SpriteRenderer>();

            if (!ShadowRenderer)
            {
                throw new NullReferenceException($"ShadowRenderer not assigned to {gameObject.name}");
            }
            ShadowTransform = ShadowRenderer.transform;
            
            if (!CameraManager)
                CameraManager = Camera.main.GetComponent<MainCameraManager>();

            _propertyBlock = new MaterialPropertyBlock();
        }

        private void OnDisable()
        {
            if (Entity?.Model)
                Destroy(_model);
            
            CameraManager.RemoveRotatingEntity(Entity);
        }

        public virtual void Init(Entity entity, bool rotating = true)
        {
            Entity = entity;

            Renderer.sortingLayerName = entity.Desc.DrawUnder ? "DrawUnder" : "Visible";
            ShadowRenderer.gameObject.SetActive(!entity.Desc.DrawOnGround);

            SetPositionAndRotation();
            RedrawShadow();

            if (rotating && !entity.Desc.DrawOnGround)
                CameraManager.AddRotatingEntity(Entity);

            if (entity.IsMyPlayer)
                CameraManager.SetFocus(gameObject);

            if (Entity.Model)
            {
                AddModel();
                Renderer.sprite = null;
            }
        }

        public virtual bool Tick()
        {
            if (!Entity.Tick(GameTime.Time, GameTime.DeltaTime, CameraManager.Camera))
            {
                return false;
            }
            
            var shadowSize = Entity.Size * Entity.Desc.ShadowSize * Entity.SizeMult;
            if (shadowSize != currentShadowScale)
            {
                currentShadowScale = shadowSize;
                RedrawShadow();
            }

            if (!Entity.Model)
                Renderer.sprite = Entity.TextureProvider.GetTexture(GameTime.Time);

            SetPositionAndRotation();
            ApplySink();
            return true;
        }

        private void RedrawShadow()
        {
            if (Entity.Desc.DrawOnGround)
                return;
            
            var scale = Entity.Size / 100f * (Entity.Desc.ShadowSize / 100f) * Entity.SizeMult;
            ShadowTransform.localScale = new Vector3(scale, scale, 1);

            var color = Entity.Desc.ShadowColor;
            color.a = .25f;
            ShadowRenderer.color = color;
        }

        private void SetPositionAndRotation()
        {
            var yOffset = Entity.Desc.DrawOnGround ? -0.5f : 0;
            transform.position = new Vector3(Entity.Position.x, Entity.Position.y + yOffset, -Entity.Z);
            transform.rotation = Quaternion.Euler(0, 0, Entity.Rotation * Mathf.Rad2Deg);
            
            if (_model)
                _model.transform.rotation = Quaternion.identity;
        }

        private static readonly int _YOffset = Shader.PropertyToID("_YOffset");
        private MaterialPropertyBlock _propertyBlock;
        private void ApplySink()
        {
            if (Entity.Square == null)
                return;
            
            var sinkValue = (Entity.Square.SinkLevel + Entity.SinkLevel) / 48f;
            if (sinkValue > 0 && (Entity.Flying ||
                                  Entity.Square.StaticObject != null &&
                                  Entity.Square.StaticObject.Desc.ProtectFromSink))
            {
                sinkValue = 0;
            }

            Renderer.GetPropertyBlock(_propertyBlock);
            
            _propertyBlock.SetFloat(_YOffset, sinkValue);
            Renderer.SetPropertyBlock(_propertyBlock);
        }
        
        private static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        private void AddModel()
        {
            _model = Instantiate(Entity.Model, transform);
            var renderer = _model.GetComponentInChildren<Renderer>();
            var texture = SpriteUtils.CreateTexture(Entity.Desc.TextureData.Texture);
            renderer.material.SetTexture(_MainTex, texture);
        }
    }
}