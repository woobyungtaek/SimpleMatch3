using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainCustomerPopup : Popup
{
    public static int CellCount = 3;

    [Header("RemainCustomerPopup")]
    [SerializeField] private Transform mCellUIGridTransform;
    [SerializeField] private GameObject mCellUIPrefab;

    private List<RemainCellUI> mCellUIList = new List<RemainCellUI>();

    public override void Init()
    {
        base.Init();
        ObserverCenter.Instance.SendNotification(Message.PauseGame);
        Time.timeScale = 0;

        CreateCellList();
    }

    private void CreateCellList()
    {
        mCellUIList.Clear();
        for (int idx = 0; idx < CellCount; ++idx)
        {
            var missionData = MissionManager.Instance.GetStageInfoByIndex(idx);
            if(missionData == null) { break; }

            var cellUI = GameObjectPool.Instantiate<RemainCellUI>(mCellUIPrefab, mCellUIGridTransform);
            cellUI.init(idx, missionData);
            cellUI.transform.SetAsLastSibling();
            mCellUIList.Add(cellUI);
        }
    }

    public void OnCancelButtonClicked()
    {
        foreach(var cellUI in mCellUIList)
        {
            cellUI.ClearCellUI();
            GameObjectPool.ReturnObject(cellUI.gameObject);
        }
        mCellUIList.Clear();

        ObserverCenter.Instance.SendNotification(Message.ResumeGame);
        Time.timeScale = 1;
        ClosePopup(true);
    }
}
