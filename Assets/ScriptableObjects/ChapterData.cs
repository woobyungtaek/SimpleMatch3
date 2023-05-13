using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterData", menuName = "ScriptableObjects/ChapterData")]
public class ChapterData : ScriptableObject
{
    public int PartCount { get => MissionLevelList.Count; }
    public int GetStageCount(int partIdx) { return MissionLevelList[partIdx].Count; }


    [SerializeField] public List<PartData> MissionLevelList = new List<PartData>(3);
}

[System.Serializable]
public class PartData
{
    public int Count { get => list.Count; }
    [SerializeField] public List<EMissionLevel> list = new List<EMissionLevel>();
}
