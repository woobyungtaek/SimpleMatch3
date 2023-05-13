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
    /// Ȱ���� �Ҹ� ī��Ʈ�� ����
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
    /// ������ ��� Ƚ���� ����
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
    /// ���� �մ� ���� ����
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
    ��� ��ġ, Ÿ�� ��� ��ġ,
    Ÿ�� Hit, ��� ����, ��� ��ȯ, Copy�� ��ġ�� ����,
    �߷� ���� ����

    ������ Info��ü�� �޾Ƽ� ó���ؾ��ҵ�

    �� ���� ��ġ�µ� �̷� ������ Type���� �Ѱܼ� �����ϰ� �̷��� �ص� ����
    ȿ�����ΰ� ���� �ʾƼ� ���� ���� ����Ʈ�� �̷��͵� �� ���� �ʿ��ϴϱ�
    �ϴ��� ȿ������ �Լ��� ���� ������
     */

    public static void Add_VineBlock(MapGimmickInfo info)
    {
        // ��ġ�� �ٷ� �����ϴ��� Ȯ�� �ʿ�
        Debug.Log("Add_VineBlock");

        // Mission Info�� Count ��ŭ �ݺ��ؾ���
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

        //���κ��� ���� ��ġ�ص� ��ġ�� �߻� ���Ѵ�.
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
