using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public static class MapGimmickMethodBook
{
    public static MethodInfo GetMapGimmickMethod(string methodName)
    {
        if (string.IsNullOrEmpty(methodName)) { return null; }
        return typeof(MapGimmickMethodBook).GetMethod(methodName);
    }

    public static List<Tile> mTargetTileList = new List<Tile>();

    #region Gimmick Check

    /// <summary>
    /// 활동력 소모 카운트에 따라
    /// </summary>
    public static bool MoveCountCheck(MapGimmickInfo info)
    {
        Debug.Log("MoveCountCheck");
        bool result = info.IsExcute_MoveUse;
        if (result)
        {
            info.MoveUseCount = 0;
        }
        return result;
    }

    /// <summary>
    /// 아이템 사용 횟수에 따라
    /// </summary>
    public static bool ItemUseCountCheck(MapGimmickInfo info)
    {
        Debug.Log("ItemUseCountCheck");
        bool result = info.IsExcute_ItemUse;
        if (result)
        {
            info.ItemUseCount = 0;
        }
        return result;
    }

    /// <summary>
    /// 일정 손님 수에 따라
    /// </summary>
    public static bool OrderCountCheck(MapGimmickInfo info)
    {
        Debug.Log("OrderCountCheck");
        bool result = info.IsExcute_OrderClear;
        if (result)
        {
            info.OrderClearCount = 0;
        }
        return result;
    }

    #endregion

    #region Gimmick

    /*
    블록 설치, 타일 기믹 설치,
    타일 Hit, 블록 제거, 블록 교환, Copy도 설치의 일종,
    중력 방향 변경

    개수나 Info자체를 받아서 처리해야할듯

    몇 까지 겹치는데 이런 이유로 Type으로 넘겨서 생성하고 이런게 해도 굳이
    효과적인것 같진 않아서 또한 생성 이펙트나 이런것도 다 따로 필요하니깐
    일단은 효과별로 함수를 새로 만들자
     */

    public static void Add_VineBlock(MapGimmickInfo info)
    {
        // 설치후 바로 증식하는지 확인 필요
        Debug.Log("Add_VineBlock");

        // Mission Info에 Count 만큼 반복해야함
        TileMapManager.Instance.GetRandomNormalTileList(mTargetTileList, info.GimickCount);

        if (mTargetTileList.Count < 0) { return; }
        for (int cnt = 0; cnt < mTargetTileList.Count; ++cnt)
        {
            System.Type blockType = typeof(VineBlock);
            int blockColor = -1;
            int blockHp = 1;

            mTargetTileList[cnt].RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mTargetTileList[cnt], blockType, blockColor, blockHp);
        }

        //바인블럭은 새로 설치해도 매치가 발생 안한다.
        //info.ENextGameState = EGameState.MatchCheck;
    }

    public static void Add_RopeBlock(MapGimmickInfo info)
    {

    }

    public static void Add_IceTile(MapGimmickInfo info)
    {

    }

    public static void Excute_Meteor(MapGimmickInfo info)
    {

    }

    public static void Excute_LineHit(MapGimmickInfo info)
    {

    }

    public static void Excute_BlockRemove(MapGimmickInfo info)
    {

    }

    public static void Excute_BlockPosSwap(MapGimmickInfo info)
    {

    }

    public static void Excute_BlockCopy(MapGimmickInfo info)
    {

    }

    public static void Excute_DropDirChange(MapGimmickInfo info)
    {

    }

    #endregion
}
