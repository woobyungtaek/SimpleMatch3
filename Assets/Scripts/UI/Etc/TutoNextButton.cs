using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoNextButton : MonoBehaviour
{
    private const string RESERV_PUZZLEBOARD = "PuzzleBoard";
    private const string RESERV_MISSIONCELL = "MissionCellUI";
    private const string RESERV_MOVECOUNT = "MoveCountUI";

    public Image DimmedImage
    {
        get
        {
            return mDimmed;
        }
    }
    public Button NextButton
    {
        get
        {
            return mNextButton;
        }   
    }
    public Text TutoInfoText
    {
        get
        {
            return mTutoInfoText;
        }
    }
    public Vector2 CanvasSize;

    [SerializeField] private Image mDimmed;
    [SerializeField] private Image mMask;
    [SerializeField] private Button mNextButton;
    [SerializeField] private Image mInfoImage;
    [SerializeField] private Text mTutoInfoText;
    [SerializeField] private PointHand mPointHand;

    public void SetTutoInfo(TutoStepData stepData)
    {
        mMask.gameObject.SetActive(false);
        mPointHand.gameObject.SetActive(stepData.IsPoint);

        mTutoInfoText.text = stepData.InfoText;
        mInfoImage.rectTransform.sizeDelta = new Vector2(CanvasSize.x * stepData.SpeechBubbleScale.x, CanvasSize.y * stepData.SpeechBubbleScale.y);
        mInfoImage.transform.localPosition = new Vector3(CanvasSize.x * stepData.SpeechBubblePos.x, CanvasSize.y * stepData.SpeechBubblePos.y, 0);

        mDimmed.gameObject.SetActive(false);
        if (stepData.IsDimmed)
        {
            mDimmed.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(stepData.MaskedObjectName))
        {
            if(stepData.IsSizeCal)
            {
                if(stepData.MaskedObjectName.Equals(RESERV_PUZZLEBOARD))
                {
                    //보드 전체에 Mask를 씌워야함
                    RectTransform imageTransform = GameObject.Find(stepData.MaskedObjectName).GetComponent<RectTransform>();
                    float sizeX = (imageTransform.anchorMax.x - imageTransform.anchorMin.x) * 720f;
                    float sizey = (imageTransform.anchorMax.y - imageTransform.anchorMin.y) * (Screen.height * (720f / Screen.width));

                    mMask.rectTransform.sizeDelta = new Vector2(sizeX, sizey);
                    mMask.transform.position = imageTransform.position;
                }
                else if(stepData.MaskedObjectName.Equals(RESERV_MOVECOUNT))
                {
                    RectTransform imageTransform = GameObject.Find(stepData.MaskedObjectName).GetComponent<RectTransform>();

                    float sizeX = (imageTransform.anchorMax.x - imageTransform.anchorMin.x) * 720f;
                    float sizey = (imageTransform.anchorMax.y - imageTransform.anchorMin.y) * (Screen.height * (720f / Screen.width));

                    mMask.rectTransform.sizeDelta = new Vector2(sizeX, sizey);
                    mMask.transform.position = imageTransform.position;
                }
            }
            else
            {
                string[] splitStr;
                mMask.rectTransform.sizeDelta = stepData.MaskSize;

                splitStr = stepData.MaskedObjectName.Split('_');
                int index = 0;
                int.TryParse(splitStr[1], out index);

                mMask.transform.position = MissionManager.Instance.GetMissionCellUIByIndex(index).transform.position;
            }
            mMask.gameObject.SetActive(true);
        }
        gameObject.SetActive(true);

        if (stepData.IsPoint)
        {
            Vector3 pos = Vector3.zero;

            mPointHand.FlipXImage(false);
            if (stepData.SelectCoordi.x != 0)
            {
                mPointHand.transform.eulerAngles = Vector3.forward * -90f;
                pos.x = (mMask.rectTransform.sizeDelta.x / -2);
                if (stepData.SelectCoordi.x < 0)
                {
                    pos.x *= -1;
                    mPointHand.transform.eulerAngles = Vector3.forward * 90f;
                    mPointHand.FlipXImage(true);
                }
            }
            else
            {
                mPointHand.transform.eulerAngles = Vector3.forward * 180f;
                pos.y = (mMask.rectTransform.sizeDelta.y / 2);
                if (stepData.SelectCoordi.y < 0)
                {
                    pos.y *= -1;
                    mPointHand.transform.eulerAngles = Vector3.forward * 0f;
                }
            }

            mPointHand.transform.localPosition = mMask.rectTransform.localPosition + pos;
            mPointHand.SetPointAnimation(mPointHand.transform.localPosition, pos);
            mPointHand.StartPointAnimation();
        }
    }
}
