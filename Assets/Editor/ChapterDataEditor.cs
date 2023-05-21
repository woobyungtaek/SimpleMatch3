using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ChapterDataEditor : EditorWindow
{
    private class PartEditData
    {
        public AnimationCurve mDiffCurve = new AnimationCurve();
        public int mStageCount;
        public int mMaxDiff;

        public PartData GetPartData()
        {
            PartData data = new PartData();

            float timeValue = 1f / (mStageCount - 1);
            for (int cnt = 0; cnt < mStageCount; ++cnt)
            {
                Debug.Log($"{cnt * timeValue}");
                float evalute = mDiffCurve.Evaluate(cnt * timeValue);
                EMissionLevel level = (EMissionLevel)Mathf.FloorToInt(evalute * mMaxDiff);
                data.list.Add(level);
            }

            return data;
        }
    }

    private int mDiffGradeCount;
    private string mFileName;
    private List<PartEditData> mPartEditList = new List<PartEditData>();

    [MenuItem("DataEditor/ChapterDataEdit")]
    public static void Init_ChapterDataEditor()
    {
        ChapterDataEditor chapterDataEditor = (ChapterDataEditor)GetWindow(typeof(ChapterDataEditor));
        chapterDataEditor.Show();

    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Part Add"))
        {
            mPartEditList.Add(new PartEditData());
        }
        if (GUILayout.Button("Part Remove"))
        {
            mPartEditList.RemoveAt(mPartEditList.Count - 1);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(25);

        for (int cnt = 0; cnt < mPartEditList.Count; ++cnt)
        {
            GUILayout.Label($"{cnt}", EditorStyles.boldLabel);

            mPartEditList[cnt].mDiffCurve = EditorGUILayout.CurveField("Diff Curve", mPartEditList[cnt].mDiffCurve, GUILayout.Width(400), GUILayout.Height(100));
            mPartEditList[cnt].mStageCount = EditorGUILayout.IntField("Stage Count", mPartEditList[cnt].mStageCount, GUILayout.Width(250));
            mPartEditList[cnt].mMaxDiff = EditorGUILayout.IntField("Max Diff", mPartEditList[cnt].mMaxDiff, GUILayout.Width(250));
            GUILayout.Space(10);
        }
        GUILayout.Space(25);
        mFileName = EditorGUILayout.TextField("FileName", mFileName, GUILayout.Width(250));
        if (GUILayout.Button("Create", GUILayout.Width(250)))
        {
            SaveData();
        }

        GUILayout.Space(30);

        GUILayout.EndVertical();
    }

    private void SaveData()
    {
        string path = "Assets/ScriptableObjects/ChapterStage";
        Directory.CreateDirectory(path);

        var inst = ScriptableObject.CreateInstance<ChapterData>();
        for (int cnt = 0; cnt < mPartEditList.Count; ++cnt)
        {
            inst.MissionLevelList.Add(mPartEditList[cnt].GetPartData());
        }
        string assetPath = Path.Combine(path, mFileName + ".asset");
        AssetDatabase.CreateAsset(inst, assetPath);
        AssetDatabase.SaveAssets();
    }
}
