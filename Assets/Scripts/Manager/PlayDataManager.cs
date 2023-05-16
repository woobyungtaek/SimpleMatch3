using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDataManager : Singleton<PlayDataManager>
{
    /// <summary>
    /// 폴더이름
    /// 처음에 생각한것이 Map에 생성 정보나 이런게 결합되어 있어서인데
    /// 컨트롤해야할 여지가 생김
    /// </summary>
    public string ConceptName;

    /// <summary>
    /// 맵 파일 이름
    /// </summary>
    public string MapName;

    /// <summary>
    /// 챕터 번호 
    /// 아웃 게임에서 ChapterNumber를 설정하고
    /// 인게임에서 이 값으로 적절한 행동을 취한다.
    /// </summary>
    public int ChapterNumber;

    /// <summary>
    /// 챕터 별 난이도 데이터
    /// Stage들의 난이도 데이터가 정해져 있다.
    /// </summary>
    public ChapterData CurrentChapterData;

    /// <summary>
    /// 챕터 기믹
    /// 맵 기믹에 대한 정보
    /// </summary>
    public MapGimmickInfo ChapterMapGimmickInfo;


    // >플레이어 정보<

    #region 부스터 적용 값들

    private int mStartCount;
    public int AdditoryMoveCount;
    public int ContinueMoveCount;
    public int ColorChangeCount;
    public int BlockSwapCount;
    public int RandomBombBoxCount;

    public int AdditoryRewardItemCount;

    public float DoubleChancePer;
    public float AdditoryGoldPer;

    public bool IsLockItem;

    public int StartCount
    {
        get => mStartCount;
        set
        {
            mStartCount = value;
            if (mStartCount <= 0)
            {
                mStartCount = 1;
            }
        }
    }
    public void InitPlayData()
    {
        mStartCount = 0;
        AdditoryMoveCount = 0;
        ContinueMoveCount = PlayerData.ContinueMoveCount;
        ColorChangeCount = 0;
        BlockSwapCount = 0;
        RandomBombBoxCount = 0;
        AdditoryRewardItemCount = 0;

        DoubleChancePer = 0f;
        AdditoryGoldPer = 0f;

        IsLockItem = false;
    }

    #endregion
    
}