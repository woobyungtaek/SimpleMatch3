using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class NotiTextUI : MonoBehaviour
{
    protected string messageStr;
    [SerializeField] protected string textStr;
    [SerializeField] protected Text textUi;

    private void Awake()
    {
        SettingTextInfo();
        ObserverCenter.Instance.AddObserver(ExcuteRefreshTextByNoti, messageStr);
    }
    private void OnEnable()
    {
        if (textUi == null)
        {
            textUi = gameObject.GetComponent<Text>();
        }
        ObserverCenter.Instance.SendNotification(messageStr);
    }

    public abstract void SettingTextInfo();
    public abstract void ExcuteRefreshTextByNoti(Notification noti);
}
