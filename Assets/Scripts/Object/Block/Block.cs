using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Block : MonoBehaviour, System.IDisposable
{
    public int BlockNumber { get => mBlockNumber; set => mBlockNumber = value; }
    public int BlockHP { get => mBlockHP; set => mBlockHP = value; }
    public int Order { get => mOrder; }
    public virtual int HomingOrder
    {
        get
        {
            return mHomingOrder + mExtraOrder;
        }
    }
    public Sprite BlockSprite { get => mBlockSprite.sprite; }
    public virtual string SpriteString { get => null; }

    [SerializeField] private int mBlockNumber;
    [SerializeField] private int mBlockHP;
    [SerializeField] private int mOrder;
    [SerializeField] private int mHomingOrder;
    [SerializeField] private int mExtraOrder;
    [SerializeField] private SpriteRenderer mBlockSprite;

    private void Awake()
    {
        ObserverCenter.Instance.AddObserver(ExcuteCalculateHomiingOrder, Message.RefreshMission, null);
    }

    public virtual void SetBlockData(int blockNumber, int blockHP)
    {
        mBlockNumber = blockNumber;
        mBlockHP = blockHP;
        ExcuteCalculateHomiingOrder(null);
        mBlockSprite.sprite = SpriteManager.Instance.GetBlockSpriteByBlockName(GetSpriteNameByBlockNumber());
    }
    public virtual void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion) { }
    public void CheckMissionBlock()
    {
        MissionManager.Instance.CheckMissionTargetByInfo(transform.position, GetType(), BlockNumber, BlockSprite);
    }
    
    public void TestBlockColorChange(Color color)
    {
        mBlockSprite.color = color;
    }

    protected void ExcuteCalculateHomiingOrder(Notification noti)
    {
        mExtraOrder = 0;
        if (MissionManager.Instance.IsMissionBlock(this.GetType(), BlockNumber))
        {
            mExtraOrder = 100;
            if (this is BombBlock)
            {
                mExtraOrder += 10;
            }
        }
    }
    protected void RemoveBlockToBlockContianer(BlockContainer blockContainer)
    {
        blockContainer.RemoveBlockByBlock(this);
        GameObjectPool.Destroy(gameObject);
    }
    protected virtual string GetSpriteNameByBlockNumber()
    {
        return null;
    }

    public void Dispose() { }
}
