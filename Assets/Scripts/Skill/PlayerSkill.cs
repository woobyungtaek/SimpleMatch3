using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESkillType
{
    None,
    OneTile,
    TwoTile,
    OneTileOption
}

public class SkillNotiArgs : NotificationArgs, IReUseObject
{
    public PlayerSkill CurrentPlayerSkill;

    public void ResetObject()
    {
        CurrentPlayerSkill = null;
    }
}

[System.Serializable]
public class PlayerSkill : IReUseObject
{
    public static string SkillUseDescKey;

    protected PlayerSkillButton mButtonObj;

    public int SkillCount;
    public virtual ESkillType SkillType { get => ESkillType.None; }

    public virtual int SkillNumber { get; }

    public virtual void OnButtonClicked(PlayerSkillButton button) { }
    public virtual void DoSkill(Tile tile) { }
    public virtual void DoSkill(Tile tile1, Tile tile2) { }

    protected void IncreaseItemUseCount()
    {
        TileMapManager.Instance.IncreaseItemUseCount();
    }

    protected SkillNotiArgs GetNotiArgs()
    {
        var noti = ObjectPool.GetInst<SkillNotiArgs>();
        noti.CurrentPlayerSkill = this;
        return noti;
    }

    public void ResetObject()
    {
        SkillCount = 0;
    }
}


public class HammerSkill : PlayerSkill
{
    public override ESkillType SkillType { get => ESkillType.OneTile; }

    public override int SkillNumber { get => 0; }

    public override void OnButtonClicked(PlayerSkillButton button)
    {
        mButtonObj = button;
        if (PuzzleManager.Instance.CurrentState == EGameState.Input)
        {
            SkillUseDescKey = "SkillUseDesc_Hammer";
            ObserverCenter.Instance.SendNotification(Message.SkillDimmedOn);
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.PlayerSkill, null, GetNotiArgs());
        }
        else if (PuzzleManager.Instance.CurrentState == EGameState.PlayerSkill)
        {
            ObserverCenter.Instance.SendNotification(Message.SkillDimmedOff);
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Input);
        }
    }

    public override void DoSkill(Tile tile)
    {
        IncreaseItemUseCount();

        tile.HitTile(true);
        SkillCount--;
        mButtonObj.RefreshSkillInfo();
        ObserverCenter.Instance.SendNotification(Message.SkillDimmedOff);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Match);
    }
}

public class RandomBoxSkill : PlayerSkill
{
    public override ESkillType SkillType { get => ESkillType.None; }

    public override int SkillNumber { get => 1; }

    public override void OnButtonClicked(PlayerSkillButton button)
    {
        mButtonObj = button;
        if (SkillCount <= 0) { return; }
        if (PuzzleManager.Instance.CurrentState != EGameState.Input) { return; }

        UseRandomBoxItem();
    }
    private void UseRandomBoxItem()
    {
        //타일 매니저로 부터 설치 할 타일을 가져온다.
        Tile selectTile = TileMapManager.Instance.GetRandomNormalTileByOrderOrNull();
        if (selectTile == null) { Debug.Log("빈 또는 일반 블록만 있는 타일이 없습니다."); return; }

        System.Type blockType = null;
        int blockColor = Random.Range(0, 5);
        int blockHp = 1;

        int randNum = Random.Range(0, 5);
        switch (randNum)
        {
            case 0:
                blockType = typeof(VerticalBombBlock);
                break;
            case 1:
                blockType = typeof(HorizontalBombBlock);
                break;
            case 2:
                blockType = typeof(HomingBombBlock);
                break;
            case 3:
                blockType = typeof(AroundBombBlock);
                break;
            case 4:
                blockType = typeof(ColorBombBlock);
                break;
            default:
                blockType = typeof(HomingBombBlock);
                break;
        }

        MissionInfo mission = MissionManager.Instance.GetMissionInfoByType(blockType);
        if (mission != null)
        {
            blockColor = mission.MissionColor;
        }

        IncreaseItemUseCount();
        SkillCount--;
        mButtonObj.RefreshSkillInfo();
        selectTile.RemoveBlockContainer();
        BlockManager.Instance.CreateBlockByBlockDataInTile(selectTile, blockType, blockColor, blockHp);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchCheck);
    }
}

