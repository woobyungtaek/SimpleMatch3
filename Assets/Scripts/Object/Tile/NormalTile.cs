using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTile : Tile
{
    [SerializeField] private List<Tile> upLinkTileList = new List<Tile>();
    [SerializeField] private List<Tile> downLinkTileList = new List<Tile>();
    [SerializeField] private List<Tile> aroundTileList = new List<Tile>();

    public void CreateUpDownLinkAndAroundSquareList()
    {
        Tile checkTileOrNull;

        checkTileOrNull = RecieveTileList[0];
        for (int upCnt = 0; upCnt < 400; upCnt++)
        {
            if (upLinkTileList.Contains(checkTileOrNull)) { continue; }
            if (checkTileOrNull == null) { break; }

            upLinkTileList.Add(checkTileOrNull);

            if (!(checkTileOrNull is NormalTile)) { break; }
            checkTileOrNull = checkTileOrNull.RecieveTileList[0];
        }

        checkTileOrNull = SendTileList[0];
        for (int downCnt = 0; downCnt < 400; downCnt++)
        {
            if (downLinkTileList.Contains(checkTileOrNull)) { continue; }
            if (checkTileOrNull == null) { break; }

            downLinkTileList.Add(checkTileOrNull);

            if (!(checkTileOrNull is NormalTile)) { break; }
            checkTileOrNull = checkTileOrNull.SendTileList[0];
        }

        TileMapManager.Instance.CreateTileListIncludeNullByAreaList(aroundTileList, Coordi, TileArea.smallcross);
    }
    public bool IsFlowable()
    {
        int loopCount;
        Tile checkTile;

        loopCount = upLinkTileList.Count;
        for (int upIndex = 0; upIndex < loopCount; upIndex++)
        {
            checkTile = upLinkTileList[upIndex];
            if (checkTile.BlockContainerOrNull != null)
            {
                if (checkTile.BlockContainerOrNull.IsMove) { return false; }
            }
            else
            {
                if (checkTile is BlankTile) { return true; }
                if (checkTile.IsCreateTile) { return false; }
            }
        }

        loopCount = downLinkTileList.Count;
        for (int downIndex = 0; downIndex < loopCount; downIndex++)
        {
            checkTile = downLinkTileList[downIndex];
            if (!(checkTile is NormalTile)) { return true; }
            if (checkTile.BlockContainerOrNull == null) { return false; }
            if (checkTile.SendTileOrNull != null) { return false; }
        }
        return true;
    }
    public override void CheckAvailableSendTile()
    {
        if (BlockContainerOrNull == null) { return; }
        if (!BlockContainerOrNull.IsMove) { return; }

        int sendOrderIndex = -1;
        int loopCount;
        NormalTile checkNormalTile;

        loopCount = downLinkTileList.Count;
        for (int downIndex = 0; downIndex < loopCount; downIndex++)
        {
            if (!(downLinkTileList[downIndex] is NormalTile)) { break; }
            if (downLinkTileList[downIndex].BlockContainerOrNull != null) { continue; }
            sendOrderIndex = 0;
            break;
        }
        if (sendOrderIndex == -1)
        {
            for (int sendIndex = 1; sendIndex < SendTileList.Count; sendIndex++)
            {
                checkNormalTile = SendTileList[sendIndex] as NormalTile;

                if (checkNormalTile == null) { continue; }
                if (checkNormalTile.BlockContainerOrNull != null) { continue; }
                if (!(checkNormalTile.IsFlowable())) { continue; }
                if (sendOrderIndex == -1)
                {
                    sendOrderIndex = sendIndex;
                    continue;
                }
            }
        }
        if (sendOrderIndex == -1) { return; }

        SendTileOrNull = SendTileList[sendOrderIndex];
        if (sendOrderIndex == 0)
        {
            RequestBlockContainerToUpTileInternal();
        }
    }
    public override void HitTile(bool bExplosionHit)
    {
        if(IsHit == true) { return; }
        if(BlockContainerOrNull== null) { return; }

        IsHit = true;
        BlockContainerOrNull.HitBlockContainer(this, bExplosionHit);
    }

    public void CheckChainTileReculsive(int blockNum, HashSet<Tile> checkHashSet, List<Tile> chainList)
    {        
        if (checkHashSet.Contains(this)) { return; }

        int loopCount;
        checkHashSet.Add(this);
        chainList.Add(this);

        loopCount = aroundTileList.Count;
        for (int index =0; index < loopCount; index++)
        {
            if (aroundTileList[index] == null) { continue; }
            if (!(aroundTileList[index] is NormalTile)) { continue; }
            if (aroundTileList[index].BlockContainerOrNull == null) { continue; }
            if (aroundTileList[index].BlockContainerOrNull.BlockContainerNumber != blockNum) { continue; }
            (aroundTileList[index] as NormalTile).CheckChainTileReculsive(blockNum, checkHashSet, chainList);
        }
    }
    public Tile GetSameNumberTileOrNullByDir(int dir)
    {
        if (!(aroundTileList[dir] is NormalTile)) { return null; }
        if (aroundTileList[dir].BlockContainerOrNull == null) { return null; }
        if (aroundTileList[dir].BlockContainerOrNull.BlockContainerNumber == -1) { return null; }
        if (BlockContainerOrNull.BlockContainerNumber != aroundTileList[dir].BlockContainerOrNull.BlockContainerNumber) { return null; }
        return aroundTileList[dir];
    }
    public Tile GetAroundTileByDir(int dir)
    {
        if(aroundTileList[dir] is NormalTile)
        {
            return aroundTileList[dir];
        }
        return null;
    }

    private void RequestBlockContainerToUpTileInternal()
    {
        Tile recieveTile;
        int loopCount = RecieveTileList.Count;
        int linkCount;
        for (int recieveIndex = 0; recieveIndex < loopCount; recieveIndex++)
        {
            recieveTile = RecieveTileList[recieveIndex];
            if (recieveTile == null) { continue; }
            if (recieveTile is NormalTile)
            {
                if (recieveTile.BlockContainerOrNull == null)
                {
                    if (recieveIndex != 0) { continue; }
                    linkCount = upLinkTileList.Count;
                    for(int linkIndex =0; linkIndex < linkCount;linkIndex++) //foreach (Tile upTile in upLinkTileList)
                    {
                        if (upLinkTileList[linkIndex].BlockContainerOrNull != null)
                        {
                            if (upLinkTileList[linkIndex].BlockContainerOrNull.IsMove) { return; }
                        }
                        else
                        {
                            if (upLinkTileList[linkIndex] is BlankTile) { continue; }
                            else if (upLinkTileList[linkIndex] is EmptyTile) { continue; }
                            if (upLinkTileList[linkIndex].IsCreateTile) { return; }
                        }
                    }
                    continue;
                }
                else
                {
                    if (recieveTile.SendTileOrNull != null)
                    {
                        int orderA = recieveTile.SendTileList.IndexOf(recieveTile.SendTileOrNull);
                        int orderB = recieveTile.SendTileList.IndexOf(this);
                        if (orderA < orderB) { continue; }
                        else if (orderA.Equals(orderB)) { return; }
                    }
                }
            }
            else
            {
                if (!recieveTile.IsCreateTile) { continue; }
            }

            if (recieveIndex != 0 && recieveTile is NormalTile)
            {
                if (recieveTile.SendTileOrNull != null) { }
            }

            recieveTile.SendTileOrNull = this;
            return;
        }
    }

    protected override void SetSendTile(Tile checkTileOrNull)
    {
        base.SetSendTile(checkTileOrNull);
        if (SendTileOrNull != null) { RequestBlockContainerToUpTileInternal(); }
    }
    public override void Dispose()
    {
        upLinkTileList.Clear();
        downLinkTileList.Clear();
        aroundTileList.Clear();
        base.Dispose();
    }
}
