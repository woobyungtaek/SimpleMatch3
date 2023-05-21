using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ChapterLevelEditor : EditorWindow
{
    private int mMoveCount_PartStart;
    private int mMoveCount_StageClear;

    private Object mChapterData;
    private ChapterData mData;

    private int[] mValueArr = { 3, 4, 5, 8, 16 };
    private AnimationCurve[] mCurveArr = { new AnimationCurve(), new AnimationCurve(), new AnimationCurve() };

    [MenuItem("DataEditor/ChapterLevelEditor")]
    public static void Init_LevelEditor()
    {
        ChapterLevelEditor chapterLevelEditor = (ChapterLevelEditor)GetWindow(typeof(ChapterLevelEditor));
        chapterLevelEditor.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(25);
        GUILayout.BeginVertical();

        mChapterData = EditorGUILayout.ObjectField(mChapterData, typeof(ChapterData), true, GUILayout.Width(200), GUILayout.Height(20));
        if (mChapterData != null)
        {
            if (GUILayout.Button("Calculate", GUILayout.Width(200), GUILayout.Height(20)))
            {
                mData = mChapterData as ChapterData;
            }
        }

        // MoveCount ют╥б
        GUILayout.BeginHorizontal();
        mMoveCount_PartStart = EditorGUILayout.IntField("PartStart", mMoveCount_PartStart, GUILayout.Width(200), GUILayout.Height(20));
        mMoveCount_StageClear = EditorGUILayout.IntField("StageClear", mMoveCount_StageClear, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        if (mData != null)
        {
            for (int idx = 0; idx < mData.PartCount; ++idx)
            {
                for (int cnt = mCurveArr[idx].keys.Length - 1; cnt >= 0; --cnt)
                {
                    mCurveArr[idx].RemoveKey(cnt);
                }
            }

            float curveValue = 1f / (float)EMissionLevel.VeryHard;
            for (int idx = 0; idx < mData.PartCount; ++idx)
            {
                GUILayout.BeginHorizontal();
                int value = 0;
                GUILayout.Label($"Part{idx} : ", EditorStyles.boldLabel, GUILayout.Width(55), GUILayout.Height(20));

                float time = 0f;
                float timeValue = 1f / mData.MissionLevelList[idx].list.Count;

                for (int levelIdx = 0; levelIdx < mData.MissionLevelList[idx].list.Count; ++levelIdx)
                {
                    EMissionLevel level = (EMissionLevel)EditorGUILayout.EnumPopup(mData.MissionLevelList[idx].list[levelIdx], GUILayout.Width(100), GUILayout.Height(20));
                    mData.MissionLevelList[idx].list[levelIdx] = level;

                    int levelInt = (int)level;
                    mCurveArr[idx].AddKey(time, levelInt * curveValue);
                    time += timeValue;

                    value += mValueArr[levelInt];
                }

                int stageCount = mData.MissionLevelList[idx].list.Count;
                int total = (stageCount - 1) * mMoveCount_StageClear + mMoveCount_PartStart;
                GUILayout.Label($" {value} / {total} ", EditorStyles.boldLabel, GUILayout.Width(200), GUILayout.Height(20));

                GUILayout.EndHorizontal();
            }

            for (int idx = 0; idx < mData.PartCount; ++idx)
            {
                for(int cnt = 0; cnt < mCurveArr[idx].keys.Length; ++cnt )
                {
                    AnimationUtility.SetKeyLeftTangentMode(mCurveArr[idx], cnt, AnimationUtility.TangentMode.Linear);
                    AnimationUtility.SetKeyRightTangentMode(mCurveArr[idx], cnt, AnimationUtility.TangentMode.Linear);
                }
                EditorGUILayout.CurveField($"Part{idx} : ", mCurveArr[idx], GUILayout.Width(400), GUILayout.Height(100));
            }
        }


        GUILayout.EndVertical();
    }
}
