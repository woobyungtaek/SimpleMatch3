using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PopupAniInfo
{
    public EPopupAnimation PopupAniType;
    public AnimationCurve AniCurve;

    public Vector3 Start;
    public Vector3 End;

    public float Duration;
}
public enum EPopupAnimation
{
    None,
    Pop,
    Slide
}

public class Popup : MonoBehaviour
{
    protected static GameObject InputBlockDimmed;

    // 현재 활성화 된 팝업 오브젝트
    protected static Popup CurrentPopupObj;

    // 현재 우선순위가 밀린 팝업 오브젝트 스택(새 팝업이 켜질때 CurrentPopup이 목록에 들어간다)
    protected static Stack<Popup> InstPopupStack = new Stack<Popup>();

    protected static Vector3 GetDownPos(RectTransform rectTransform)
    {
        return new Vector3(0, rectTransform.rect.size.y * -1, 0);
    }
    protected static Vector3 GetUpPos(RectTransform rectTransform)
    {
        return new Vector3(0, rectTransform.rect.size.y, 0);
    }
    protected static Vector3 GetLeftPos(RectTransform rectTransform)
    {
        return new Vector3(rectTransform.rect.size.x * -1, 0, 0);
    }
    protected static Vector3 GetRightPos(RectTransform rectTransform)
    {
        return new Vector3(rectTransform.rect.size.x, 0, 0);
    }

    [Header("Popup Ani")]
    [SerializeField] protected PopupAniInfo mPopupAniInfo;
    [SerializeField] protected PopupAniInfo mCloseAniInfo;

    private System.Action mAniEndFunc;
    private bool mbDestroy;

    // 가리기 오브젝트
    protected static GameObject DimmiedObj;
    protected bool OnDimmied
    {
        set
        {
            if (DimmiedObj != null) { DimmiedObj.SetActive(value); }
        }
    }

    // 외부
    public static void SetPopupInfo(GameObject dimmiedObj)
    {
        DimmiedObj = dimmiedObj;
        if(InputBlockDimmed == null)
        {
            InputBlockDimmed = new GameObject("InputBlockDimmed");
            InputBlockDimmed.transform.SetParent(DimmiedObj.transform.parent.parent);
            var rect = InputBlockDimmed.AddComponent<RectTransform>();
            rect.position = Vector3.zero;
            rect.anchorMin = Vector3.zero;
            rect.anchorMax = Vector3.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = InputBlockDimmed.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0, 0, 0, 0);
            InputBlockDimmed.SetActive(false);
        }
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
        PopupAnimationPlay(mPopupAniInfo);

        CurrentPopupObj = this;
    }

    public virtual bool ClosePopup(bool bCloseAniPlay = false, bool bDestroy = false)
    {
        mbDestroy = bDestroy;
        if (mCloseAniInfo.PopupAniType != EPopupAnimation.None && bCloseAniPlay)
        {
            PopupAnimationPlay(mCloseAniInfo);
            mAniEndFunc = DoClosePopup;
            return true;
        }

        DoClosePopup();
        return true;
    }

    private void DoClosePopup()
    {

        // 끄거나 지우기
        DestroyPopup(mbDestroy);

        // 이전 팝업 꺼내기
        if (InstPopupStack.Count > 0)
        {
            CurrentPopupObj = InstPopupStack.Pop();
            OnDimmied = true;
            CurrentPopupObj.gameObject.SetActive(true);
            return;
        }

        // 이전 팝업 없다면 끄기
        OnDimmied = false;
    }

    protected void DestroyPopup(bool bDestroy)
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


    private void PopupAnimationPlay(PopupAniInfo aniInfo)
    {
        mAniEndFunc = null;
        
        
        if(aniInfo.PopupAniType == EPopupAnimation.None)
        {
            InputBlockDimmed.SetActive(false);
            return;
        }
        InputBlockDimmed.SetActive(true);

        switch (aniInfo.PopupAniType)
        {
            case EPopupAnimation.Pop:
                StartCoroutine(PopupAni_Pop(aniInfo));
                break;
            case EPopupAnimation.Slide:
                StartCoroutine(PopupAni_Slide(aniInfo));
                break;
            default:
                break;
        }
    }

    private IEnumerator PopupAni_Pop(PopupAniInfo info)
    {
        transform.localScale = info.Start;
        float time = 0;
        float value = 0;
        while (value < 1f)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            value = time / info.Duration;
            transform.localScale = Vector3.LerpUnclamped(info.Start, info.End, info.AniCurve.Evaluate(value));
        }

        InputBlockDimmed.SetActive(false);
        mAniEndFunc?.Invoke();
    }

    private IEnumerator PopupAni_Slide(PopupAniInfo info)
    {
        transform.localPosition = info.Start;
        float time = 0;
        float value = 0;

        while (value < 1f)
        {
            yield return null;
            time += Time.unscaledDeltaTime;
            value = time / 0.5f;
            transform.localPosition = Vector3.LerpUnclamped(info.Start, info.End, info.AniCurve.Evaluate(value));
        }

        InputBlockDimmed.SetActive(false);
        mAniEndFunc?.Invoke();
    }
}
