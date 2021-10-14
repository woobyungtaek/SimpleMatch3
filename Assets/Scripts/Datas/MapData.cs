using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    public Vector2Int mapSize;

    public int moveCount;
    public Vector2Int gravity;

    public List<MissionData> missionList;
    public List<BlockMaker> blockMakerList;

    public string tutoName;

    public string tileStr;
    public string blockStr;
}
