using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SOEvents
{

    [CreateAssetMenu(fileName = "SOEvent", menuName = "SOEvent/SOEvent", order = 0)]
    public class SOEvent : ScriptableObject
    {
        public UnityEvent objectEvent = new UnityEvent();

        // Contains all the listeners that are currently active on the specific Event.
        // Only listenerMethodNames is visible. Can be used for debugging.
        public CurrentListeners currentListeners = new CurrentListeners();

        public void AddListener(UnityAction call)
        {
            currentListeners.AddListener(call);
            objectEvent.AddListener(call);
        }

        public void RemoveListener(UnityAction call) 
        {
            objectEvent.RemoveListener(call);
            currentListeners.RemoveListener(call);
        }

        public void RemoveAllListeners() 
        {
            objectEvent.RemoveAllListeners();
            currentListeners.Clear();
        }

        public void Invoke()
        {
            objectEvent.Invoke();
        }
    }

    [CreateAssetMenu(fileName = "SOEvent", menuName = "SOEvent/SOEvent", order = 1)]
    public class SOEvent<T> : ScriptableObject
    {

        public UnityEvent<T> objectEvent = new UnityEvent<T>();

        public CurrentListeners<T> currentListeners = new CurrentListeners<T>();

        // Optional string that can be used to indicate what the argument provided should be e.g. Players Body
        [TextArea(minLines:1, maxLines:5)] public string argument1Description;
        
        public void AddListener(UnityAction<T> call)
        {
            currentListeners.AddListener(call);
            objectEvent.AddListener(call);
        }

        public void RemoveListener(UnityAction<T> call) 
        {
            objectEvent.RemoveListener(call);
            currentListeners.RemoveListener(call);
        }

        public void RemoveAllListeners() 
        {
            objectEvent.RemoveAllListeners();
            currentListeners.Clear();
        }

        public void Invoke(T arg)
        {
            objectEvent.Invoke(arg);
        }
    }

    [CreateAssetMenu(fileName = "SOEvent", menuName = "SOEvent/SOEvent", order = 2)]
    public class SOEvent<T, W> : ScriptableObject
    {
        public UnityEvent<T, W> objectEvent = new UnityEvent<T, W>();
        
        public CurrentListeners<T, W> currentListeners = new CurrentListeners<T, W>();

        // Optional string that can be used to indicate what the argument provided should be e.g. Player's Body and Enemy's Body.
        [TextArea(minLines:1, maxLines:5)] public string argument1Description;
        [TextArea(minLines:1, maxLines:5)] public string argument2Description;

        public void AddListener(UnityAction<T, W> call)
        {
            currentListeners.AddListener(call);
            objectEvent.AddListener(call);
        }

        public void RemoveListener(UnityAction<T, W> call) 
        {
            objectEvent.RemoveListener(call);
            currentListeners.RemoveListener(call);
        }

        public void RemoveAllListeners() 
        {
            objectEvent.RemoveAllListeners();
            currentListeners.Clear();
        }

        public void Invoke(T arg1, W arg2)
        {
            objectEvent.Invoke(arg1, arg2);
        }
    }

    [CreateAssetMenu(fileName = "SOEvent", menuName = "SOEvent/SOEvent", order = 3)]
    public class SOEvent<T, V, W> : ScriptableObject
    {
        public UnityEvent<T, V, W> objectEvent = new UnityEvent<T, V, W>();
        
        public CurrentListeners<T, V, W> currentListeners = new CurrentListeners<T, V, W>();

        // Optional string that can be used to indicate what the argument provided should be e.g. Player's Body, Enemy's Body, and Current Time
        [TextArea(minLines:1, maxLines:5)] public string argument1Description;
        [TextArea(minLines:1, maxLines:5)] public string argument2Description;
        [TextArea(minLines:1, maxLines:5)] public string argument3Description;

        public void AddListener(UnityAction<T, V, W> call)
        {
            currentListeners.AddListener(call);
            objectEvent.AddListener(call);
        }

        public void RemoveListener(UnityAction<T, V, W> call) 
        {
            objectEvent.RemoveListener(call);
            currentListeners.RemoveListener(call);
        }

        public void RemoveAllListeners() 
        {
            objectEvent.RemoveAllListeners();
            currentListeners.Clear();
        }

        public void Invoke(T arg1, V arg2, W arg3)
        {
            objectEvent.Invoke(arg1, arg2, arg3);
        }
    }



    [System.Serializable]
    public class CurrentListeners
    {
        List<UnityAction> listeners = new List<UnityAction>();

        public List<ListenerInfoStrings> listenersInfo = new List<ListenerInfoStrings>();

        public void AddListener(UnityAction call)
        {
            if (listeners.Count == 0) listenersInfo.Clear();
            if (listeners.Contains(call)) return;

            listeners.Add(call);
            listenersInfo.Add(new ListenerInfoStrings(call.Target.ToString(), call.Method.Name));
        }

        public void RemoveListener(UnityAction call) 
        {
            listenersInfo.RemoveAll(listener => listener.listenerClassName == call.Target.ToString() &&
             listener.listenerMethodName == call.Method.Name);
            listeners.Remove(call);
        }

        public void Clear() 
        {
            listeners.Clear();
            listenersInfo.Clear();
        }
    }



    [System.Serializable]
    public class CurrentListeners<T>
    {
        List<UnityAction<T>> listeners = new List<UnityAction<T>>();

        public List<ListenerInfoStrings> listenersInfo = new List<ListenerInfoStrings>();

        public void AddListener(UnityAction<T> call)
        {
            if (listeners.Count == 0) listenersInfo.Clear();
            if (listeners.Contains(call)) return;

            listeners.Add(call);
            listenersInfo.Add(new ListenerInfoStrings(call.Target.ToString(), call.Method.Name));
        }

        public void RemoveListener(UnityAction<T> call) 
        {
            listenersInfo.RemoveAll(listener => listener.listenerClassName == call.Target.ToString() &&
             listener.listenerMethodName == call.Method.Name);
            listeners.Remove(call);
        }

        public void Clear() 
        {
            listeners.Clear();
            listenersInfo.Clear();
        }
    }


    [System.Serializable]
    public class CurrentListeners<T, W>
    {
        List<UnityAction<T, W>> listeners = new List<UnityAction<T, W>>();

        public List<ListenerInfoStrings> listenersInfo = new List<ListenerInfoStrings>();

        public void AddListener(UnityAction<T, W> call)
        {
            if (listeners.Count == 0) listenersInfo.Clear();
            if (listeners.Contains(call)) return;
            
            listeners.Add(call);
            listenersInfo.Add(new ListenerInfoStrings(call.Target.ToString(), call.Method.Name));
        }

        public void RemoveListener(UnityAction<T, W> call) 
        {
            listenersInfo.RemoveAll(listenerInfoString => listenerInfoString.listenerClassName == call.Target.ToString() &&
             listenerInfoString.listenerMethodName == call.Method.Name);
            listeners.Remove(call);
        }

        public void Clear() 
        {
            listeners.Clear();
            listenersInfo.Clear();
        }
    }


    [System.Serializable]
    public class CurrentListeners<T, V, W>
    {
        List<UnityAction<T, V, W>> listeners = new List<UnityAction<T, V, W>>();

        public List<ListenerInfoStrings> listenersInfo = new List<ListenerInfoStrings>();

        public void AddListener(UnityAction<T, V, W> call)
        {
            if (listeners.Count == 0) listenersInfo.Clear();
            if (listeners.Contains(call)) return;

            listeners.Add(call);
            listenersInfo.Add(new ListenerInfoStrings(call.Target.ToString(), call.Method.Name));
        }

        public void RemoveListener(UnityAction<T, V, W> call) 
        {
            
            listenersInfo.RemoveAll(listener => listener.listenerClassName == call.Target.ToString() &&
             listener.listenerMethodName == call.Method.Name);
            listeners.Remove(call);
        }

        public void Clear() 
        {
            listeners.Clear();
            listenersInfo.Clear();
        }
    }
}


[System.Serializable]
public class ListenerInfoStrings
{
    public string listenerClassName;
    public string listenerMethodName;

    public ListenerInfoStrings(string listenerClassName, string listenerMethodName)
    {
        this.listenerClassName = listenerClassName;
        this.listenerMethodName = listenerMethodName;
    }
}