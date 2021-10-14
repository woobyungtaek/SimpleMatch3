using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupObject : MonoBehaviour
{
    public virtual void InitPopup() { }
    public virtual void OnCancelButtonClicked()
    {
        GameObjectPool.Destroy(gameObject);
    }
    public virtual void OnOkButtonClicked()
    {
        GameObjectPool.Destroy(gameObject);
    }
}
