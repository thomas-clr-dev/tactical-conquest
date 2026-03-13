using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOEventTest", menuName = "Scriptable Objects/SOEventTest")]
public class SOEventTest : ScriptableObject
{
    public List<ISOEventListener> Listeners = new List<ISOEventListener>();

    public void Register(ISOEventListener Subscriber)
    {
        Listeners.Add(Subscriber);
    }

    public void Unregister(ISOEventListener Subscriber)
    {
        Listeners.Remove(Subscriber);
    }

    public void Raise()
    {
        Listeners.ForEach(x => x.OnEventRaise());
    }
}
