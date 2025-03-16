
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class MessageSystem : SingletonViaPrefab<MessageSystem>
{
    private Dictionary<MessageSystemEvent, UnityEvent> _eventDictionary;
    private Dictionary<MessageSystemEvent, UnityEvent<float>> _floatEventDictionary;
    private Dictionary<MessageSystemEvent, UnityEvent<GameObject>> _gameObjectEventDictionary;
    private Dictionary<MessageSystemEvent, UnityEvent<string>> _stringEventDictionary;

    protected override void InitializeSingleton()
    {
        if (_eventDictionary == null)
        {
            _eventDictionary = new();
        }
        if (_floatEventDictionary == null)
        {
            _floatEventDictionary = new();
        }
        if (_gameObjectEventDictionary == null)
        {
            _gameObjectEventDictionary = new();
        }
        if (_stringEventDictionary == null)
        {
            _stringEventDictionary = new();
        }
    }

    public static void StartListening(MessageSystemEvent eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance._eventDictionary.Add(eventName, thisEvent);
        }
    }
    public static void StartListening(MessageSystemEvent eventName, UnityAction<float> listener)
    {
        UnityEvent<float> thisEvent = null;
        if (Instance._floatEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<float>();
            thisEvent.AddListener(listener);
            Instance._floatEventDictionary.Add(eventName, thisEvent);
        }
    }
    public static void StartListening(MessageSystemEvent eventName, UnityAction<GameObject> listener)
    {
        UnityEvent<GameObject> thisEvent = null;
        if (Instance._gameObjectEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<GameObject>();
            thisEvent.AddListener(listener);
            Instance._gameObjectEventDictionary.Add(eventName, thisEvent);
        }
    }
    public static void StartListening(MessageSystemEvent eventName, UnityAction<string> listener)
    {
        UnityEvent<string> thisEvent = null;
        if (Instance._stringEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<string>();
            thisEvent.AddListener(listener);
            Instance._stringEventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(MessageSystemEvent eventName, UnityAction listener)
    {
        if (Instance == null)
            return;
        UnityEvent thisEvent = null;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(MessageSystemEvent eventName, UnityAction<float> listener)
    {
        if (Instance == null)
            return;
        UnityEvent<float> thisEvent = null;
        if (Instance._floatEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(MessageSystemEvent eventName, UnityAction<GameObject> listener)
    {
        if (Instance == null)
            return;
        UnityEvent<GameObject> thisEvent = null;
        if (Instance._gameObjectEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    public static void StopListening(MessageSystemEvent eventName, UnityAction<string> listener)
    {
        if (Instance == null)
            return;
        UnityEvent<string> thisEvent = null;
        if (Instance._stringEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void FireEvent(MessageSystemEvent eventName)
    {
        Debug.Log("want to fire event " + eventName);
        UnityEvent thisEvent = null;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
            Debug.Log($"Firing event {eventName}");
        }
    }
    /*
     * Every Event fires additionally the parameterless Event
     */

    public static void FireEvent(MessageSystemEvent eventName, float value)
    {
        UnityEvent<float> thisEvent = null;
        if (Instance._floatEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(value);
            //Debug.Log($"Firing event {eventName} with float {value.ToString("F4")}");
        }
        FireEvent(eventName);
    }
    public static void FireEvent(MessageSystemEvent eventName, GameObject value)
    {
        UnityEvent<GameObject> thisEvent = null;
        if (Instance._gameObjectEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(value);
            //Debug.Log($"Firing event {eventName} with gameObject {value.name}", value);
        }
        FireEvent(eventName);
    }
    public static void FireEvent(MessageSystemEvent eventName, string value)
    {
        UnityEvent<string> thisEvent = null;
        if (Instance._stringEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(value);
            Debug.Log($"Firing event {eventName} with string value {value}");
        }
        FireEvent(eventName);
    }

}
/// <summary>
/// Enum for Events
/// ALWAYS write the enums in caps, such that we can negate that mistake from ink to enum
/// Only add numbers higher then each other in a region.
/// </summary>
public enum MessageSystemEvent
{
    #region Quests
    //start with 000 here!
    StartErsteSamen = 000,
    FinishErsteSamen = 001,
    BuyFeminizedSeed = 002,
    ObjectiveFinished = 003,
    #endregion

    #region Other
    //start with 100 here!
    #endregion

    #region UIEvents
    //start with 200 here!
    InventoryUpdated = 200,
    #endregion
}
