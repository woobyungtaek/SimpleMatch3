using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Vector2Comparer : IEqualityComparer<Vector2>
{
    public bool Equals(Vector2 x, Vector2 y)
    {
        return x.x == y.x && x.y == y.y;
    }

    public int GetHashCode(Vector2 obj)
    {
        return obj.GetHashCode();
    }
}

public class IntComparer : IEqualityComparer<int>
{
    public bool Equals(int x, int y)
    {
        return x == y;
    }

    public int GetHashCode(int obj)
    {
        return obj.GetHashCode();
    }
}