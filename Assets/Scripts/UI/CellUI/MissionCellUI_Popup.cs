using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionCellUI_Popup : MonoBehaviour
{
    [SerializeField] private Image mMissionImage;
    [SerializeField] private TextMeshProUGUI mMissionCountText;

    private MissionInfo mMissionInfo;

    public void InitCellUI(MissionInfo missionInfo)
    {
        mMissionInfo = missionInfo;
        mMissionImage.sprite = SpriteManager.Instance.GetBlockSpriteByBlockName(mMissionInfo.MissionSpriteName);
        mMissionImage.SetNativeSize();
        mMissionCountText.text = $"X {mMissionInfo.MissionCount}";
    }
}
