using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutoStepData : System.IDisposable
{
    public string StepType;
    public string InfoText;
    public string TargetItem;
    public Vector2 SelectCoordi;
    public Vector2 TargetCoordi;

    public Vector2 SpeechBubblePos;
    public Vector2 SpeechBubbleScale;

    public Vector2 MaskStartPos;
    public Vector2 MaskEndPos;

    public string MaskedObjectName;
    public Vector2 MaskSize;

    public bool IsDimmed = false;
    public bool IsSizeCal = false;
    public bool IsPoint = false;

    public void Dispose()
    {
        StepType = null;
        InfoText = null;
        TargetItem = null;
        MaskedObjectName = null;
        IsSizeCal = false;
        IsDimmed = false;
    }
}
