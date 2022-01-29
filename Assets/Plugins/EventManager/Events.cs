using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventManager 
{
    public class Events : MonoBehaviour 
    {
        // private static Events instance;
        // public static Events Instance => instance ? instance : instance = FindObjectOfType<Events>();

        // GLOBAL EVENTS
        public delegate void GlobalGameEvent(object origin, EventArgs eventArgs);
        private static Dictionary<Flag, GlobalGameEvent> eventListeners = new Dictionary<Flag, GlobalGameEvent>();
        private static Dictionary<(Flag, object), GlobalGameEvent> objectEventListeners = new Dictionary<(Flag, object), GlobalGameEvent>();

        public static void TriggerEvent(Flag eventFlag, object origin, EventArgs eventArgs = null, bool networked = false)
        {
            if (eventListeners.TryGetValue(eventFlag, out GlobalGameEvent globalEvent))
                globalEvent?.Invoke(origin, eventArgs);
            if (objectEventListeners.TryGetValue((eventFlag, origin), out GlobalGameEvent objectEvent))
                objectEvent?.Invoke(origin, eventArgs);
        }

        /// <summary>
        /// Listens for any triggering of the event
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="callback"></param>
        public static void AddListener(Flag flag, GlobalGameEvent callback)
        {
            if (eventListeners.ContainsKey(flag) == false)
                eventListeners.Add(flag, null);
            eventListeners[flag] += callback;
        }

        /// <summary>
        /// Only listens for specific origin
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="origin"></param>
        /// <param name="callback"></param>
        public static void AddListener(Flag flag, object origin, GlobalGameEvent callback)
        {
            if (objectEventListeners.ContainsKey((flag, origin)) == false)
                objectEventListeners.Add((flag, origin), null);
            objectEventListeners[(flag, origin)] += callback;
        }

        public static void RemoveListener(Flag flag, GlobalGameEvent callback)
        {
            if (eventListeners.ContainsKey(flag))
                eventListeners[flag] -= callback;
        }

        public static void RemoveListener(Flag flag, object origin, GlobalGameEvent callback)
        {
            if (objectEventListeners.ContainsKey((flag, origin)))
                objectEventListeners[(flag, origin)] -= callback;
        }
    }
}