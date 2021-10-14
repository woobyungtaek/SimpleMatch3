using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointHand : MonoBehaviour
{
    private Vector3 mStart;
    private Vector3 mEnd;

    private float mSwapDuration = 1.5f;
    private float mPointDuration = 1.6f;
    private float mTime;

    [SerializeField] private AnimationCurve mMoveCurve;
    [SerializeField] private AnimationCurve mPointCurve;
    [SerializeField] private Transform mArrowPivot;
    [SerializeField] private Image mHandImage;

    public void FlipXImage(bool bFlip)
    {
        if(mHandImage == null) { mHandImage = gameObject.GetComponent<Image>(); }
        float flipX = 1;
        if (bFlip) { flipX = -1; }
        mHandImage.rectTransform.localScale = new Vector3(flipX, 1, 1);
    }

    public void SetSwapAnimation(Vector2 startPos, Vector2 endPos)
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        transform.position = startPos;
        mStart = transform.localPosition;
        transform.position = endPos;
        mEnd = transform.localPosition;

        transform.localEulerAngles = Vector3.zero;
        if ((mEnd - mStart).x == 0)
        {
            transform.localEulerAngles = Vector3.forward * -90f;
            mArrowPivot.localEulerAngles = Vector3.forward * 0f;
            if ((mEnd - mStart).y > 0)
            {
                mArrowPivot.localEulerAngles = Vector3.forward * 180f;
            }
        }
        else
        {
            mArrowPivot.localEulerAngles = Vector3.forward * 0f;
            if ((mEnd - mStart).x < 0)
            {
                mArrowPivot.localEulerAngles = Vector3.forward * 180f;
            }
        }

        mTime = 0;
    }
    public void StartSwapAnimation()
    {
        gameObject.SetActive(true);
        StartCoroutine(SwapAnimation());
    }
    private IEnumerator SwapAnimation()
    {
        mTime = 0f;
        while (true)
        {
            mTime += Time.deltaTime / mSwapDuration;
            transform.localPosition = Vector3.LerpUnclamped(mStart, mEnd, mMoveCurve.Evaluate(mTime));
            yield return null;
            if(mTime >= 1)
            {
                mTime = 0;
            }
        }
    }

    public void SetPointAnimation(Vector2 startPos, Vector2 endPos)
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        endPos = startPos + endPos.normalized * (mHandImage.rectTransform.sizeDelta * 0.66f);        
        mStart = endPos;
        mEnd = startPos;
    }
    public void StartPointAnimation()
    {
        gameObject.SetActive(true);
        StartCoroutine(PointAnimation());
    }
    private IEnumerator PointAnimation()
    {
        mTime = 0f;
        while (true)
        {
            mTime += Time.deltaTime / mPointDuration;
            transform.localPosition = Vector3.LerpUnclamped(mStart, mEnd, mPointCurve.Evaluate(mTime));
            yield return null;
            if (mTime >= 1)
            {
                mTime = 0;
            }
        }
    }
}
