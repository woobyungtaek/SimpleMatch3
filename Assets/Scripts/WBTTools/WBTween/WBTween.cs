using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �� �⺻ �Լ���
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
        // �䰡�ΰ��ѵ�... �ϴ� 1������ ���� ����.
        yield return null;

        // ���� �޼��� ����
        mStartFunc?.Invoke();

        float time = 0f;
        float value = 0f;

        //>> ������
        if (mDelay > 0f)
        {
            while (value < 1f)
            {
                time += Time.deltaTime;
                value = time / mDelay;
                yield return null;
            }
        }

        //>> ����
        time = 0f;
        value = 0f;
        while (value < 1f)
        {
            // Pause�� �ɸ��� ���� ó���ؾ� �ұ�. deltatime�� ���� �������ؼ� 0���� ����ų�
            // �ƿ� bool�� ó���ϰų�
            time += Time.deltaTime;
            value = time / mDuration;

            // ������ �Լ�
            mTweenFunc?.Invoke(value);
            yield return null;
        }

        //>> ��            
        // �� �޼��� ����
        mCompleteFunc?.Invoke();

        // ��ȯ ó��
        ReturnQueue();
    }

    private void ReturnQueue()
    {
        mAniCoroutine = null;
        TweenQueue.Enqueue(this);
    }

}
