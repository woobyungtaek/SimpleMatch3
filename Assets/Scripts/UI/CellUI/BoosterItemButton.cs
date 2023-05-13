using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoosterItemButton : Button
{
    [Header("BoosterItem")]
    [SerializeField] private Image mItemImage;
    [SerializeField] private TextMeshProUGUI mItemNameText;
    [SerializeField] private int mInventoryIndex;

    public int GetInvenIndex
    {
        get => mInventoryIndex;
    }

    public void Init()
    {
        mInventoryIndex = -1;
        SetButtonByIndex(mInventoryIndex);

        onClick.RemoveAllListeners();
    }

    public void SetButtonByIndex(int invenIdx)
    {
        mInventoryIndex = invenIdx;
        if (mInventoryIndex < 0)
        {
            mItemNameText.text = "Select Item";
            return;
        }
        
        // �׽�Ʈ�� ������ ��ȣ�� ����ϱ�
        mItemNameText.text = $"{invenIdx}";

        // �κ��丮�� ���� BoosterItem�� ������ Index�� �˾ƿͼ�
        // �������� �̸��� �־������
    }
}
