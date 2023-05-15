using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionCellUI : MonoBehaviour, System.IDisposable
{
    public bool IsComplete { get => (mMissionInfo.MissionCount <= 0); }
    public MissionInfo CurrentMission { get => mMissionInfo; }

    private int mFakeCount;
    [SerializeField] private Image mMissionImage;
    [SerializeField] private TextMeshProUGUI mMissionCountText;

    [SerializeField] private MissionInfo mMissionInfo;

    private void Start()
    {
        ObserverCenter.Instance.AddObserver(CellRefreshByFakeCount, Message.RefreshMissionCellUI);

#if UNITY_EDITOR
        ObserverCenter.Instance.AddObserver(Cheat_ClearMission, Message.ClearMissionCheat);
#endif 
    }

    //private void OnDestroy()
    //{
    //    ObserverCenter.Instance.RemoveObserver(Message.RefreshMissionCellUI, CellRefreshByFakeCount);
    //    ObserverCenter.Instance.RemoveObserver(Message.ClearMissionCheat, Cheat_ClearMission);
    //}

    public void InitCellUI(MissionInfo missionInfo)
    {
        mMissionInfo = missionInfo;

        mFakeCount = mMissionInfo.MissionCount;
        mMissionImage.sprite = SpriteManager.Instance.GetBlockSpriteByBlockName(mMissionInfo.MissionSpriteName);
        mMissionImage.SetNativeSize();
        mMissionCountText.text = string.Format("{0}", mFakeCount);
    }
    public void Dispose()
    {
        ObserverCenter.Instance.RemoveObserver(Message.RefreshMissionCellUI, CellRefreshByFakeCount);
    }

    public void CollectMissionTarget(int count)
    {
        mMissionInfo.MissionCount -= count;
    }

    private void CellRefreshByFakeCount(Notification noti)
    {
        // 더블 찬스 발동 시 어떻게 처리 할 것인가..

        if (mMissionInfo.MissionCount < mFakeCount)
        {
            mFakeCount -= 1;

            mMissionCountText.text = $"{mFakeCount}";
        }
    }

#if UNITY_EDITOR
    private void Cheat_ClearMission(Notification noti)
    {
        mMissionInfo.MissionCount = 0;
        mMissionCountText.text = $"{mMissionInfo.MissionCount}";
    }
#endif
}
