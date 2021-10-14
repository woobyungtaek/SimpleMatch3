using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockEffect : MonoBehaviour
{
    public abstract float EffectDuration { get; }
    public abstract WaitForSeconds YieldEffectDuration { get; }

    public abstract void SetEffectDataByData(Vector3 startPos, Vector3 targetPos, Sprite spriteOrNull = null);
    public abstract void PlayEffect();
}
