using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WBTTestClass : IWBTTestInterface
{
    public int Count { get; set; }

    public void IncreaseCount()
    {
        Count += 5;
        Debug.Log($"{Count}");
    }
}


public class WBTGenericTestClass<T>
{
    private List<T> mGenericTestList = new List<T>(3);

    public int Count
    {
        get => mGenericTestList.Capacity;
    }
}

public interface IWBTTestInterface
{
    int Count { get; set; }
}