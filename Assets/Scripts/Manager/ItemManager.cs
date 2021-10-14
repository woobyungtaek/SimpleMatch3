using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : SceneSingleton<ItemManager>
{
    private static string countStrFormat = "{0}";

    public int HammerCount
    {
        get
        {
            return mHammerCount;
        }
        set
        {
            mHammerCount = value;
            if(mHammerCountText != null)
            {
                mHammerCountText.text = string.Format(countStrFormat, mHammerCount);
            }
        }
    }
    public int RandomBombBoxCount
    {
        get
        {
            return mRandomBombBoxCount;
        }
        set
        {
            mRandomBombBoxCount = value;
            if (mRandombombBoxCountText != null)
            {
                mRandombombBoxCountText.text = string.Format(countStrFormat, mRandomBombBoxCount);
            }
        }
    }

    [SerializeField] private int mHammerCount;
    [SerializeField] private int mRandomBombBoxCount;

    [SerializeField] private Text mHammerCountText;
    [SerializeField] private Text mRandombombBoxCountText;

    private void Start()
    {
        HammerCount = 0;
        RandomBombBoxCount = 0;
    }

    public void OnHammerButtonClicked()
    {
        if(mHammerCount <= 0)
        {
            Debug.Log("해머 아이템 개수가 없습니다.");
            return;
        }
        if(PuzzleManager.Instance.CurrentState == EGameState.Input)
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Hammer, this);
        }
        else if (PuzzleManager.Instance.CurrentState == EGameState.Hammer)
        {
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Input);
        }
    }
    public void OnRandomBoxButtonClicked()
    {
        if (mRandomBombBoxCount <= 0)
        {
            //Debug.Log("랜덤 박스 아이템 개수가 없습니다.");
            return;
        }
        if (PuzzleManager.Instance.CurrentState != EGameState.Input) { return; }

        UseRandomBoxItem();
    }
    private void UseRandomBoxItem()
    {
        //타일 매니저로 부터 설치 할 타일을 가져온다.
        Tile selectTile = TileMapManager.Instance.GetRandomNormalTileByOrderOrNull();
        if(selectTile == null) { Debug.Log("빈 또는 일반 블록만 있는 타일이 없습니다.");  return; }

        System.Type blockType = null;
        int blockColor = Random.Range(0, 5);
        int blockHp = 1;

        int randNum = Random.Range(0, 5);
        switch(randNum)
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
        if(mission != null)
        {
            blockColor = mission.MissionColor;
        }

        RandomBombBoxCount -= 1;
        selectTile.RemoveBlockContainer();
        BlockManager.Instance.CreateBlockByBlockDataInTile(selectTile, blockType, blockColor, blockHp);
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchCheck);
    }
}
