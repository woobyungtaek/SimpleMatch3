using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundEffect : BlockEffect
{
    public override float EffectDuration { get => mDuration; }
    public override WaitForSeconds YieldEffectDuration { get => mYieldEffectDuration; }

    private Vector3 mScale;

    private int loopCount = 0;
    private float mTime = 0f;

    private static float mDuration = 0.3f;
    private static WaitForSeconds mYieldEffectDuration = new WaitForSeconds(mDuration/3);

    [SerializeField] private AnimationCurve mScaleCurve = new AnimationCurve();
    [SerializeField] private AnimationCurve mAlphaCurve = new AnimationCurve();
    [SerializeField] private List<SpriteRenderer> mEffectObject = new List<SpriteRenderer>();

    public override void SetEffectDataByData(Vector3 startPos, Vector3 targetPos, Sprite spriteOrNull = null)
    {
        transform.position = startPos;
        mScale = targetPos;
    }

    public override void PlayEffect()
    {
        StartCoroutine(PlayEffectCoroutine());
    }
    private IEnumerator PlayEffectCoroutine()
    {
        mTime = 0f;
        loopCount = mEffectObject.Count;
        for (int index = 0; index < loopCount; index++)
        {
            mEffectObject[index].transform.localScale = Vector3.zero;
        }

        while (mTime < 1)
        {
            mTime += Time.deltaTime / mDuration;
            for (int index = 0; index < loopCount; index++)
            {
                mEffectObject[index].color = new Color(1,1,1, mAlphaCurve.Evaluate(mTime));
                mEffectObject[index].gameObject.transform.localScale = mScale * mScaleCurve.Evaluate(mTime);
            }
            yield return null;
        }
        GameObjectPool.ReturnObject(gameObject);
    }
}
