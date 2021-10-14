using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CostCalculator
{
    public static int GetBasicRewardMoveCount(int totalCost)
    {        
        return (int)Mathf.Sqrt(3 * totalCost);
    }
}
