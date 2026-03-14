using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour, ISOEventListener
{
    public UnityEvent<int> MyUnityEvent;

    public UnityAction<int> MyContourneur;

    public SOEventTest sOEvent;

    private void Awake()
    {
        sOEvent.Register(this);
        MyContourneur += ShowMyInt;

        MyUnityEvent.AddListener(MyContourneur);
    }
    
    public void OnEventRaise()
    {
        MyUnityEvent?.Invoke(974);
    }

    private void ShowMyInt(int myInt)
    {
        Utils.ColorLog($"{myInt}", "Cyan");
    }

    private void OnDestroy()
    {
        sOEvent.Unregister(this);
    }
}
