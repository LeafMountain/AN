using InventorySystem;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    public interface IActorComponent
    {
        public void OnParentInitialized();
    }
    
    [RequireComponent(typeof(Actor))]
    public abstract class ActorComponent : SerializedMonoBehaviour, IActorComponent
    {
        [SerializeField]
        private Actor _actor;
        public Actor Parent { get => _actor; private set => _actor = value; }

        private void OnValidate()
        {
            _actor = GetComponent<Actor>();
        }

        public virtual void OnParentInitialized()
        {
        }
    }
    
    [RequireComponent(typeof(Actor))]
    public abstract class NetworkActorComponent : NetworkBehaviour, IActorComponent
    {
        [SerializeField]
        private Actor _actor;
        public Actor Parent { get => _actor; private set => _actor = value; }
        public ActorHandle handle => Parent.handle;

        private void OnValidate()
        {
            _actor = GetComponent<Actor>();
        }

        protected virtual void Start()
        {
        }

        public virtual void OnParentInitialized()
        {
        }
    }
}
