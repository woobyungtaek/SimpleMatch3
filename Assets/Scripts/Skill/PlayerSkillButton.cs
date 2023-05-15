using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSkillButton : MonoBehaviour
{
    public static PlayerSkill CurrentActiveSkill;

    [SerializeField] private Button mSkillButton;
    [SerializeField] private Image mItemImage;
    [SerializeField] private TextMeshProUGUI mItemCountText;

    [SerializeField] private PlayerSkill mCurrentSkill;

    public void SetPlayerSkill(PlayerSkill skill)
    {
        if(PlayDataManager.IsExist)
        {
            if(PlayDataManager.Instance.IsLockItem)
            {
                mSkillButton.interactable = false;
                return;
            }
        }

        mSkillButton.onClick.RemoveAllListeners();
        mSkillButton.onClick.AddListener(OnButtonClicked);

        mCurrentSkill = skill;
        mItemImage.sprite = SpriteManager.Instance.GetUISpriteByName($"SkillIcons_{skill.SkillNumber}");

        // Skill������ ���� �̹��� ����
        RefreshSkillInfo();
    }
    public void ClearSkill()
    {
        // Ȯ���ؾ���
        ObjectPool.ReturnInst(mCurrentSkill);
    }

    public void AddSkillCount(int count)
    {
        mCurrentSkill.SkillCount += count;
        RefreshSkillInfo();
    }

    public virtual void SkillUse()
    {
        mCurrentSkill.SkillCount -= 1;
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

    public virtual void RefreshSkillInfo()
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
