using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WBTWeen;

/// <summary>
/// 변수 및 기본 함수들
/// </summary>
public partial class BTTransformTween : TweenClass
{
    #region static

    private static Queue<BTTransformTween> TweenQueue = new Queue<BTTransformTween>();

    public static BTTransformTween GetTween(Transform transform)
    {
        BTTransformTween tween = null;

        if (TweenQueue.Count > 0)
        {
            tween = TweenQueue.Dequeue();
        }
        if (tween == null)
        {
            tween = new BTTransformTween();
        }

        tween.Init(transform);

        return tween;
    }

    #endregion

    private Transform mTransform;

    private Vector3 mFrom;
    private Vector3 mTo;


    public Vector3 From     { set => mFrom = value; }
    public Vector3 To       { set => mTo = value; }
    public float Duration   { set => mDuration = value; }


    public void Init(Transform transform)
    {
        mTransform = transform;

        mDelay = 0f;

        mCurve = WBTween.EaseCurveArr[0];
        mAniCoroutine = null;

        mCompleteFunc = null;
        mStartFunc = null;
    }

    protected override void ReturnQueue()
    {
        mAniCoroutine = null;
        TweenQueue.Enqueue(this);
    }

}
