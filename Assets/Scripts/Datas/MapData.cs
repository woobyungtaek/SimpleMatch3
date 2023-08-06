using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapLoadData
{
    public Vector2Int mapSize;

    public int moveCount;
    public Vector2Int gravity;

    public List<MissionData_Element> missionList;
    public List<BlockMaker> blockMakerList;

    public string tutoName;

    public string tileStr;
    public string blockStr;

    public void Copy(MapData data)
    {
        data.mapSize = mapSize;

        data.moveCount = moveCount;
        data.gravity = gravity;

        data.missionList.Clear();
        for(int index =0; index < missionList.Count; ++index)
        {
            data.missionList.Add(missionList[index]);
        }
        data.blockMakerList.Clear();
        for (int index = 0; index < blockMakerList.Count; ++index)
        {
            data.blockMakerList.Add(blockMakerList[index]);
        }
        data.tutoName = tutoName;
        data.tileStr = tileStr;
        data.blockStr = blockStr;
    }
}
