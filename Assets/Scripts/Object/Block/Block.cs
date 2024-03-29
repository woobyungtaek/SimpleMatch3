﻿using System.Collections;
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

    public virtual bool IsFixed { get => mbFixed; }

    public virtual bool IsVineTarget { get => false; }

    public Sprite BlockSprite { get => mBlockSprite.sprite; }
    public virtual string SpriteString { get => null; }

    [SerializeField] protected int mBlockNumber;
    [SerializeField] protected int mBlockHP;
    [SerializeField] protected int mOrder;
    [SerializeField] protected int mHomingOrder;
    [SerializeField] protected int mExtraOrder;
    [SerializeField] protected bool mbFixed;
    [SerializeField] protected SpriteRenderer mBlockSprite;

    private void Awake()
    {
        ObserverCenter.Instance.AddObserver(ExcuteCalculateHomiingOrder, Message.RefreshMission, null);
    }

    public bool IsSameBlock(System.Type type, int blockColor)
    {
        if (!GetType().Equals(type)) { return false; }
        if (blockColor < 0) { return true; }
        if (mBlockNumber == blockColor) { return true; }
        return false;
    }

    public virtual void SetBlockData(int blockNumber, int blockHP)
    {
        mBlockNumber = blockNumber;
        mBlockHP = blockHP;
        ExcuteCalculateHomiingOrder(null);
        mBlockSprite.sprite = SpriteManager.Instance.GetPuzzleSpriteByName(SpriteString);
    }
    public virtual void HitBlock(Tile tile, BlockContainer blockContainer, bool bExplosion) { }
    public virtual void SplashHitBlock(Tile tile, BlockContainer blockContainer) { }
    public bool CheckMissionBlock()
    {
        bool bResult = MissionManager.Instance.CheckMissionTargetByInfo(transform.position, GetType(), BlockNumber, BlockSprite);
        if (!bResult)
        {
            // 미션 블럭이 아니면 게이지로 전환 되어야 한다.
            ObserverCenter.Instance.SendNotification(Message.MainSkillIncrease);
        }

        return bResult;
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
    }

    public void Dispose() { }
}
