using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCellUI_Popup : MonoBehaviour
{
    [SerializeField] private Image mMissionImage;
    [SerializeField] private Text mMissionCountText;

    public void InitCellUI(MissionInfo missionInfo)
    {
        mMissionImage.sprite = SpriteManager.Instance.GetBlockSpriteByBlockName(missionInfo.MissionSpriteName);
        mMissionImage.SetNativeSize();
        mMissionCountText.text = $"X {missionInfo.MissionCount}";
    }
}
