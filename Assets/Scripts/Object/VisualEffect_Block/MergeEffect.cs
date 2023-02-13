using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeEffect : BlockEffect
{
    public override float EffectDuration { get => mDuration; }
    public override WaitForSeconds YieldEffectDuration { get => GameConfig.yieldMergeDuration; }
    
    private Vector3 mStartPos;
    private Vector3 mTargetPos;
    private float mDuration = 0f;
    private float mTime = 0f;

    [SerializeField] private AnimationCurve mAniCurve = new AnimationCurve(new Keyframe(0, 0, 0, Mathf.Tan(-45)), new Keyframe(1, 1, Mathf.Tan(45), 0));
    [SerializeField] private SpriteRenderer mEffectSprite;

    public override void SetEffectDataByData(Vector3 startPos, Vector3 targetPos, Sprite spriteOrNull = null)
    {
        mStartPos = startPos;
        transform.position = mStartPos;
        mTargetPos = targetPos;
        mDuration = GameConfig.MERGE_DURATION;
        mEffectSprite.sprite = spriteOrNull;
    }
    public override void PlayEffect()
    {
        StartCoroutine(MovePositionCoroutine());
    }

    private IEnumerator MovePositionCoroutine()
    {
        mTime = 0f;
        while (mTime < 1)
        {
            mTime += Time.deltaTime / mDuration;
            transform.position = Vector3.LerpUnclamped(mStartPos, mTargetPos, mAniCurve.Evaluate(mTime));
            yield return null;
        }

        transform.position = mTargetPos;
        GameObjectPool.ReturnObject(gameObject);
    }
}
