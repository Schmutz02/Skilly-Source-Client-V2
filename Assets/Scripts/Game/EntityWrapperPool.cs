using System.Collections.Generic;
using Game.EntityWrappers;
using UnityEngine;

namespace Game
{
    public class EntityWrapperPool
    {
        private Dictionary<string, Queue<EntityWrapper>> _entityPool;
        private Dictionary<string, EntityWrapper> _entityPrefabs;
        private Transform _wrapperParent;

        public EntityWrapperPool(Transform wrapperParent, Dictionary<string, EntityWrapper> prefabs)
        {
            _entityPool = new Dictionary<string, Queue<EntityWrapper>>();
            _entityPrefabs = prefabs;
            _wrapperParent = wrapperParent;
        }

        public EntityWrapper Get(string type)
        {
            if (!_entityPrefabs.ContainsKey(type))
            {
                Debug.LogWarning("PREFAB NOT IMPLEMENTED");
                return null;
            }
            
            if (!_entityPool.TryGetValue(type, out var queue))
                _entityPool[type] = queue = new Queue<EntityWrapper>();

            if (queue.Count > 0)
                return queue.Dequeue();

            var wrp = Object.Instantiate(_entityPrefabs[type], _wrapperParent);
            wrp.Type = type;
            return wrp;
        }

        public void Return(EntityWrapper wrapper)
        {
            if (wrapper == null)
            {
                Debug.LogWarning("CAN NOT RETURN NULL WRAPPER");
                return;
            }

            wrapper.OnCleanup();

            _entityPool[wrapper.Type].Enqueue(wrapper);
        }
    }
}