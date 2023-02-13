using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WBTExtension 
{
    public static void CommonLog(this WBTTestClass testclass)
    {
        Debug.Log("TestLog");
    }

    public static void GenericTestMethod<T>(this WBTGenericTestClass<T> testGenericClass)
    {
        Debug.Log($"{testGenericClass.Count}");
    }

    public static void GenericTestMethodInt(this WBTGenericTestClass<int> testGenericIntClass)
    {

    }

    public static void IncreaseCount(this IWBTTestInterface testInterface)
    {
        testInterface.Count++;
        Debug.Log($"{testInterface.Count}");
    }
}
