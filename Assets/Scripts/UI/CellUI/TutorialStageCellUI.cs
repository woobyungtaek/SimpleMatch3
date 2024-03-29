﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStageCellUI : MonoBehaviour
{
    private const string CONCEPT_NAME = "Tutorial";

    [SerializeField] private Text mButtonText;
    [SerializeField] private int mTutoIndex;

    public void SetCellInfo(string info, int index)
    {
        mButtonText.text = info;
        mTutoIndex = index;
    }
    public void OnTutoStageButtonClicked()
    {
        //InGameUseDataManager.Instance.ConceptName = CONCEPT_NAME;
        //InGameUseDataManager.Instance.MapName = string.Format("TutorialMap_{0}", mTutoIndex);

        LobbySceneManager.Instance.LoadGameScene();
    }
}
