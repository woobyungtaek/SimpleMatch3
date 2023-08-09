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
    public string MissionName;
    public int MissionColor;
    public int MissionCount;
}

[System.Serializable]
public class MissionDataPreset
{
    public int Index;
    public EAreaType Area;
    public EMissionLevel Level;
    public bool IsTemplate;

    public List<MissionData_Element> MissionList;

    private MissionData_Element mCurrentMissionDataElement;

    // CSV 리더가 사용
    public string Block_Name
    {
        set
        {
            if (MissionList == null) { MissionList = new List<MissionData_Element>(); }
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

[System.Serializable]
public class MissionData : Pool<MissionData>
{
    private static int[] ColorArr = { 0, 1, 2, 3, 4 };

    public EAreaType Area;
    public EMissionLevel Level;
    public List<MissionInfo> MissionInfoList = new List<MissionInfo>();

    public void Init(MissionDataPreset presetData)
    {
        Area = presetData.Area;
        Level = presetData.Level;
        CreateMissionInfoListByDataPreset(presetData);
    }

    public void ClearMissionInfoList()
    {
        int loopCount = MissionInfoList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            MissionInfo.Destroy(MissionInfoList[index]);
        }
        MissionInfoList.Clear();
    }
    private void ShuffleColorArr()
    {
        int shuffleCount = UnityEngine.Random.Range(3, 10);
        for (int count = 0; count < shuffleCount; ++count)
        {
            int aIdx = UnityEngine.Random.Range(0, 5);
            int bIdx = UnityEngine.Random.Range(0, 5);

            int temp = ColorArr[aIdx];
            ColorArr[aIdx] = ColorArr[bIdx];
            ColorArr[bIdx] = temp;
        }

        //Debug.Log($"{ColorArr[0]} /{ColorArr[1]} /{ColorArr[2]} /{ColorArr[3]} /{ColorArr[4]}");
    }
    private void CreateMissionInfoListByDataPreset(MissionDataPreset preset)
    {
        ClearMissionInfoList();

        // Template일때
        if (preset.IsTemplate)
        {
            // ColorArr 섞기
            ShuffleColorArr();
        }

        for (int idx = 0; idx < preset.MissionList.Count; ++idx)
        {
            var element = preset.MissionList[idx];
            if (preset.IsTemplate && element.MissionColor != -1)
            {
                // MissionColor가 인덱스로 사용된다.
                element.MissionColor = ColorArr[element.MissionColor];
            }
            var info = MissionInfo.Instantiate();
            info.InitMissionInfo(element);

            MissionInfoList.Add(info);
        }
    }


    public void AddMissionInfoForce(int index, MissionInfo missionInfo)
    {
        if(MissionInfoList.Count <= index)
        {
            MissionInfoList.Add(missionInfo);
            return;
        }

        MissionInfo.Destroy(MissionInfoList[index]);
        MissionInfoList[index] = missionInfo;
    }
}

public class MissionInfo : Pool<MissionInfo>
{
    //MissionData가 복사되어 들어가는 일종의 슬롯이다.
    private static string SpriteFieldName = "spriteString";

    public Type MissionType { get => mMissionType; }
    public String MissionSpriteName { get => mSpriteName; }
    public int MissionColor { get => mMissionColor; set => mMissionColor = value; }
    public int MissionCount
    {
        get => mMissionCount;
        set
        {
            mMissionCount = value;
            if (mMissionCount < 0) { mMissionCount = 0; }
        }
    }

    private Type mMissionType;
    private int mMissionColor;
    private int mMissionCount;

    private string mSpriteName;

    public void InitMissionInfo(Type missionType, int color, int count)
    {
        mMissionType = missionType;
        mMissionColor = color;
        mMissionCount = count;

        // SpriteName을 가져오는 부분 부터 너무 분산되어있다.
        // IMissionTarget을 만들고 타겟이 될 수 있는 Class에 붙여서 강제로 입력가능하게 하자

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
