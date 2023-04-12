using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Message
{
    //public static string Sample        = "Sample";
#if UNITY_EDITOR
    public static readonly string ClearMissionCheat          = "ClearMissionCheat";
#endif

    public static readonly string RefreshPlayerStatUI        = "RefreshPlayerStatUI";

    public static readonly string RefreshMission             = "RefreshMission";
    public static readonly string RefreshMissionCellUI       = "RefreshMissionCellUI";
    public static readonly string RefreshHammer              = "RefreshHammer";
    public static readonly string RefreshMoveCount           = "ChangeMoveCount";
                   
    public static readonly string SelectRewardCell           = "SelectRewardCell";
                   
    public static readonly string OnNormalMode               = "OnNormalMode";
    public static readonly string OnTutoMode                 = "OnTutoMode";
                   
    public static readonly string ChangeMapInfo              = "ChangeMapInfo";
    public static readonly string PauseGame                  = "PauseGame";
    public static readonly string ResumeGame                 = "ResumeGame";
                   
    public static readonly string DropEndCheck               = "DropEndCheck";
                   
    public static readonly string MainSkillIncrease          = "MainSkillIncrease";

    public static readonly string SkillDimmedOff             = "SkillDimmedOff";
    public static readonly string SkillDimmedOn              = "SkillDimmedOn";
                   
    public static readonly string CameraUp                   = "CameraUp";
    public static readonly string CharacterEnter             = "CharacterEnter";
    public static readonly string CharacterOut               = "CharacterOut";
}
