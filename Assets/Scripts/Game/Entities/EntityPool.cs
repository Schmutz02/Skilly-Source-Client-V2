using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities
{
    public class EntityPool
    {
        private readonly Dictionary<string, Queue<Entity>> _entityPool;
        private readonly Dictionary<string, Entity> _entityPrefabs;
        private readonly Transform _wrapperParent;

        public EntityPool(Dictionary<string, Entity> prefabs, Transform wrapperParent)
        {
            _entityPool = new Dictionary<string, Queue<Entity>>();
            _entityPrefabs = prefabs;
            _wrapperParent = wrapperParent;
        }

        public Entity Get(string type)
        {
            if (!_entityPrefabs.ContainsKey(type))
            {
                Debug.LogWarning("PREFAB NOT IMPLEMENTED");
                return null;
            }
            
            if (!_entityPool.TryGetValue(type, out var queue))
                _entityPool[type] = queue = new Queue<Entity>();

            if (queue.Count > 0)
                return queue.Dequeue();

            var entity = Object.Instantiate(_entityPrefabs[type], _wrapperParent);
            return entity;
        }

        public void Return(Entity entity)
        {
            if (entity == null)
            {
                Debug.LogWarning("CAN NOT RETURN NULL WRAPPER");
                return;
            }

            entity.Dispose();

            _entityPool[entity.Desc.Class].Enqueue(entity);
        }
    }
}