using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEffect : BlockEffect
{
    public override float EffectDuration { get => mDuration; }
    public override WaitForSeconds YieldEffectDuration { get => mYieldEffectDuration; }

    [SerializeField] LineRenderer mLine;
    [SerializeField] private AnimationCurve mAniCurve;

    private Vector3 mStartPos;
    private Vector3 mTargetPos;
    private float mTime = 0f;
    private static float mDuration = 0.5f;
    private static WaitForSeconds mYieldEffectDuration = new WaitForSeconds(mDuration);
    
    public override void SetEffectDataByData(Vector3 startPos, Vector3 targetPos, Sprite spriteOrNull = null)
    {
        mStartPos = startPos;
        mTargetPos = targetPos;
        mLine.SetPosition(0, mStartPos);
        mLine.SetPosition(1, mStartPos);
    }
    public override void PlayEffect()
    {
        StartCoroutine(PlayEffectCoroutine());
    }
    private IEnumerator PlayEffectCoroutine()
    {
        mLine.SetPosition(1, mTargetPos);
        mTime = 0f;
        while (mTime < 1f)
        {
            mTime += Time.deltaTime / mDuration;
            mLine.startWidth = 0.3f * mAniCurve.Evaluate(mTime);
            yield return null;
        }
        GameObjectPool.ReturnObject(gameObject);
    }
}
