using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillDimmedControl : MonoBehaviour
{
    [SerializeField] private TextMeshPro mUseDescText;
    [SerializeField] private SpriteRenderer mBoardMaskRenderer;
    public Sprite BoardMaskSprite
    {
        set => mBoardMaskRenderer.sprite = value;
    }

    public void Init()
    {
        mBoardMaskRenderer.gameObject.SetActive(false);
        gameObject.SetActive(false);

        ObserverCenter.Instance.AddObserver(ExcuteOnMask, Message.SkillDimmedOn);
        ObserverCenter.Instance.AddObserver(ExcuteOffMask, Message.SkillDimmedOff);
    }

    private void ExcuteOnMask(Notification noti)
    {
        mUseDescText.text = Localization.GetString(PlayerSkill.SkillUseDescKey);
        gameObject.SetActive(true);
        mBoardMaskRenderer.gameObject.SetActive(true);
    }
    private void ExcuteOffMask(Notification noti)
    {
        gameObject.SetActive(false);
        mBoardMaskRenderer.gameObject.SetActive(false);
    }
}
