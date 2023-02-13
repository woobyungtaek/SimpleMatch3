using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCellUI : MonoBehaviour, System.IDisposable
{
    public bool IsComplete { get => (mMissionInfo.MissionCount <= 0); }
    public MissionInfo CurrentMission { get => mMissionInfo; }

    private int mFakeCount;
    [SerializeField] private Image mMissionImage;
    [SerializeField] private Text mMissionCountText;

    private MissionInfo mMissionInfo;
  
    public void InitCellUI(MissionInfo missionInfo)
    {
        ObserverCenter.Instance.AddObserver(CellRefreshByFakeCount, Message.RefreshMissionCellUI);

        mMissionInfo = missionInfo;

        mFakeCount = mMissionInfo.MissionCount;
        mMissionImage.sprite = SpriteManager.Instance.GetBlockSpriteByBlockName(mMissionInfo.MissionSpriteName);
        mMissionCountText.text = string.Format("{0}", mFakeCount);
    }
    public void Dispose()
    {
        ObserverCenter.Instance.RemoveObserver(Message.RefreshMissionCellUI, CellRefreshByFakeCount);
    }

    public void CollectMissionTarget()
    {
        mMissionInfo.MissionCount -= 1;
    }

    private void CellRefreshByFakeCount(Notification noti)
    {
        //null 발생했었음
        //if (mMissionCountText == null) { return; }
        if (mMissionInfo.MissionCount < mFakeCount)
        {
            mFakeCount -= 1;
            
            mMissionCountText.text = string.Format("{0}", mFakeCount);
        }
    }

}
