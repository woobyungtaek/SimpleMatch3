using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialData : System.IDisposable
{
    public int TutoIndex;
    public int HammerCount;
    public int RandBoxCount;
    public List<TutoStepData> tutoStepList;
    public List<MissionData_Element>  missionList;

    public void Dispose()
    {
        HammerCount = 0;
        RandBoxCount = 0;
        tutoStepList.Clear();
    }
}
