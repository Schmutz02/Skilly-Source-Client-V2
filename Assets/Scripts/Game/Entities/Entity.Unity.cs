using System;
using UnityEngine;
using Utils;

namespace Game.Entities
{
    public partial class Entity
    {
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
        
        public virtual void Dispose()
        {
            if (_model)
                Destroy(_model);
            
            CameraManager.RemoveRotatingEntity(this);

            gameObject.SetActive(false);
        }
        
        public virtual void Draw()
        {
            var shadowSize = Size * Desc.ShadowSize * SizeMult;
            if (shadowSize != currentShadowScale)
            {
                currentShadowScale = shadowSize;
                RedrawShadow();
            }

            if (!_model)
                Renderer.sprite = TextureProvider.GetTexture(GameTime.Time);

            SetPositionAndRotation();
            ApplySink();
        }
        
        private void SetPositionAndRotation()
        {
            if (_model)
                _model.transform.rotation = Quaternion.identity;
        }
        
        private void RedrawShadow()
        {
            if (Desc.DrawOnGround)
                return;
            
            var scale = Size / 100f * (Desc.ShadowSize / 100f) * SizeMult;
            ShadowTransform.localScale = new Vector3(scale, scale, 1);

            var color = Desc.ShadowColor;
            color.a = .25f;
            ShadowRenderer.color = color;
        }
        
        private static readonly int _YOffset = Shader.PropertyToID("_YOffset");
        private MaterialPropertyBlock _propertyBlock;
        private void ApplySink()
        {
            if (Square == null)
                return;
            
            var sinkValue = (Square.SinkLevel + SinkLevel) / 48f;
            if (sinkValue > 0 && (Flying ||
                                  Square.StaticObject != null &&
                                  Square.StaticObject.Desc.ProtectFromSink))
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
            var go = AssetLibrary.GetModel(Desc.Model);
            _model = Instantiate(go, transform);
            var renderer = _model.GetComponentInChildren<Renderer>();
            var texture = SpriteUtils.CreateTexture(Desc.TextureData.Texture);
            renderer.material.SetTexture(_MainTex, texture);
        }
    }
}