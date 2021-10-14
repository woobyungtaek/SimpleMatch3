using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig
{
    public const float DROP_DURATION = 0.15f;
    public const float MERGE_DURATION = 0.2f;
    public const float CHANGE_EFFECT_DURATION = 0.5f;
    public const float GAME_OVER_DURATION = 0.5f;

    public static WaitForSeconds yieldDropDuraion           = new WaitForSeconds(DROP_DURATION);
    public static WaitForSeconds yieldMergeDuration         = new WaitForSeconds(MERGE_DURATION);
    public static WaitForSeconds yieldChangeEffectDuration  = new WaitForSeconds(CHANGE_EFFECT_DURATION);
    public static WaitForSeconds yieldGameOverDuration = new WaitForSeconds(GAME_OVER_DURATION);

    public static WaitForSeconds yieldGameEndDuration       = new WaitForSeconds(1.5f);
}