public class BlockSwapSkill : PlayerSkill
{
    public override ESkillType SkillType { get => ESkillType.TwoTile; }
    public override int SkillNumber { get => 2; }
    public override void OnButtonClicked(PlayerSkillButton button)
    {
        mButtonObj = button;
        if (PuzzleManager.Instance.CurrentState == EGameState.Input)
        {
            SkillUseDescKey = "SkillUseDesc_Swap";
            ObserverCenter.Instance.SendNotification(Message.SkillDimmedOn);
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.PlayerSkill, null, GetNotiArgs());
        }
        else if (PuzzleManager.Instance.CurrentState == EGameState.PlayerSkill)
        {
            ObserverCenter.Instance.SendNotification(Message.SkillDimmedOff);
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Input);
        }
    }

    public override void DoSkill(Tile tile1, Tile tile2)
    {
        // > 블록 컨테이너가 있고 & 블록이 움직일 수 있는 상태여야한다.
        if (tile1.BlockContainerOrNull.IsFixed) { return; }
        if (tile2.BlockContainerOrNull.IsFixed) { return; }

        // 둘다 변경 가능한 상태라면 교환 후
        var tempBC = tile2.BlockContainerOrNull;
        tile2.BlockContainerOrNull = tile1.BlockContainerOrNull;
        tile1.BlockContainerOrNull = tempBC;

        tile1.BlockContainerOrNull.transform.position = tile1.transform.position;
        tile2.BlockContainerOrNull.transform.position = tile2.transform.position;
        
        IncreaseItemUseCount();

        // 스킬사용 및 매치 체크
        SkillCount--;
        mButtonObj.RefreshSkillInfo();
        ObserverCenter.Instance.SendNotification(Message.SkillDimmedOff);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Match);
    }
}

public class ColorChangeSkill : PlayerSkill
{
    public override ESkillType SkillType { get => ESkillType.OneTile; }
    public override int SkillNumber { get => 3; }

    private List<Tile> mTargetTileList = new List<Tile>();

    public override void OnButtonClicked(PlayerSkillButton button)
    {
        mButtonObj = button;
        if (PuzzleManager.Instance.CurrentState == EGameState.Input)
        {
            SkillUseDescKey = "SkillUseDesc_ColorChange";
            ObserverCenter.Instance.SendNotification(Message.SkillDimmedOn);
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.PlayerSkill, null, GetNotiArgs());
        }
        else if (PuzzleManager.Instance.CurrentState == EGameState.PlayerSkill)
        {
            ObserverCenter.Instance.SendNotification(Message.SkillDimmedOff);
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Input);
        }
    }

    public override void DoSkill(Tile tile1 )
    {
        // > 블록 컨테이너가 있고 & 블록이 움직일 수 있는 상태여야한다.
        if (tile1.BlockContainerOrNull.IsFixed) { return; }

        int targetNum = tile1.BlockContainerOrNull.MainBlock.BlockNumber;
        if (targetNum == -1) { return; }

        int changeNum = Random.Range(0, 5);
        while(changeNum == targetNum)
        {
            changeNum = Random.Range(0, 5);
        }

        // targetNum과 같은 블록을 changeNum으로 바꾸고 갱신해야한다.
        TileMapManager.Instance.CreateTileListBySameNumber(mTargetTileList, targetNum);

        foreach(var tile in mTargetTileList)
        {
            BlockContainer bc = tile.BlockContainerOrNull;
            int hp = bc.MainBlock.BlockHP;
            bc.MainBlock.SetBlockData(changeNum, hp);
        }

        IncreaseItemUseCount();

        // 스킬사용 및 매치 체크
        SkillCount--;
        mButtonObj.RefreshSkillInfo();
        ObserverCenter.Instance.SendNotification(Message.SkillDimmedOff);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Match);
    }
}