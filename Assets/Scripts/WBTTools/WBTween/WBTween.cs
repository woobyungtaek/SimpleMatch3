using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 변수 및 기본 함수들
/// </summary>
public partial class BTTransformTween
{
    #region static

    private static AnimationCurve[] EaseCurveArr =
    {
            new AnimationCurve( new Keyframe(0,0), new Keyframe(1,1) )
        };

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

    private float mDuration;
    private float mDelay;

    private AnimationCurve mCurve;

    private Coroutine mAniCoroutine;

    private System.Action<float> mTweenFunc;
    private System.Action mCompleteFunc;
    private System.Action mStartFunc;

    public Vector3 From { set => mFrom = value; }
    public Vector3 To { set => mTo = value; }
    public float Duration { set => mDuration = value; }


    public void Init(Transform transform)
    {
        mTransform = transform;

        mDelay = 0f;

        mCurve = EaseCurveArr[0];
        mAniCoroutine = null;

        mCompleteFunc = null;
        mStartFunc = null;
    }

    private IEnumerator TweenBasicFunc()
    {
        // 긴가민가한데... 일단 1프레임 쉬고 들어간다.
        yield return null;

        // 시작 메서드 실행
        mStartFunc?.Invoke();

        float time = 0f;
        float value = 0f;

        //>> 딜레이
        if (mDelay > 0f)
        {
            while (value < 1f)
            {
                time += Time.deltaTime;
                value = time / mDelay;
                yield return null;
            }
        }

        //>> 동작
        time = 0f;
        value = 0f;
        while (value < 1f)
        {
            // Pause가 걸리면 어케 처리해야 할까. deltatime을 따로 변수로해서 0으로 만들거나
            // 아예 bool로 처리하거나
            time += Time.deltaTime;
            value = time / mDuration;

            // 실행할 함수
            mTweenFunc?.Invoke(value);
            yield return null;
        }

        //>> 끝            
        // 끝 메서드 실행
        mCompleteFunc?.Invoke();

        // 반환 처리
        ReturnQueue();
    }

    private void ReturnQueue()
    {
        mAniCoroutine = null;
        TweenQueue.Enqueue(this);
    }

}
