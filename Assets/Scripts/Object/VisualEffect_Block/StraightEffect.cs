using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightEffect : BlockEffect
{
    public override float EffectDuration { get => mDuration; }
    public override WaitForSeconds YieldEffectDuration { get => mYieldEffectDuration; }

    private int loopCount = 0;
    private float mTime = 0f;

    private static float mDuration = 0.5f;
    private static WaitForSeconds mYieldEffectDuration = new WaitForSeconds(mDuration / 3);

    [SerializeField] private AnimationCurve mAniCurve = new AnimationCurve();
    [SerializeField] private List<GameObject> mEffectObject = new List<GameObject>();
    
    public override void SetEffectDataByData(Vector3 startPos, Vector3 targetPos, Sprite spriteOrNull = null)
    {
        transform.position = startPos;
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
            mEffectObject[index].transform.localPosition = Vector3.zero;
        }

        while (mTime < 1)
        {
            mTime += Time.deltaTime / mDuration;
            for (int index = 0; index < loopCount; index++)
            {
                mEffectObject[index].transform.Translate(mEffectObject[index].transform.up * mAniCurve.Evaluate(mTime),Space.World);
            }
            yield return null;
        }
        GameObjectPool.ReturnObject(gameObject);
    }
}
