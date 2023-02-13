using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupObject : MonoBehaviour
{
    public virtual void InitPopup() { }
    public virtual void OnCancelButtonClicked()
    {
        GameObjectPool.ReturnObject(gameObject);
    }
    public virtual void OnOkButtonClicked()
    {
        GameObjectPool.ReturnObject(gameObject);
    }
}
