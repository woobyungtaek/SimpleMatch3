using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSuccessPopup : PopupObject
{
    [Header("Mission")]
    [SerializeField]    private GameObject  mMissionCellUIPrefab;
    [SerializeField]    private Transform   mMissionCellTransform;
    private List<MissionCellUI> mMissionCellList = new List<MissionCellUI>();

    [Header("Reward")]
    [SerializeField]    private GameObject      mRewardCellUIPrefab;
    [SerializeField]    private GridLayoutGroup mRewardCellGrid;
    private List<RewardCellUI> mRewardCellList = new List<RewardCellUI>();

    [Header("SelectReward")]
    [SerializeField] private RewardData         mSelectedReward;
    [SerializeField] private GameObject         mSelectCellUIPrefab;
    [SerializeField] private GridLayoutGroup    mSelectRewardCellGrid;
    [SerializeField] private GameObject         mSelectRewardInfoText;
    private List<SelectRewardCellUI> mSelectRewardCellList = new List<SelectRewardCellUI>();
    private object[] mRewardParam = new object[1];
    
    private void CreateNextMissionCellUI(List<MissionInfo> missionList)
    {
        int loopCount = missionList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mMissionCellList.Add(GameObjectPool.Instantiate<MissionCellUI>(mMissionCellUIPrefab,mMissionCellTransform));
            mMissionCellList[index].InitCellUI(missionList[index]);
        }
    }
    private void ClearMissionCellList()
    {
        int loopCount = mMissionCellList.Count;
        for (int index = 0; index < mMissionCellList.Count; index++)
        {
            GameObjectPool.ReturnObject(mMissionCellList[index].gameObject);
        }
        mMissionCellList.Clear();
    }    
    private void CreateRewardMissionCellUI(List<RewardData> basicRewardList)
    {
        int loopCount = basicRewardList.Count;
        for(int index =0; index < loopCount; index++)
        {
            RewardCellUI inst =
                GameObjectPool.Instantiate<RewardCellUI>(mRewardCellUIPrefab, mRewardCellGrid.transform);
            inst.InitCellUI(basicRewardList[index]);
            mRewardCellList.Add(inst);
        }
    }
    private void ClearRewardCellList()
    {
        int loopCount = mRewardCellList.Count;
        for (int index = 0; index < mRewardCellList.Count; index++)
        {
            GameObjectPool.ReturnObject(mRewardCellList[index].gameObject);
        }
        mRewardCellList.Clear();
    }    
    private void CreateSelectRewardMissionCellUI(List<RewardData> selectRewardList)
    {
        int loopCount = selectRewardList.Count;
        for(int index =0; index< loopCount; index++)
        {
            SelectRewardCellUI inst =
                GameObjectPool.Instantiate<SelectRewardCellUI>(mSelectCellUIPrefab, mSelectRewardCellGrid.transform);

            inst.InitCellUI(selectRewardList[index]);
            inst.UnSelectCell();
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
        mSelectedReward = cellUI.CellRewardData;

        int loopCount = mSelectRewardCellList.Count;
        for(int index =0; index< loopCount; index++)
        {
            if(mSelectRewardCellList[index] == cellUI) { continue; }
            mSelectRewardCellList[index].UnSelectCell();
        }
    }

    public override void InitPopup()
    {
        mSelectedReward = null;
        mSelectRewardInfoText.SetActive(false);

        CreateNextMissionCellUI(MissionManager.Instance.MissionInfoList);
        CreateRewardMissionCellUI(MissionManager.Instance.BasicRewardDataList);
        CreateSelectRewardMissionCellUI(MissionManager.Instance.SelectRewardDataList);
    }
    public override void OnCancelButtonClicked()
    {
        if (mSelectRewardCellList.Count > 0)
        {
            if (mSelectedReward == null)
            {
                mSelectRewardInfoText.SetActive(true);
                return;
            }
            mRewardParam[0] = mSelectedReward;
            mSelectedReward.RewardMethodInfo.Invoke(null, mRewardParam);
        }

        int loopCount = mRewardCellList.Count;
        for(int index =0; index< loopCount; index++)
        {
            mRewardParam[0] = mRewardCellList[index].CurrenRewardData;
            mRewardCellList[index].CurrenRewardData.RewardMethodInfo.Invoke(null, mRewardParam);
        }

        ClearMissionCellList();
        ClearRewardCellList();
        ClearSelectRewardCellList();

        MissionManager.Instance.StartStage();
        base.OnCancelButtonClicked();
    }
    public override void OnOkButtonClicked()
    {
        OnCancelButtonClicked();
    }
}
