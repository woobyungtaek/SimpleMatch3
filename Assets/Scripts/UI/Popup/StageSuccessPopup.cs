using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 선택 보상 팝업으로 바뀌어야하고
// 일반 보상은 캐릭터가 퇴장하기 전에 줘야함

public class StageSuccessPopup : Popup
{
    [Header("SelectReward")]
    [SerializeField] private RewardData mSelectedReward;
    [SerializeField] private GameObject mSelectCellUIPrefab;
    [SerializeField] private GridLayoutGroup mSelectRewardCellGrid;
    [SerializeField] private GameObject mSelectImage;
    [SerializeField] private List<GameObject> mSelectUiObjectList = new List<GameObject>();
    private List<SelectRewardCellUI> mSelectRewardCellList = new List<SelectRewardCellUI>();
    private object[] mRewardParam = new object[1];

    private void CreateSelectRewardMissionCellUI(List<RewardData> selectRewardList)
    {
        int loopCount = selectRewardList.Count;
        if (loopCount == 0) { return; }

        for (int index = 0; index < loopCount; index++)
        {
            SelectRewardCellUI inst =
                GameObjectPool.Instantiate<SelectRewardCellUI>(mSelectCellUIPrefab, mSelectRewardCellGrid.transform);

            inst.InitCellUI(selectRewardList[index]);
            inst.EventSelectReward = OnSelectRewardButtonClicked;

            mSelectRewardCellList.Add(inst);
        }

    }
    private void ClearSelectRewardCellList()
    {
        int loopCount = mSelectRewardCellList.Count;
        for (int index = 0; index < mSelectRewardCellList.Count; index++)
        {
            GameObjectPool.ReturnObject(mSelectRewardCellList[index].gameObject);
        }
        mSelectRewardCellList.Clear();
    }

    public void OnSelectRewardButtonClicked(SelectRewardCellUI cellUI)
    {
        //if(mSelectedReward == cellUI.CellRewardData)
        //{
        //    OnCancelButtonClicked();
        //    return;
        //}
        //mSelectImage.transform.position = cellUI.transform.position;
        //mSelectImage.SetActive(true);


        mSelectedReward = cellUI.CellRewardData;
        OnCancelButtonClicked();

        int loopCount = mSelectRewardCellList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mSelectRewardCellList[index] == cellUI) { continue; }
        }
    }

    private void Awake()
    {
        mPopupAniInfo.Start = GetDownPos(transform as RectTransform);
        mPopupAniInfo.End = Vector3.zero;

        mCloseAniInfo.Start = Vector3.zero;
        mCloseAniInfo.End = GetUpPos(transform as RectTransform);
    }

    public override void Init()
    {
        base.Init();

        mSelectedReward = null;
        mSelectImage.SetActive(false);
        CreateSelectRewardMissionCellUI(MissionManager.Instance.SelectRewardDataList);
    }
    public void OnCancelButtonClicked()
    {
        if (mSelectRewardCellList.Count > 0)
        {
            if (mSelectedReward == null) { return; }
            mRewardParam[0] = mSelectedReward;
            mSelectedReward.RewardMethodInfo.Invoke(null, mRewardParam);
        }
        ClearSelectRewardCellList();
        MissionManager.Instance.TakeBasicReward();
        ClosePopup(true);
    }
}
