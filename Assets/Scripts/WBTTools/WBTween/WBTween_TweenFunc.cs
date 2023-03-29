using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  Tween 동작 함수들
/// </summary>
public partial class BTTransformTween
{
    public enum ETweenType
    {
        Move = 0,
        Move_Local,
        Scale
    }

    public ETweenType TweenType
    {
        set
        {
            switch (value)
            {
                case ETweenType.Move:
                    mTweenFunc = Move;
                    break;
                case ETweenType.Move_Local:
                    mTweenFunc = MoveLocal;
                    break;
                case ETweenType.Scale:
                    mTweenFunc = Scale;
                    break;
            }
        }
    }

    private void Move(float value)
    {
        mTransform.position = Vector3.LerpUnclamped(mFrom, mTo, mCurve.Evaluate(value));
    }
    private void MoveLocal(float value)
    {
        mTransform.localPosition = Vector3.LerpUnclamped(mFrom, mTo, mCurve.Evaluate(value));
    }
    private void Scale(float value)
    {
        mTransform.localScale = Vector3.LerpUnclamped(mFrom, mTo, mCurve.Evaluate(value));
    }
}
