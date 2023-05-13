using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(fileName = "MapGimmickInfo", menuName = "ScriptableObjects/MapGimmickInfos")]
public class MapGimmickInfo : ScriptableObject
{
    // GimickCheck용 
    [HideInInspector] public int MoveUseCount;
    [HideInInspector] public int ItemUseCount;
    [HideInInspector] public int OrderClearCount;

    // 기믹 및 체크 메서드 네임
    public string MapGimmickCheckName;
    public string MapGimmickName;

    // 발동 카운트, Public인게 맘에 안들긴함..
    public int ExcuteCheckCount;

    // 블럭같은게 몇개 설치 될 것인가
    public int GimickCount;

    //
    public EGameState ENextGameState;

    private System.Func<MapGimmickInfo, bool> mMapGimmickCheckFunc;
    private System.Action<MapGimmickInfo> mMapGimmickFunc;

    public bool IsExcute_MoveUse { get { return MoveUseCount >= ExcuteCheckCount; } }
    public bool IsExcute_ItemUse { get { return ItemUseCount >= ExcuteCheckCount; } }
    public bool IsExcute_OrderClear { get { return OrderClearCount >= ExcuteCheckCount; } }

    public bool IsExcutePossible
    {
        get
        {
            if (mMapGimmickCheckFunc == null) { return false; }
            return mMapGimmickCheckFunc.Invoke(this);
        }
    }

    public void Init()
    {
        MoveUseCount = 0;
        ItemUseCount = 0;
        OrderClearCount = 0;

        mMapGimmickFunc = null;
        mMapGimmickCheckFunc = null;

        if (string.IsNullOrEmpty(MapGimmickName) || string.IsNullOrEmpty(MapGimmickCheckName)) { return; }

        MethodInfo methodInfo;
        methodInfo = MapGimmickMethodBook.GetMapGimmickMethod(MapGimmickName);
        mMapGimmickFunc = (System.Action<MapGimmickInfo>)methodInfo.CreateDelegate(typeof(System.Action<MapGimmickInfo>));

        methodInfo = MapGimmickMethodBook.GetMapGimmickMethod(MapGimmickCheckName);
        mMapGimmickCheckFunc = (System.Func<MapGimmickInfo, bool>)methodInfo.CreateDelegate(typeof(System.Func<MapGimmickInfo, bool>));
    }

    public void ExcuteMapGimmick()
    {
        mMapGimmickFunc?.Invoke(this);
    }
}

