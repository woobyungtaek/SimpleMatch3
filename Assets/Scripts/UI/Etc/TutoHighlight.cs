using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoHighlight : MonoBehaviour
{
    public string InfoText
    {
        set
        {
            mInfoText.text = value;
        }
    }

    public Vector2 CanvasSize;

    private static float mCellSize = -1;

    [SerializeField] private Image         mDimmed;
    [SerializeField] private Image         mInfoImage;
    [SerializeField] private Text          mInfoText;
    [SerializeField] private RectTransform mMask;
    [SerializeField] private PointHand     mPointHand;

    public void SetMaskPositoin(TutoStepData stepData, bool bCoordi)
    {
        mPointHand.gameObject.SetActive(stepData.IsPoint);

        Vector2 start = stepData.MaskStartPos;
        Vector2 end   = stepData.MaskEndPos;

        mInfoText.text = stepData.InfoText;
        mInfoImage.rectTransform.sizeDelta = new Vector2(CanvasSize.x * stepData.SpeechBubbleScale.x, CanvasSize.y * stepData.SpeechBubbleScale.y);
        mInfoImage.transform.localPosition = new Vector3(CanvasSize.x * stepData.SpeechBubblePos.x, CanvasSize.y * stepData.SpeechBubblePos.y, 0);

        mDimmed.gameObject.SetActive(false);
        if (stepData.IsDimmed)
        {
            mDimmed.gameObject.SetActive(true);
        }

        if(bCoordi == true)
        {
            Tile startTile = TileMapManager.Instance.GetTileByCoordiOrNull(start);
            Tile endTile = TileMapManager.Instance.GetTileByCoordiOrNull(end);
            if(startTile == null) { return; }
            if(endTile == null) { return; }

            float x = (end - start).x;
            if (x < 0) { x *= -1; }
            float y = (end - start).y;
            if (y < 0) { y *= -1; }

            start = startTile.transform.position;
            end = endTile.transform.position;

            #region 1칸당 크기를 다시 계산한다.
            mMask.position = start;
            Vector3 startPos = mMask.transform.localPosition;
            mMask.position = end;
            Vector3 endPos = mMask.transform.localPosition;

            float divine = x;
            mCellSize = startPos.x - endPos.x;
            if(mCellSize == 0)
            {
                mCellSize = startPos.y - endPos.y;
                divine = y;
            }
            if (mCellSize < 0)
            {
                mCellSize *= -1;
            }
            mCellSize /= divine;
            #endregion

            mMask.sizeDelta = new Vector3(mCellSize, mCellSize, 1);
            mMask.localScale = new Vector3(x +1, y +1, 1);
            mMask.position = start + ((end - start) / 2f);
        }
        gameObject.SetActive(true);

        if (stepData.IsPoint)
        {
            Vector3 select = TileMapManager.Instance.GetTileByCoordiOrNull(stepData.SelectCoordi).transform.position;
            Vector3 target = TileMapManager.Instance.GetTileByCoordiOrNull(stepData.TargetCoordi).transform.position;

            mPointHand.SetSwapAnimation(select, target);
            mPointHand.StartSwapAnimation();
        }
    }
}
