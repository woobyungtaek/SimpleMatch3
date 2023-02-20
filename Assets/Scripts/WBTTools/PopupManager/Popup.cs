using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Popup : MonoBehaviour
{
    protected enum EPopupAnimation
    {
        None,
        Pop,
        Slide
    }

    // 현재 활성화 된 팝업 오브젝트
    protected static Popup CurrentPopupObj;

    // 현재 우선순위가 밀린 팝업 오브젝트 스택(새 팝업이 켜질때 CurrentPopup이 목록에 들어간다)
    protected static Stack<Popup> InstPopupStack = new Stack<Popup>();

    [SerializeField] private EPopupAnimation mPopupAni;
    [SerializeField] private AnimationCurve mPopupAniCurve;

    // 가리기 오브젝트
    protected static GameObject DimmiedObj;
    protected bool OnDimmied
    {
        set
        {
            if(DimmiedObj != null) { DimmiedObj.SetActive(value); }
        }
    }

    // 외부
    public static void SetPopupInfo(GameObject dimmiedObj)
    {
        DimmiedObj = dimmiedObj;
    }

    public virtual void Init()
    {
        OnDimmied = true;
        if (CurrentPopupObj != null)
        {
            InstPopupStack.Push(CurrentPopupObj);
            CurrentPopupObj.gameObject.SetActive(false);
            CurrentPopupObj = null;
        }

        // popupAnimation실행
        if (mPopupAni != EPopupAnimation.None)
        {
            mAniFunc = null;
            mTime = 0;
            switch (mPopupAni)
            {
                case EPopupAnimation.Pop:
                    transform.localScale = Vector3.zero;
                    mAniFunc = PopupAni_Pop;
                    break;
                case EPopupAnimation.Slide:
                    break;
            }
        }

        CurrentPopupObj = this;
    }

    public virtual bool ClosePopup(bool bDestroy = false)
    {
        // 끄거나 지우기
        DestroyPopup(bDestroy);

        // 이전 팝업 꺼내기
        if (InstPopupStack.Count > 0)
        {
            CurrentPopupObj = InstPopupStack.Pop();
            OnDimmied = true;
            CurrentPopupObj.gameObject.SetActive(true);
            return false;
        }

        // 이전 팝업 없다면 끄기
        OnDimmied = false;
        return true;
    }

    public void DestroyPopup(bool bDestroy)
    {
        // 지우고자 하는 오브젝트가 현재 활성화 오브젝트 이면
        if (this == CurrentPopupObj)
        {
            CurrentPopupObj = null;
        }

        // 지우거나 끄기
        if (bDestroy)
        {
            Destroy(gameObject);
        }
        else
        {
            GameObjectPool.ReturnObject(gameObject);
        }
    }

    private float mTime;
    private System.Action mAniFunc;

    private void Update()
    {
        if(mAniFunc == null) { return; }
        mTime += Time.unscaledDeltaTime;
        mAniFunc.Invoke();
    }

    private void PopupAni_Pop()
    {
        float value = (mTime / 0.5f) * 3f;
        transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, mPopupAniCurve.Evaluate(value));
        if(value >= 1) { mAniFunc = null; }
    }
}
