using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 감시자의 정보 : 
/// 구독메세지, 감시자(컴포넌트)
/// </summary>
public class Notification : Pool<Notification>
{
    public string               Message;
    public Component            Sender;
    public NotificationArgs     Data;

    public override void Dispose()
    {
        Message     = null;
        Sender      = null;
        if(Data != null) { Data.Dispose(); }
        Data = null;
    }
    public static Notification Instantiate(string message, Component sender, NotificationArgs dataOrNull = null)
    {
        Notification inst   = Instantiate();
        inst.Message        = message;
        inst.Sender         = sender;
        inst.Data           = dataOrNull;

        return inst;
    }
}
public class NotificationArgs : IDisposable
{
    public virtual void Dispose() { }
}
