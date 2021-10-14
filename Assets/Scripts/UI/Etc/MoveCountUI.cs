using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCountUI : MonoBehaviour
{
    [SerializeField]
    private Text MoveCountText;
    [SerializeField]
    private Image MoveCountImage;

    private void Start()
    {
        //if (MoveCountText == null) { MoveCountText = GetComponentInChildren<Text>(); }
        //if (MoveCountImage == null) { MoveCountImage = GetComponentInChildren<Image>(); }

        ObserverCenter.Instance.AddObserver(ExcuteRefreshMoveCountByNoti, Message.RefreshMoveCount);
        ObserverCenter.Instance.AddObserver(ExcuteInputStateOnByNoti, EGameState.Input.ToString());
        ObserverCenter.Instance.AddObserver(ExcuteInputStateOnByNoti, EGameState.Match.ToString());
        ObserverCenter.Instance.AddObserver(ExcuteHammerStateOnByNoti, EGameState.Hammer.ToString());

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
        MoveCountImage.sprite = SpriteManager.Instance.GetUISpriteByName("Icon_Hammer");
        MoveCountText.text = string.Format("{0}", ItemManager.Instance.HammerCount);
    }
}
