using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, System.IDisposable
{
    [Header("BasicData")]
    [SerializeField] private Vector2 mCoordi;
    [SerializeField] private Vector2 mGravity;
    [SerializeField] private bool mbCreateTile;
    [SerializeField] private bool mbReservationTile;
    [SerializeField] private bool mbDroping;
    [SerializeField] private bool mbHit;
    [SerializeField] private bool mbSplahHit;

    [SerializeField] private Tile mSendTile;
    [SerializeField] protected BlockContainer mBlockContainer;

    [Header("List")]
    [SerializeField] private List<Tile> mSendTileList = new List<Tile>();
    [SerializeField] private List<Tile> mRecieveTileList = new List<Tile>();

    [Header("BlockMaker")]
    private BlockMaker mMakerData;

    public Vector2 Coordi           { get => mCoordi;           set => mCoordi = value; }
    public Vector2 Gravity          { get => mGravity;          set => mGravity = value; }
    public bool IsCreateTile        { get => mbCreateTile;      set => mbCreateTile = value; }
    public bool IsReservationTile   { get=> mbReservationTile;  set=> mbReservationTile = value; }
    public bool IsDroping           { get => mbDroping;         set => mbDroping = value; }
    public bool IsHit               { get => mbHit;             set => mbHit = value; }
    public bool IsSplashHit         { get => mbSplahHit;        set => mbSplahHit = value; }

    public BlockContainer BlockContainerOrNull
    {
        get => mBlockContainer;
        set
        {
            mBlockContainer = value;
        }
    }

    public Tile SendTileOrNull {
        get => mSendTile;
        set => SetSendTile(value);
    }
    public List<Tile> SendTileList { get => mSendTileList; set => mSendTileList = value; }
    public List<Tile> RecieveTileList { get => mRecieveTileList; set => mRecieveTileList = value; }
    public BlockMaker BlockMaker { get => mMakerData; set => mMakerData = value; }

    public void CreateSendTileListByMapSize(Vector2 mapSize)
    {
        Vector2[] gravityDirArr = new Vector2[3];

        gravityDirArr[0] = mGravity;
        if (mGravity.x == 0)
        {
            if (Coordi.x <= mapSize.x / 2)
            {
                gravityDirArr[1] = mGravity + Vector2.right;
                gravityDirArr[2] = mGravity + Vector2.left;
            }
            else
            {
                gravityDirArr[1] = mGravity + Vector2.left;
                gravityDirArr[2] = mGravity + Vector2.right;
            }
        }
        else
        {
            if (Coordi.y <= mapSize.y / 2)
            {
                gravityDirArr[1] = mGravity + Vector2.down;
                gravityDirArr[2] = mGravity + Vector2.up;
            }
            else
            {
                gravityDirArr[1] = mGravity + Vector2.up;
                gravityDirArr[2] = mGravity + Vector2.down;
            }
        }

        Tile instTile;
        for (int dirIndex = 0; dirIndex < gravityDirArr.Length; dirIndex++)
        {
            instTile = TileMapManager.Instance.GetTileByCoordiOrNull(Coordi + gravityDirArr[dirIndex]);
            if (instTile is NormalTile) { SendTileList.Add(instTile); }
            else { SendTileList.Add(null); }
        }
    }
    public void SetRecieveTileListOfSendTileList()
    {
        int loopCount = SendTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (SendTileList[index] == null) { continue; }

            if (index == 0) { SendTileList[index].RecieveTileList.Insert(0, this); }
            else { SendTileList[index].RecieveTileList.Add(this); }

        }
    }
    public void AdjustRecieveTileListByMapSize(Vector2 mapSize)
    {
        if (RecieveTileList.Count < 3) { return; }

        Vector2 checkCoordi;
        Tile tempTile;

        if (mGravity.x == 0)
        {
            if (Coordi.x <= mapSize.x / 2) { checkCoordi = Vector2.left + Coordi; }
            else { checkCoordi = Vector2.right + Coordi; }
        }
        else
        {
            if (Coordi.y <= mapSize.y / 2) { checkCoordi = Vector2.down + Coordi; }
            else { checkCoordi = Vector2.up + Coordi; }
        }

        checkCoordi += (mGravity * -1);
        if (RecieveTileList[1].mCoordi == checkCoordi)
        {
            tempTile = RecieveTileList[1];
            RecieveTileList[1] = RecieveTileList[2];
            RecieveTileList[2] = tempTile;
        }
    }

    public void CreateBlockByCreateTileData()
    {
        //블록 생성기
        if(BlockMaker != null)
        {
            if (!BlockMaker.IsEnd)
            {
                BlockData blockData = BlockMaker.CreateBlockList[BlockMaker.CurrentIndex];
                BlockManager.Instance.CreateBlockByBlockDataInTile(this, blockData.BlockType, blockData.BlockColor, blockData.BlockHP, TileMapManager.Instance.TileParentTransform);
                BlockMaker.CurrentIndex += 1;
                if (BlockMaker.IsEnd)
                {
                    BlockMaker = null;
                }
                return;
            }
        }
        BlockManager.Instance.CreateBlockByBlockDataInTile(this, typeof(NormalBlock), Random.Range(0, 5),1, TileMapManager.Instance.TileParentTransform);       
    }
    public void ResetTileState()
    {
        SendTileOrNull = null;
        IsReservationTile = false;
        IsHit = false;
    }
    public void CheckBlockContainer()
    {
        if(BlockContainerOrNull == null) { return; }
        if(BlockContainerOrNull.BlockCount == 0)
        {
            BlockContainerOrNull = null;
        }
    }

    public void TestChangeBlockColor(Color color)
    {
        if(BlockContainerOrNull == null) { return; }
        BlockContainerOrNull.MainBlock.TestBlockColorChange(color);
    }
    public void RemoveBlockContainer()
    {
        if(BlockContainerOrNull == null) { return; }
        BlockContainerOrNull.RemoveAllBlock();
        GameObjectPool.Destroy(BlockContainerOrNull.gameObject);
        BlockContainerOrNull = null;
    }    

    public virtual void CheckAvailableSendTile() { }
    public virtual void HitTile(bool bExplosionHit) { }
    public virtual void Dispose()
    {
        BlockContainerOrNull = null;

        mbCreateTile = false;
        mbReservationTile = false;
        mbDroping = false;
        mbHit = false;
        mbSplahHit = false;

        mSendTile = null;
        mSendTileList.Clear();
        mRecieveTileList.Clear();
    }

    protected virtual void SetSendTile(Tile checkTileOrNull)
    {
        if (checkTileOrNull == null) { mSendTile = null; return; }
        if (checkTileOrNull.IsReservationTile) { return; }
        if (!IsCreateTile)
        {
            if (BlockContainerOrNull == null) { return; }
            if (BlockContainerOrNull.IsMove == false) { return; }
        }

        if (SendTileOrNull == null)
        {
            mSendTile = checkTileOrNull;
        }
        else
        {
            int currentOrder = mSendTileList.IndexOf(mSendTile);
            int checkOrder = mSendTileList.IndexOf(checkTileOrNull);

            if (currentOrder > checkOrder)
            {
                mSendTile = checkTileOrNull;
            }
        }
        mSendTile.IsReservationTile = true;
    }
}
