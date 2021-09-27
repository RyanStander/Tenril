using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DiplomacyStatementState : DiplomacyAbstractStateFSM
{
    public string GetRandomListString(List<string> stringList)
    {
        //Check for null or empty
        if (stringList == null || stringList.Count == 0)
        {
            return null;
        }
        else
        {
            return stringList[Random.Range(0, stringList.Count)];
        }
    }
}
