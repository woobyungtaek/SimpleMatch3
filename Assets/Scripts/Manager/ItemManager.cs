using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class ItemManager : SceneSingleton<ItemManager>
{    
    [SerializeField] private PlayerSkillButton mButtonPrefab;
    [SerializeField] private Transform mButtonUITransform;
    private Dictionary<System.Type, PlayerSkillButton> mSkillButtonDict
        = new Dictionary<System.Type, PlayerSkillButton>();

    [Header("Main Skill")]
    [SerializeField] private PlayerSkillButton mMainSkillButton;
    [SerializeField] private float mMainSkillValue = 0;
    [SerializeField] private int mMainSkillCount = 0;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        mMainSkillValue = 0;
        ObserverCenter.Instance.AddObserver(ExcuteMainSkillIncrease, Message.MainSkillIncrease);

        // 초기화
        foreach (var button in mSkillButtonDict.Values)
        {
            button.ClearSkill();
            GameObjectPool.ReturnObject(button.gameObject);
        }

        // 메인 스킬 설정 및 초기화
        var obj = ObjectPool.GetInstByStr(typeof(HammerSkill).Name);
        mMainSkillButton.SetPlayerSkill(obj as PlayerSkill);
        mMainSkillButton.SkillCount = 0;
    }


    public void AddSkillCount(System.Type type, int count)
    {
        //객체를 만들고 리턴이 싫은데...
        if(mSkillButtonDict.ContainsKey(type))
        {
            mSkillButtonDict[type].AddSkillCount(count);
            return;
        }
        var obj = ObjectPool.GetInstByStr(type.Name);

        if( !(obj is PlayerSkill) ) { return; }
        var button = GameObjectPool.Instantiate<PlayerSkillButton>(mButtonPrefab.gameObject, mButtonUITransform);
        button.SetPlayerSkill(obj as PlayerSkill);
        button.AddSkillCount(count);

        mSkillButtonDict.Add(type, button);
    }

    public void ExcuteMainSkillIncrease(Notification noti)
    {
        if(mMainSkillButton.SkillCount > 0) { return; }

        mMainSkillValue += 5f;
        if(mMainSkillValue >= 100f)
        {
            mMainSkillValue = 0f;
            mMainSkillButton.AddSkillCount(1);
        }
    }
}
