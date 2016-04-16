using Stump.Core.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stump.Core.Pool
{
    public class EntityManager<TSingleton, T, TC> : Singleton<TSingleton>
        where TSingleton : class
        where T : class
    {
        private readonly ConcurrentDictionary<TC, T> m_entities = new ConcurrentDictionary<TC, T>();

        public event Action<T> EntityAdded;

        public event Action<T> EntityRemoved;

        protected virtual void OnEntityAdded(T obj)
        {
            Action<T> entityAdded = this.EntityAdded;
            if (entityAdded != null)
            {
                entityAdded(obj);
            }
        }

        protected virtual void OnEntityRemoved(T obj)
        {
            Action<T> entityRemoved = this.EntityRemoved;
            if (entityRemoved != null)
            {
                entityRemoved(obj);
            }
        }

        protected void AddEntity(TC identifier, T entity)
        {
            if (!this.m_entities.TryAdd(identifier, entity))
            {
                throw new Exception(string.Format("Cannot add entity, identifier {0} may already exist", identifier));
            }
            this.OnEntityAdded(entity);
        }

        protected T RemoveEntity(TC identifier)
        {
            T t;
            if (!this.m_entities.TryRemove(identifier, out t))
            {
                throw new Exception(string.Format("Cannot remove entity, identifier {0} may not exist", identifier));
            }
            this.OnEntityRemoved(t);
            return t;
        }

        protected T GetEntityOrDefault(TC identifier)
        {
            T t;
            return (!this.m_entities.TryGetValue(identifier, out t)) ? default(T) : t;
        }

        protected T GetEntity(TC identifier)
        {
            T result;
            if (!this.m_entities.TryGetValue(identifier, out result))
            {
                throw new KeyNotFoundException(string.Format("Entity with identifier {0} not found", identifier));
            }
            return result;
        }
    }

    public class EntityManager<TSingleton, T> : EntityManager<TSingleton, T, int>
        where TSingleton : class
        where T : class
    {
    }
}