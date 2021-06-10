using System.Collections.Generic;
using Game.Entities;
using Models;
using UnityEngine;

namespace Game
{
    public class MainCameraManager : MonoBehaviour
    {
        [SerializeField]
        private float Angle;
        [SerializeField]
        private float ZScale = -1.15f;
        [SerializeField]
        private float ZOffset = -10;
        
        public Camera Camera { get; private set; }

        private GameObject _focus;
        
        private bool _offset;

        private HashSet<Entity> _rotatatingEntities;
        
        private void Awake()
        {
            Camera = Camera.main;
            Camera!.orthographicSize = PlayerPrefs.GetFloat("Map Scale", 6);
            Camera.transparencySortMode = TransparencySortMode.CustomAxis;
            _offset = PlayerPrefs.GetInt("Camera Offset", 0) == 1;
            _rotatatingEntities = new HashSet<Entity>();
        }

        private void Update()
        {
            CheckForInputs();

            transform.rotation = Quaternion.Euler(0, 0, Settings.CameraAngle * Mathf.Rad2Deg);
            if (!(_focus is null))
            {
                var yOffset = (_offset ? 2.5f : 0) * ((Camera.orthographicSize - 6) / 3f + 1);
                transform.position = new Vector3(_focus.transform.position.x, _focus.transform.position.y + yOffset, ZOffset);
            }

            var orthoHeight = Camera.orthographicSize;
            var orthoWidth = Camera.aspect * orthoHeight;
            var m = Matrix4x4.Ortho(-orthoWidth, orthoWidth, -orthoHeight, orthoHeight, Camera.nearClipPlane, Camera.farClipPlane);
            var s = ZScale / orthoHeight;
            m[0, 2] = s * Mathf.Sin (Mathf.Deg2Rad * -Angle);
            m[1, 2] = -s * Mathf.Cos (Mathf.Deg2Rad * -Angle);
            m[0, 3] = -ZOffset * m[0, 2];
            m[1, 3] = -ZOffset * m[1, 2];
            Camera.projectionMatrix = m;
            Camera.transparencySortAxis = transform.up;

            foreach (var entity in _rotatatingEntities)
            {
                entity.Rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            }
        }
        
        private void CheckForInputs()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.mouseScrollDelta != Vector2.zero)
                {
                    var newSize = Camera.orthographicSize - Input.mouseScrollDelta.y * 0.1f;
                    Camera.orthographicSize = Mathf.Clamp(newSize, 3, 7.5f);
                    PlayerPrefs.SetFloat("Map Scale", Camera.orthographicSize); //TODO move to settings and add save on exit
                }
            }
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                _offset = !_offset;
                PlayerPrefs.SetInt("Camera Offset", _offset ? 1 : 0);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                transform.rotation = Quaternion.identity;
            }
        }

        public void SetFocus(GameObject focus)
        {
            _focus = focus;
        }

        public void AddRotatingEntity(Entity entity)
        {
            _rotatatingEntities.Add(entity);
        }

        public void RemoveRotatingEntity(Entity entity)
        {
            _rotatatingEntities.Remove(entity);
        }
    }
}