using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public struct MissionData
{
    public int      MissionCost;
    public string   MissionName;
    public int      MissionColor;
    public int      MissionCount;

    //미션 목표 자체를 프리셋으로 여러개 만들어 놓는다.
    //프리셋을 등급별로 구별하고
    //프리셋을 원하는 등급에 맞춰 조합하여 목표를 최종 결정한다.
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
    public override void Dispose() { }
}
