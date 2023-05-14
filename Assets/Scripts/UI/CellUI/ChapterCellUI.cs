using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterCellUI : RecycleCellUI
{
    [SerializeField] private Text mTestText;

    public override void Init(int index)
    {
        mTestText.text = $"{index}";
    }
}
