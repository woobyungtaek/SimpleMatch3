using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillButton : MonoBehaviour
{
    public static PlayerSkill CurrentActiveSkill;

    [SerializeField] private Image mItemImage;
    [SerializeField] private Text mItemCountText;

    [SerializeField] private PlayerSkill mCurrentSkill;

    public void SetPlayerSkill(PlayerSkill skill)
    {
        mCurrentSkill = skill;
        mItemImage.sprite = SpriteManager.Instance.GetUISpriteByName($"SkillIcons_{skill.SkillNumber}");

        // Skill종류에 따라 이미지 변경
        RefreshSkillInfo();
    }
    public void ClearSkill()
    {
        // 확인해야함
        ObjectPool.ReturnInst(mCurrentSkill);
    }

    public void AddSkillCount(int count)
    {
        mCurrentSkill.SkillCount += count;
        RefreshSkillInfo();
    }

    public int SkillCount
    {
        get
        {
            return mCurrentSkill.SkillCount;
        }
        set
        {
            mCurrentSkill.SkillCount = value;
            RefreshSkillInfo();
        }
    }

    public void RefreshSkillInfo()
    {
        mItemCountText.text = mCurrentSkill.SkillCount.ToString();
    }

    public void OnButtonClicked()
    {
        if(mCurrentSkill.SkillCount <= 0) { return; }
        
        CurrentActiveSkill = mCurrentSkill;
        mCurrentSkill.OnButtonClicked(this);
    }
}
