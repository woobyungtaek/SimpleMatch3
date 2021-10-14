using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCollectEffect : MonoBehaviour
{
    public static WaitForSeconds MaxDuration = new WaitForSeconds(MaxSec);
    
    private static float MaxSec= 1.25f;
    private static float twoPI = 360 * Mathf.Deg2Rad;
    private static WaitForSeconds mYieldEffectDuration;// = new WaitForSeconds(Duration);

    public WaitForSeconds YieldEffectDuration { get => mYieldEffectDuration; }

    [Header("Mission")]
    [SerializeField]
    private SpriteRenderer mCollectImage;

    [Header("Animation")]
    private Vector3     mStartPos;
    private Vector3     mTargetPos;
    private Vector3     point1Pos = Vector3.zero;
    private Vector3     point2Pos = Vector3.zero;
    private float       mTime = 0f;
    private float mDuration = 1f;
    private Matrix4x4   mCGMatrix = new Matrix4x4();



    [SerializeField]
    private AnimationCurve mAniCurve = new AnimationCurve();
    [SerializeField]
    private AnimationCurve mSclaeCurve = new AnimationCurve();

    public void Awake()
    {
        mDuration = Random.Range(0.8f, MaxSec);
        mYieldEffectDuration = new WaitForSeconds(mDuration);
    }

    public void SetEffectDataByData(Vector3 startPos, Vector3 targetPos, Sprite spriteOrNull = null)
    {
        mStartPos = startPos;
        transform.position = mStartPos;
        mTargetPos = targetPos;
        mCollectImage.transform.localScale = Vector3.one;

        //Vector3 dir = mTargetPos - mStartPos;
        //float angle = Mathf.Atan2(dir.y, dir.x);
        //if (angle < 0) { angle += twoPI; }
        //bool bFlip = false;
        //if (Random.Range(0, 2) % 2 == 1) { bFlip = true; }
        //CreatePointByAngle(ref point1Pos, angle, Random.Range(155f, 165f), bFlip,false);
        //CreatePointByAngle(ref point2Pos, angle, Random.Range(85f, 95f), bFlip,false);        
        //point1Pos += mStartPos;
        //point2Pos += mStartPos;
        //CurveMatrix.CreateBezierCurveCubic(ref mCGMatrix, mStartPos, point1Pos, point2Pos, mTargetPos);

        mCollectImage.sprite = spriteOrNull;
    }
    public void PlayEffect()
    {
        StartCoroutine(PlayEffectCoroutine());
    }
    private IEnumerator PlayEffectCoroutine()
    {
        mStartPos = transform.position;
        mCollectImage.transform.localScale = Vector3.one;

        mTime = 0f;
        while (mTime < 1)
        {
            mTime += Time.deltaTime / mDuration;
            transform.position = Vector3.Lerp(mStartPos, mTargetPos, mAniCurve.Evaluate(mTime));
            mCollectImage.transform.localScale = Vector3.one +( Vector3.one * mSclaeCurve.Evaluate(mTime));

            yield return null;
        }
        transform.position = mTargetPos;

        ObserverCenter.Instance.SendNotification(this, Message.RefreshMissionCellUI);
        GameObjectPool.Destroy(gameObject);
    }

    #region 포물선 움직임
    //private IEnumerator PlayEffectCoroutine()
    //{
    //    float t;
    //    Vector4 timeVector = new Vector4(0, 0, 0, 1);
    //    mTime = 0f;
    //    while (mTime < 1f)
    //    {
    //        mTime += Time.deltaTime / mDuration;
    //        t = mAniCurve.Evaluate(mTime);

    //        timeVector.x = t * t * t;
    //        timeVector.y = t * t;
    //        timeVector.z = t;

    //        transform.position = mCGMatrix * timeVector;

    //        yield return null;
    //    }
    //    transform.position = mTargetPos;
    //    GameObjectPool.Destroy(gameObject);
    //}
    //private void CreatePointByAngle(ref Vector3 angleVector, float dirAngle, float pointAngle, bool bFlip, bool bUseZ)
    //{
    //    if (bFlip) { pointAngle = 360 - pointAngle; }
    //    pointAngle *= Mathf.Deg2Rad;
    //    pointAngle += dirAngle;
    //    angleVector.x = Mathf.Cos(pointAngle);
    //    angleVector.y = Mathf.Sin(pointAngle);
    //    if(bUseZ== false)
    //    {
    //        angleVector.z = 0;
    //    }

    //    angleVector *= Random.Range(3f, 5f);
    //}
    #endregion
}
