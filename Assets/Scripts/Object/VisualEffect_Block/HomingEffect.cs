using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingEffect : BlockEffect
{
    public override float EffectDuration { get => mDuration; }
    public override WaitForSeconds YieldEffectDuration { get => mYieldEffectDuration; }

    private Vector3 mStartPos;
    private Vector3 mTargetPos;
    private Vector3 point1Pos = Vector3.zero;
    private Vector3 point2Pos = Vector3.zero;
    private float mTime = 0f;
    private static float mDuration = 0.5f;
    private Matrix4x4 mCGMatrix = new Matrix4x4();

    [SerializeField] private SpriteRenderer mEffectSprite;
    [SerializeField] private AnimationCurve mAniCurve = new AnimationCurve();

    private static WaitForSeconds mYieldEffectDuration = new WaitForSeconds(mDuration);
    private static float twoPI = 360 * Mathf.Deg2Rad;

    public override void SetEffectDataByData(Vector3 startPos, Vector3 targetPos, Sprite spriteOrNull = null)
    {
        mStartPos = startPos;
        transform.position = mStartPos;
        mTargetPos = targetPos;

        Vector3 dir = mTargetPos - mStartPos;
        float angle = Mathf.Atan2(dir.y, dir.x);
        if (angle < 0) { angle += twoPI; }

        bool bFlip = false;
        if (Random.Range(0, 2) % 2 == 1) { bFlip = true; }

        CreatePointByAngle(ref point1Pos, angle, Random.Range(155f, 165f), bFlip, false);
        CreatePointByAngle(ref point2Pos, angle, Random.Range(85f, 95f), bFlip, false);

        point1Pos += mStartPos;
        point2Pos += mStartPos;

        CurveMatrix.CreateBezierCurveCubic(ref mCGMatrix, mStartPos, point1Pos, point2Pos, mTargetPos);

        mEffectSprite.sprite = spriteOrNull;
    }
    public override void PlayEffect()
    {
        StartCoroutine(PlayEffectCoroutine());
    }
    private IEnumerator PlayEffectCoroutine()
    {
        float t;
        Vector4 timeVector = new Vector4(0, 0, 0, 1);
        mTime = 0f;
        while (mTime < 1f)
        {
            mTime += Time.deltaTime / mDuration;
            t = mAniCurve.Evaluate(mTime);

            timeVector.x = t * t * t;
            timeVector.y = t * t;
            timeVector.z = t;

            transform.position = mCGMatrix * timeVector;

            yield return null;
        }
        transform.position = mTargetPos;
        GameObjectPool.ReturnObject(gameObject);
    }

    private void CreatePointByAngle(ref Vector3 angleVector, float dirAngle, float pointAngle, bool bFlip, bool bUseZ)
    {
        if (bFlip) { pointAngle = 360 - pointAngle; }
        pointAngle *= Mathf.Deg2Rad;
        pointAngle += dirAngle;
        angleVector.x = Mathf.Cos(pointAngle);
        angleVector.y = Mathf.Sin(pointAngle);
        if (bUseZ == false)
        {
            angleVector.z = 0;
        }

        angleVector *= Random.Range(3f, 5f);
    }
}
