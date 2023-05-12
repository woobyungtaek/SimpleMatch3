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
    [SerializeField] private PlayerSkillButton_Charge mMainSkillButton;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        ObserverCenter.Instance.AddObserver(ExcuteMainSkillIncrease, Message.MainSkillIncrease);

        // 초기화
        foreach (var button in mSkillButtonDict.Values)
        {
            button.ClearSkill();
            GameObjectPool.ReturnObject(button.gameObject);
        }

        // 메인 스킬 설정 및 초기화
        var obj = ObjectPool.GetInstByStr(typeof(HammerSkill).Name);
        mMainSkillButton.SetPlayerChargeSkill(obj as PlayerSkill, 100f, 0f);
        mMainSkillButton.SkillCount = 0;
    }

    public void SetSkillCountByData()
    {
        int count_0 = 0;
        int count_1 = 0;
        int count_2 = 0;

        if(PlayDataManager.IsExist)
        {
            count_0 = PlayDataManager.Instance.RandomBombBoxCount;
            count_1 = PlayDataManager.Instance.BlockSwapCount;
            count_2 = PlayDataManager.Instance.ColorChangeCount;
        }

        AddSkillCount(typeof(RandomBoxSkill), count_0);
        AddSkillCount(typeof(BlockSwapSkill), count_1);
        AddSkillCount(typeof(ColorChangeSkill), count_2);
    }

    public void AddSkillCount(System.Type type, int count)
    {
        //객체를 만들고 리턴이 싫은데...
        if (mSkillButtonDict.ContainsKey(type))
        {
            mSkillButtonDict[type].AddSkillCount(count);
            return;
        }
        var obj = ObjectPool.GetInstByStr(type.Name);

        if (!(obj is PlayerSkill)) { return; }
        var button = GameObjectPool.Instantiate<PlayerSkillButton>(mButtonPrefab.gameObject, mButtonUITransform);
        button.SetPlayerSkill(obj as PlayerSkill);
        button.AddSkillCount(count);

        mSkillButtonDict.Add(type, button);
    }

    public void ExcuteMainSkillIncrease(Notification noti)
    {
        if (mMainSkillButton.SkillCount > 0) { return; }
        mMainSkillButton.FillCurrent += 5f;
    }
}
