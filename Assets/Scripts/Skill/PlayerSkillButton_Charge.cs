using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillButton_Charge : PlayerSkillButton
{
    private float mFillMax;
    private float mFillCurrent;

    [SerializeField] private Image mChargeImage;

    public float FillCurrent
    {
        get 
        { 
            return mFillCurrent; 
        }
        set
        {
            mFillCurrent = value;
            mChargeImage.fillAmount = mFillAmount;
            if(mChargeImage.fillAmount >= 1f)
            {
                SkillCount = 1;
            }
        }
    }

    private float mFillAmount
    {
        get => mFillCurrent / mFillMax;
    }


    public void SetPlayerChargeSkill(PlayerSkill skill, float max, float current)
    {
        mFillMax = max;
        mFillCurrent = current;

        mChargeImage.fillAmount = mFillAmount;

        SetPlayerSkill(skill);

        // 충전형 스킬은 아이템 잠금에 해당하지 않는다.
        mSkillButton.interactable = true;
        mLockImage.gameObject.SetActive(!mSkillButton.interactable);
    }


    public override void SkillUse()
    {
        mFillCurrent = 0;
        mChargeImage.fillAmount = mFillAmount;
        base.SkillUse();
    }
    public override void RefreshSkillInfo()
    {
        base.RefreshSkillInfo();
    }
}
