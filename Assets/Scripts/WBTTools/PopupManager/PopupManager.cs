using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : SceneSingleton<PopupManager>
{
    [SerializeField] private Transform mPopupTransform;
    [SerializeField] private GameObject mDimmied;

    [SerializeField] private List<Popup> mPopupPrefabList = new List<Popup>();

    private void Awake()
    {
        Popup.SetPopupInfo(mDimmied);
    }

    public void CreatePopupByName(string popupName)
    {
        // 이름으로 프리팹을 찾아서 있다면 생성합니다.
        Popup target = null;
        foreach(Popup obj in mPopupPrefabList)
        {
            if(obj.GetType().Name == popupName)
            {
                target = obj;
                break;
            }
        }

        if(target != null)
        {
            Popup inst = GameObjectPool.Instantiate<Popup>(target.gameObject, mPopupTransform);
            inst.Init();
        }
    }
}