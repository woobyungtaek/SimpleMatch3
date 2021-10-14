using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverCenter : MonoBehaviour
{
    #region Singleton
    private static bool mbDestroy = false;
    private static ObserverCenter instance;
    private static object lockObj = new object();
    public static ObserverCenter Instance
    {
        get
        {
            //if(mbDestroy == true)
            //{
            //    return null;
            //}
            lock(lockObj)
            {
                if(instance == null)
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
        public SubscribeMethod  _SubMethod;
        public string           _Message;
        public Component        _Sender;

        public Observer (SubscribeMethod subscribeMethod, string message, Component sender)
        {
            _SubMethod  = subscribeMethod;
            _Message    = message;
            _Sender     = sender;
        }
    }
    public delegate void SubscribeMethod(Notification observerInfo);
    #endregion

    #region Member
    private Dictionary<string, List<Observer>>       mMessageObserver   = new Dictionary<string, List<Observer>>();
    private Dictionary<Component, List<Observer>>   mSenderObserver    = new Dictionary<Component, List<Observer>>();
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
        List<Observer> observerList = null;

        if (!string.IsNullOrEmpty(message))
        {
            observer = new Observer(subMethod, message, senderOrNull);

            if (!mMessageObserver.ContainsKey(message))
            {
                mMessageObserver[message] = new List<Observer>();
            }
            observerList = mMessageObserver[message];
            observerList.Add(observer);
        }
        else if (senderOrNull != null)
        {
            observer = new Observer(subMethod, message, senderOrNull);
            if (!mSenderObserver.ContainsKey(senderOrNull))
            {
                mSenderObserver[senderOrNull] = new List<Observer>();
            }
            observerList = mSenderObserver[senderOrNull];
            observerList.Add(observer);
        }
        else
        {
            Debug.LogError("감시자 추가 실패, 메세지 또는 발신자가 필요합니다.");
        }
    }
    public void RemoveObserver(SubscribeMethod subscribeMethod)
    {
        int loopCount;
        List<Observer> removeList = new List<Observer>();


        foreach(List<Observer> messageObserver in mMessageObserver.Values)
        {
            loopCount = messageObserver.Count;
            for (int index =0; index< loopCount; index++)
            {
                if (messageObserver[index]._SubMethod != subscribeMethod) { continue; }
                removeList.Add(messageObserver[index]);
            }

            loopCount = removeList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                messageObserver.Remove(removeList[index]);
            }

            removeList.Clear();
        }
        foreach (List<Observer> senderObserver in mSenderObserver.Values)
        {
            loopCount = senderObserver.Count;
            for(int index =0; index< loopCount; index++)
            {
                if(senderObserver[index]._SubMethod != subscribeMethod) { continue; }
                removeList.Add(senderObserver[index]);
            }

            loopCount = removeList.Count;
            for(int index =0; index< loopCount; index++)
            {
                senderObserver.Remove(removeList[index]);
            }
            removeList.Clear();
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
        List<Observer> observerList;
        int loopCount;

        if (!string.IsNullOrEmpty(obInfo.Message) && mMessageObserver.ContainsKey(obInfo.Message))
        {
            observerList = mMessageObserver[obInfo.Message];

            loopCount = observerList.Count;
            for(int index=0; index< loopCount; index++)
            {
                observerList[index]._SubMethod(obInfo);
            }
        }
        else if(obInfo.Sender != null && mSenderObserver.ContainsKey(obInfo.Sender))
        {
            observerList = mSenderObserver[obInfo.Sender];

            loopCount = observerList.Count;
            for (int index = 0; index < loopCount; index++)
            {
                observerList[index]._SubMethod(obInfo);
            }
        }

        Notification.Destroy(obInfo);
    }

    private void OnDestroy()
    {
        mbDestroy = true;
    }
}
