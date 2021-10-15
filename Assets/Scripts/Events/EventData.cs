using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class type that allows for the creation of event datas
public class EventData
{
    public readonly EventType eventType;

    public EventData(EventType type)
    {
        eventType = type;
    }
}
