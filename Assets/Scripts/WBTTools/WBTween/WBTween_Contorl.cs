using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  추가 설정 함수들
///  Tween 컨트롤 함수들
/// </summary>
public partial class BTTransformTween
{
    public BTTransformTween OnComplete(System.Action complete)
    {
        mCompleteFunc = complete;
        return this;
    }
    public BTTransformTween OnStart(System.Action start)
    {
        mStartFunc = start;
        return this;
    }


    public BTTransformTween SetEase(AnimationCurve ease)
    {
        mCurve = ease;
        return this;
    }
    public BTTransformTween SetDelay(float delay)
    {
        mDelay = delay;
        return this;
    }


    public void Play()
    {
        mAniCoroutine = Coroutine_Helper.StartCoroutine(TweenBasicFunc());
    }
    public void Stop()
    {
        Coroutine_Helper.StopCoroutine(mAniCoroutine);
        ReturnQueue();
    }
}
