using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 22-01-29 : NotificaitonArgs를 ObjectPool로 반환하는 코드 삽입 > 테스트 필요
// 22-06-09 : LinkedList로 변경 > 삭제 후 ProcessObserver에서 삭제된 인덱스로 접근이 발생해서
//            And 지우는 행동 시 Next가 링킹이 끊킴.. 다음을 미리 저장후 지우고 다시 설정해준다.

public class ObserverCenter : MonoBehaviour
{
    #region Singleton
    private static ObserverCenter instance;
    private static object lockObj = new object();
    public static ObserverCenter Instance
    {
        get
        {
            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(ObserverCenter)) as ObserverCenter;
                }
                if (instance == null)
                {
                    GameObject ObseverCenterSingleton = new GameObject();
                    instance = ObseverCenterSingleton.AddComponent<ObserverCenter>();
                    ObseverCenterSingleton.name = typeof(ObserverCenter).Name;
                }
            }
            return instance;
        }
    }
    #endregion

    #region Define
    private class Observer
    {
        public SubscribeMethod _SubMethod;
        public string _Message;
        public Component _Sender;

        public Observer(SubscribeMethod subscribeMethod, string message, Component sender)
        {
            _SubMethod = subscribeMethod;
            _Message = message;
            _Sender = sender;
        }
    }
    public delegate void SubscribeMethod(Notification observerInfo);
    #endregion

    #region Member
    private Dictionary<string, LinkedList<Observer>> mMessageObserver = new Dictionary<string, LinkedList<Observer>>();
    private Dictionary<Component, LinkedList<Observer>> mSenderObserver = new Dictionary<Component, LinkedList<Observer>>();
    #endregion

    #region Management
    /// <summary>
    /// 감시자 센터에 감시자를 추가 합니다.
    /// </summary>
    /// <param name="subMethod">감시자가 실행할 Method</param>
    /// <param name="message">구독할 메세지</param>
    /// <param name="senderOrNull">메세지 발생자</param>
    public void AddObserver(SubscribeMethod subMethod, string message, Component senderOrNull = null)
    {
        Observer observer = null;
        LinkedList<Observer> observerList = null;

        if (!string.IsNullOrEmpty(message))
        {
            observer = new Observer(subMethod, message, senderOrNull);

            if (!mMessageObserver.ContainsKey(message))
            {
                mMessageObserver[message] = new LinkedList<Observer>();
            }
            observerList = mMessageObserver[message];
            observerList.AddLast(observer);
        }
        else if (senderOrNull != null)
        {
            observer = new Observer(subMethod, message, senderOrNull);
            if (!mSenderObserver.ContainsKey(senderOrNull))
            {
                mSenderObserver[senderOrNull] = new LinkedList<Observer>();
            }
            observerList = mSenderObserver[senderOrNull];
            observerList.AddLast(observer);
        }
        else
        {
            Debug.LogError("감시자 추가 실패, 메세지 또는 발신자가 필요합니다.");
        }
    }

    public void RemoveObserver(string msg, SubscribeMethod subscribeMethod)
    {
        foreach (var observer in mMessageObserver[msg])
        {
            if (observer._SubMethod != subscribeMethod) { continue; }

            mMessageObserver[msg].Remove(observer);
            break;
        }
    }

    public void RemoveObserver(Component sender, SubscribeMethod subscribeMethod)
    {
        foreach (var observer in mSenderObserver[sender])
        {
            if (observer._SubMethod != subscribeMethod) { continue; }
            mSenderObserver[sender].Remove(observer);
            break;
        }
    }

    #endregion

    public void SendNotification(Notification obInfo)
    {
        ProcessObserver(obInfo);
    }
    public void SendNotification(string message, NotificationArgs dataOrNull = null)
    {
        Notification inst = Notification.Instantiate(message, null, dataOrNull);
        SendNotification(inst);
    }
    public void SendNotification(Component sender, NotificationArgs dataOrNull = null)
    {
        Notification inst = Notification.Instantiate(null, sender, dataOrNull);
        SendNotification(inst);
    }
    public void SendNotification(Component sender, string message, NotificationArgs dataOrNull = null)
    {
        Notification inst = Notification.Instantiate(message, sender, dataOrNull);
        SendNotification(inst);
    }

    //public void PostObserverInfo(ObserverInfo obInfo)
    //{
    //    ProcessObserver(obInfo);
    //}
    //public void PostObserverInfo(string message)
    //{
    //    ObserverInfo inst = ObserverInfo.Instantiate(message, null);
    //    PostObserverInfo(inst);
    //}
    //public void PostObserverInfo(Component sender)
    //{
    //    ObserverInfo inst = ObserverInfo.Instantiate(null, sender);
    //    PostObserverInfo(inst);
    //}
    //public void PostObserverInfo(string message, Component sender)
    //{
    //    ObserverInfo inst = ObserverInfo.Instantiate(message, sender);
    //    PostObserverInfo(inst);
    //}

    private void ProcessObserver(Notification obInfo)
    {
        if (!string.IsNullOrEmpty(obInfo.Message) && mMessageObserver.ContainsKey(obInfo.Message))
        {
            var node = mMessageObserver[obInfo.Message].First;
            while (node != null)
            {
                if (node.Value == null)
                {
                    node = node.Next;
                    continue;
                }
                var inst = node.Next;
                node.Value._SubMethod(obInfo);
                node = inst;
            }
        }
        else if (obInfo.Sender != null && mSenderObserver.ContainsKey(obInfo.Sender))
        {
            var node = mSenderObserver[obInfo.Sender].First;
            while (node != null)
            {
                if (node.Value == null)
                {
                    node = node.Next; continue;
                }
                var inst = node.Next;
                node.Value._SubMethod(obInfo);
                node = inst;
            }
        }

        // Notification의 원형 이름을 알아야 하는데...
        if (obInfo.Data != null)
        {
            ObjectPool.ReturnInstByStr(obInfo.Data.GetType().Name, obInfo.Data);
        }

        Notification.Destroy(obInfo);
    }

}
