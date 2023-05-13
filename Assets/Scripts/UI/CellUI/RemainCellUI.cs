using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RemainCellUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mIndexText;
    [SerializeField] private Transform mGridTransform;
    [SerializeField] private GameObject mMissionCellUIPrefab;

    private List<MissionCellUI_Popup> mCellUIList = new List<MissionCellUI_Popup>();

    public void init(int idx, MissionData missionData)
    {
        mCellUIList.Clear();

        mIndexText.text = $"{idx + 1}";

        foreach(var data in missionData.MissionInfoList)
        {
            var cellUI = GameObjectPool.Instantiate<MissionCellUI_Popup>(mMissionCellUIPrefab, mGridTransform);
            cellUI.InitCellUI(data);
            cellUI.transform.SetAsLastSibling();
            mCellUIList.Add(cellUI);
        }
    }

    public void ClearCellUI()
    {
        foreach (var cellUI in mCellUIList)
        {
            GameObjectPool.ReturnObject(cellUI.gameObject);
        }
        mCellUIList.Clear();
    }
}
