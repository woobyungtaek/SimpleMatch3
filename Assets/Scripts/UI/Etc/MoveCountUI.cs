using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveCountUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI MoveCountText;
    [SerializeField]
    private Image MoveCountImage;


    private void Start()
    {
        ObserverCenter.Instance.AddObserver(ExcuteRefreshMoveCountByNoti, Message.RefreshMoveCount);
        ObserverCenter.Instance.AddObserver(ExcuteInputStateOnByNoti, EGameState.Input.ToString());
        ObserverCenter.Instance.AddObserver(ExcuteInputStateOnByNoti, EGameState.Match.ToString());

        ObserverCenter.Instance.AddObserver(ExcuteHammerStateOnByNoti, EGameState.PlayerSkill.ToString());

        ExcuteRefreshMoveCountByNoti(null);
    }

    private void ExcuteRefreshMoveCountByNoti(Notification noti)
    {
        MoveCountText.text = string.Format("{0}", TileMapManager.Instance.MoveCount);
    }
    private void ExcuteInputStateOnByNoti(Notification noti)
    {
        MoveCountImage.sprite = SpriteManager.Instance.GetUISpriteByName("Icon_Move");
        ExcuteRefreshMoveCountByNoti(null);
    }
    private void ExcuteHammerStateOnByNoti(Notification noti)
    {
        int num = PlayerSkillButton.CurrentActiveSkill.SkillNumber;
        int count = PlayerSkillButton.CurrentActiveSkill.SkillCount;
        MoveCountImage.sprite = SpriteManager.Instance.GetUISpriteByName($"SkillIcons_{num}");
        MoveCountText.text = string.Format("{0}", count);
    }
}
