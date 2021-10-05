using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Event that informs subscribers of a debug log
public class SendDebugLog : EventData
{
    public readonly string debuglog;

    public SendDebugLog(string givenLog) : base(EventType.ReceiveDebug)
    {
        debuglog = givenLog;
    }
}

//Event that informs subscribers of the quickslot being updated
public class UpdateQuickslotDisplay : EventData
{
    public UpdateQuickslotDisplay() : base(EventType.UpdateQuickslotDisplay)
    {
    }
}