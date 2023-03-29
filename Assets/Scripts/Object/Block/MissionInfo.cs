using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum EAreaType
{
    All = -1,
    Stage1,
    Stage2,
    Stage3
}

public enum EMissionLevel
{
    VeryEasy = 0,
    Easy,
    Normal,
    Hard,
    VeryHard,
    Max
}

[System.Serializable]
public struct MissionData_Element
{
    public string   MissionName;
    public int      MissionColor;
    public int      MissionCount;
}

public class MissionData
{
    public int           Index;
    public EAreaType     Area;
    public EMissionLevel Level;

    public List<MissionData_Element> MissionList;

    private MissionData_Element mCurrentMissionDataElement;

    public string Block_Name
    {
        set 
        {
            if(MissionList == null) { MissionList = new List<MissionData_Element>(); }

            mCurrentMissionDataElement = new MissionData_Element();
            mCurrentMissionDataElement.MissionName = value;    
        }
    }
    public int Block_Color
    {
        set
        {
            mCurrentMissionDataElement.MissionColor = value;
        }
    }
    public int Block_Count
    {
        set
        {
            mCurrentMissionDataElement.MissionCount = value;
            MissionList.Add(mCurrentMissionDataElement);
        }
    }
}

public class MissionInfo : Pool<MissionInfo>
{
    //MissionData가 복사되어 들어가는 일종의 슬롯이다.
    private static string SpriteFieldName = "spriteString";

    public Type     MissionType         { get => mMissionType; }
    public String   MissionSpriteName   { get => mSpriteName; }
    public int      MissionColor        { get => mMissionColor; }
    public int      MissionCount
    {
        get => mMissionCount;
        set
        {
            mMissionCount = value;
            if (mMissionCount < 0) { mMissionCount = 0; }
        }
    }

    private Type    mMissionType;
    private int     mMissionColor;
    private int     mMissionCount;

    private string  mSpriteName;

    public void InitMissionInfo(Type missionType, int color, int count)
    {
        mMissionType = missionType;
        mMissionColor = color;
        mMissionCount = count;

        mSpriteName = (string)missionType.GetField(SpriteFieldName).GetValue(null);
        mSpriteName = string.Format(mSpriteName, mMissionColor);
    }
    public void InitMissionInfo(string missionName, int color, int count)
    {
        InitMissionInfo(Type.GetType(missionName), color, count);
    }
    public void InitMissionInfo(MissionData_Element element)
    {
        InitMissionInfo(Type.GetType(element.MissionName), element.MissionColor, element.MissionCount);
    }

    public override void Dispose() { }
}
