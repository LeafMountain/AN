using Unity.Netcode;
using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(Actor))]
    public class ActorComponent : MonoBehaviour
    {
        [SerializeField]
        private Actor _actor;
        public Actor Parent { get => _actor; private set => _actor = value; }

        private void OnValidate()
        {
            _actor = GetComponent<Actor>();
        }
    }
    
    [RequireComponent(typeof(Actor))]
    public class NetworkActorComponent : NetworkBehaviour 
    {
        [SerializeField]
        private Actor _actor;
        public Actor Parent { get => _actor; private set => _actor = value; }

        private void OnValidate()
        {
            _actor = GetComponent<Actor>();
        }

        protected virtual void Start()
        {
        }
    }
}
