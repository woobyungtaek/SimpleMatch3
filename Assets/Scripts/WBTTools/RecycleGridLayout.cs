using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RecycleCellUI : MonoBehaviour
{
    public virtual void Init(int index) { }
}

public enum EGridDirection
{
    Vertical,
    Horizontal
}

public enum EChildStart
{
    LeftTop,
    LeftBottom,
    RightTop,
    RightBottom
}

public enum EChildPivot
{
    LeftTop,
    CenterTop,
    RightTop,
    LeftMiddle,
    CenterMiddle,
    RightMiddle,
    LeftBottom,
    CenterBottom,
    RightBottom
}

[RequireComponent(typeof(RectTransform))]
public class RecycleGridLayout : MonoBehaviour
{
    // RectTransforms
    private RectTransform mRectTrs;
    [SerializeField] private ScrollRect     mScrollRect;
    [SerializeField] private RectTransform  mViewportRect;
    [SerializeField] private RectTransform  mChildRectTrs;

    // Prefabs
    [Header("Prefab")]
    [SerializeField] GameObject mCellPrefab;

    // Set Info
    [Header("Data Info")]
    [SerializeField] private int mTotalDataCount;
    public int TotalDataCount { set => mTotalDataCount = value; }

    [Header("Dir")]
    [SerializeField] private EGridDirection mGridDir;
    [SerializeField] private EChildStart mEChildStart;
    [SerializeField] private EChildPivot mEChildPivot;

    [Header("Count")]
    [SerializeField] private int mLineCellCount; // 1라인에 들어갈 Cell 갯수
    [SerializeField] private int mExtraLineCount; // 여유분 Line
    private int mLineCount;

    [Header("Setting")]
    [SerializeField] private Vector2 CellSize;
    [SerializeField] private Vector2 Spacing;
    [SerializeField] private Vector2 ViewOffset;

    [Header("Etc")]
    [SerializeField] private bool mbSticky;
    [SerializeField] private bool mbHalfMove;

    // CellUI
    private int mViewLineCount;
    private int mViewCellCount;
    private LinkedList<RecycleCellUI> mCellList = new LinkedList<RecycleCellUI>();

    // 계산되어야 하는 값
    private Vector2 mCellSize;
    private float mCellPosValue;
    private float mLinePosValue;

    private System.Action mRePositionContentFunc;
    private System.Func<float, float, Vector2> mCellUIPos_Func;

    // 드래그 이동시 사용되는 값
    private System.Action mMoveFunc_Input;
    private int mCurrentIdx;
    [SerializeField]private int mCurrentLineIdx;

    // 인덱스 기반 이동
    private System.Action mMoveFunc_Auto;
    private float mTime;
    private Vector2[] MoveVec = new Vector2[2];

    // 모노
    private void Update()
    {
        // 자동으로 움직이는 함수 끝나면 Null된다.
        mMoveFunc_Auto?.Invoke();

        // 유저가 직접 움직이는 부분
        mMoveFunc_Input?.Invoke();

    }

    // 초기화
    public void Init()
    {
        mRectTrs = transform as RectTransform;

        Setting_ContentPivot();      // 컨텐츠 오브젝트의 Pivot설정
        Setting_ChildRectPivot();    // CellUI의 Pivot설정
        Setting_PosValue();          // 시작점, 방향에 따른 위치 조절에 필요한 값 설정

        // 공통으로 쓰일 값들 계산        
        mLineCount = mTotalDataCount / mLineCellCount; // 전체 라인수 계산
        if(mTotalDataCount % mLineCellCount != 0)
        {
            mLineCount += 1;
        }
        mCellSize = CellSize + Spacing;

        Setting_ContentSize();// Content 사이즈 계산

        // 셀 갯수 관련
        float rectLength;
        float cellLength;
        if (mGridDir == EGridDirection.Vertical)
        {
            rectLength = mViewportRect.rect.height;
            cellLength = mCellSize.y;
        }
        else
        {
            rectLength = mViewportRect.rect.width;
            cellLength = mCellSize.x;
        }

        // 양쪽다 고려하는게아니라 움직일 방향에 맞춰서만 계산되면 됨
        mViewLineCount = (int)(rectLength / cellLength);
        int totalViewLineCount = (mViewLineCount + mExtraLineCount);
        mViewCellCount = totalViewLineCount * mLineCellCount; // > 한번에 표시될 Cell의 개수

        mCurrentIdx = 0;

        // 셀 생성
        CreateCellUI_Pool();

        // 시작시 보여지기 원하는 인덱스를 넣으면된다.
        ForceRefreshByDataIndex(0); // > 얘도 고장났네... 고쳐야한다..

        //Debug.Log(mTotalCellCount);
    }

    public void Clear()
    {
        var iter = mCellList.First;
        while (iter != null)
        {
            GameObjectPool.ReturnObject(iter.Value.gameObject);
            iter = iter.Next;
        }
        mCellList.Clear();
    }

    // 값 설정
    private void Setting_ContentPivot()
    {
        switch (mEChildStart)
        {
            case EChildStart.LeftTop:
                mRectTrs.anchorMin = Vector2.up;
                mRectTrs.anchorMax = Vector2.up;
                mRectTrs.pivot = Vector2.up;
                break;
            case EChildStart.LeftBottom:
                mRectTrs.anchorMin = Vector2.zero;
                mRectTrs.anchorMax = Vector2.zero;
                mRectTrs.pivot = Vector2.zero;
                break;
            case EChildStart.RightTop:
                mRectTrs.anchorMin = Vector2.one;
                mRectTrs.anchorMax = Vector2.one;
                mRectTrs.pivot = Vector2.one;
                break;
            case EChildStart.RightBottom:
                mRectTrs.anchorMin = Vector2.right;
                mRectTrs.anchorMax = Vector2.right;
                mRectTrs.pivot = Vector2.right;
                break;
        }
        mRectTrs.anchoredPosition = Vector2.zero;
    }
    private void Setting_ChildRectPivot()
    {
        if (mChildRectTrs == null) { mChildRectTrs = mCellPrefab.transform as RectTransform; }
        switch (mEChildPivot)
        {
            case EChildPivot.LeftTop:
                mChildRectTrs.anchorMin = Vector2.up;
                mChildRectTrs.anchorMax = Vector2.up;
                mChildRectTrs.pivot = Vector2.up;
                break;
            case EChildPivot.CenterTop:
                mChildRectTrs.anchorMin = new Vector2(0.5f, 1f);
                mChildRectTrs.anchorMax = new Vector2(0.5f, 1f);
                mChildRectTrs.pivot = new Vector2(0.5f, 1f);
                break;
            case EChildPivot.RightTop:
                mChildRectTrs.anchorMin = Vector2.one;
                mChildRectTrs.anchorMax = Vector2.one;
                mChildRectTrs.pivot = Vector2.one;
                break;

            case EChildPivot.LeftMiddle:
                mChildRectTrs.anchorMin = new Vector2(0f, 0.5f);
                mChildRectTrs.anchorMax = new Vector2(0f, 0.5f);
                mChildRectTrs.pivot = new Vector2(0f, 0.5f);
                break;
            case EChildPivot.CenterMiddle:
                mChildRectTrs.anchorMin = new Vector2(0.5f, 0.5f);
                mChildRectTrs.anchorMax = new Vector2(0.5f, 0.5f);
                mChildRectTrs.pivot = new Vector2(0.5f, 0.5f);
                break;
            case EChildPivot.RightMiddle:
                mChildRectTrs.anchorMin = new Vector2(1f, 0.5f);
                mChildRectTrs.anchorMax = new Vector2(1f, 0.5f);
                mChildRectTrs.pivot = new Vector2(1f, 0.5f);
                break;

            case EChildPivot.LeftBottom:
                mChildRectTrs.anchorMin = Vector2.zero;
                mChildRectTrs.anchorMax = Vector2.zero;
                mChildRectTrs.pivot = Vector2.zero;
                break;

            case EChildPivot.CenterBottom:
                mChildRectTrs.anchorMin = new Vector2(0.5f, 0f);
                mChildRectTrs.anchorMax = new Vector2(0.5f, 0f);
                mChildRectTrs.pivot = new Vector2(0.5f, 0f);
                break;
            case EChildPivot.RightBottom:
                mChildRectTrs.anchorMin = Vector2.right;
                mChildRectTrs.anchorMax = Vector2.right;
                mChildRectTrs.pivot = Vector2.right;
                break;
        }
    }
    private void Setting_PosValue()
    {
        Vector2 pivot = mRectTrs.pivot;
        pivot *= -2f;
        pivot += Vector2.one;

        if (mGridDir == EGridDirection.Vertical)
        {
            mCellPosValue = pivot.x;
            mLinePosValue = pivot.y;
            mScrollRect.vertical = true;
            mScrollRect.horizontal = false;
            mRePositionContentFunc = RePositionContent_Vertical;
            mCellUIPos_Func = RePositionCellUI_Vertical;
            mMoveFunc_Input = MoveContentByInput_Vertical;
        }
        else
        {
            mCellPosValue = pivot.y;
            mLinePosValue = pivot.x;
            mScrollRect.vertical = false;
            mScrollRect.horizontal = true;
            mRePositionContentFunc = RePositionContent_Horizontal;
            mCellUIPos_Func = RePositionCellUI_Horizontal;
            mMoveFunc_Input = MoveContentByInput_Horizontal;
        }
    }
    private void Setting_ContentSize()
    {
        Vector2 contentSize;
        float width;
        float height;
        if (mGridDir == EGridDirection.Vertical)
        {
            width = mCellSize.x * mLineCellCount;
            height = mCellSize.y * mLineCount;
        }
        else
        {
            width = mCellSize.x * mLineCount;
            height = mCellSize.y * mLineCellCount;
        }

        contentSize = new Vector2(width, height);

        // 전체 길이 적용
        mRectTrs.sizeDelta = contentSize;
    }

    // 셀 생성
    private void CreateCellUI_Pool()
    {
        Clear();

        RecycleCellUI inst;
        for (int idx = -mExtraLineCount; idx < mViewCellCount - mExtraLineCount; ++idx)
        {
            // 생성
            inst = GameObjectPool.Instantiate<RecycleCellUI>(mCellPrefab, transform);

            // 첫 위치 조정
            var rectTrs = inst.transform as RectTransform;
            rectTrs.sizeDelta = CellSize;
            rectTrs.anchorMin = mChildRectTrs.anchorMin;
            rectTrs.anchorMax  = mChildRectTrs.anchorMax;
            rectTrs.pivot = mChildRectTrs.pivot;

            // 관리 셀에 추가
            mCellList.AddLast(inst);
            inst.gameObject.SetActive(idx >= 0 && idx < mTotalDataCount);
        }
    }

    // 인덱스 계산
    private void CalLineCellIdxByDataIndex(int dataIdx, out int line, out int cell)
    {
        if (dataIdx < 0)
        {
            int celIdx = dataIdx - mLineCellCount + 1;
            line = celIdx / mLineCellCount;
            cell = celIdx % mLineCellCount + mLineCellCount - 1;
        }
        else
        {
            line = dataIdx / mLineCellCount;
            cell = dataIdx % mLineCellCount;
        }
    }
    // 위치 조절
    private void RePositionContent_Vertical()
    {
        int linePosIdx = mCurrentIdx / mLineCellCount;
        Vector2 rectPos;
        rectPos = new Vector2(0, linePosIdx * mCellSize.y);
        mRectTrs.anchoredPosition = rectPos;
    }
    private void RePositionContent_Horizontal()
    {
        int linePosIdx = mCurrentIdx / mLineCellCount;
        Vector2 rectPos;
        rectPos = new Vector2(linePosIdx * mCellSize.x, 0);
        mRectTrs.anchoredPosition = rectPos;
    }
    private Vector2 RePositionCellUI_Vertical(float posIdx, float lineIdx)
    {
        Vector2 pos;
        pos.x = posIdx * mCellPosValue * mCellSize.x;
        pos.y = lineIdx * mLinePosValue * mCellSize.y;
        //Debug.Log($"Vertical : ({posIdx},{lineIdx}){pos}");
        return pos;
    }
    private Vector2 RePositionCellUI_Horizontal(float posIdx, float lineIdx)
    {
        Vector2 pos;
        pos.x = lineIdx * mLinePosValue * mCellSize.x;
        pos.y = posIdx * mCellPosValue * mCellSize.y;
        //Debug.Log($"Horizontal : ({posIdx},{lineIdx}){pos}");
        return pos;
    }

    // 데이터 인덱스로 이동
    // 스크롤뷰 갱신 - 즉시 표시
    public void ForceRefreshByDataIndex(int dataIndex)
    {
        mCurrentLineIdx = dataIndex / mLineCellCount;
        mCurrentIdx = dataIndex;

        // Content 위치 설정
        mRePositionContentFunc?.Invoke();

        // CellUI들 위치 설정
        int useDataIndex = dataIndex;// - (mLineCellCount * mExtraLineCount);
        foreach (var cell in mCellList)
        {
            CalLineCellIdxByDataIndex(useDataIndex, out int lineIdx, out int cellIdx);

            // Cell은 데이터 인덱스로 계산되어야 한다.
            cell.Init(useDataIndex);

            // Cell의 활성화는 Line까지 봐야한다. Line이 보이는 상태인지 확인해야함
            cell.gameObject.SetActive(!(useDataIndex < 0 || useDataIndex >= mTotalDataCount));
            (cell.transform as RectTransform).anchoredPosition = (Vector2)mCellUIPos_Func?.Invoke(cellIdx, lineIdx);
            useDataIndex++;
        }
    }
    // 스크롤뷰 갱신 - 이동
    public void MoveToPositionByDataIndex(int dataIndex)
    {
        // 인덱스 입력하면 위치 설정해서 지정된 시간동안 이동한다.
        mTime = 0;

        // 현재위치 / 가야할 곳 위치
        MoveVec[0] = mRectTrs.anchoredPosition;
        MoveVec[1] = new Vector2(0, -(dataIndex - 1) * mCellSize.y);

        float dist = Vector2.Distance(MoveVec[0], MoveVec[1]);
        float viewLen = mViewportRect.rect.height - ViewOffset.y;

        if (dist > viewLen)
        {
            // 보여져야하는 만큼 현재 위치에서 이동시켜주기
            MoveVec[1] = MoveVec[0] - new Vector2(0, ViewOffset.y);
        }
        else
        {
            if (dist > CellSize.y / 2) { return; }

            // 거리가 일정 이상이면 안움직여도 됨?
            int maxIndex = mTotalDataCount - mViewLineCount + mExtraLineCount;

            if (dataIndex < 1) { dataIndex = 1; }
            else if (dataIndex > maxIndex) { dataIndex = maxIndex; }

            float max = MoveVec[0].y;
            MoveVec[1] = new Vector2(0, -(dataIndex - 1) * mCellSize.y);
            if (MoveVec[1].y < max) { return; }
        }

        mMoveFunc_Auto = MoveFunc;
    }

    // 유저 입력
    // 세로 이동
    private void MoveContentByInput_Vertical()
    {
        // 방향 별
        float changePosY = mRectTrs.anchoredPosition.y * -1;
        int changeLineIdx = (int)(changePosY / mCellSize.y * mLinePosValue);

        // 나머지는 공통 함수로 처리 가능
        MoveContentByInput_Common(changeLineIdx);
    }

    // 가로 이동
    private void MoveContentByInput_Horizontal()
    {
        // 방향 별
        float changePosX = mRectTrs.anchoredPosition.x * -1;
        int changeLineIdx = (int)(changePosX / mCellSize.x * mLinePosValue);

        // 나머지는 공통 함수로 처리 가능
        MoveContentByInput_Common(changeLineIdx);
    }

    private void MoveContentByInput_Common(int changeLineIdx)
    {
        // 인덱스 변동 체크
        if (mCurrentLineIdx == changeLineIdx) { return; }

        // 이동 방향은 현재 라인 번호에서 바뀐 라인 번호를 빼면 된다.
        int dir = changeLineIdx - mCurrentLineIdx;

        int dataIdx = 0;

        // 바뀐 라인의 끝부분 인덱스부터 갱신이 시작
        if (dir < 0)
        {
            dataIdx = mCurrentLineIdx * mLineCellCount - 1;
        }
        else
        {
            dataIdx = mCurrentLineIdx * mLineCellCount + mViewCellCount;
        }

        mCurrentLineIdx += dir;

        int loopCount = mExtraLineCount * mLineCellCount;
        // 한번 변경시 라인에 들어가는 수만큼 씩 갱신된다.
        for (int cnt = 0; cnt < loopCount; ++cnt)
        {
            CalLineCellIdxByDataIndex(dataIdx, out int line, out int cell);
            RecycleCellUI cellUI;
            // 인덱스 증가
            if (dir > 0)
            {
                // 재사용 할 Cell UI 선택
                cellUI = mCellList.First.Value;

                // 관리 리스트의 순서 변경
                mCellList.RemoveFirst();
                mCellList.AddLast(cellUI);

                // 위치 변경
                (cellUI.transform as RectTransform).anchoredPosition
                    = (Vector2)mCellUIPos_Func?.Invoke(cell, line);
            }
            // 인덱스 감소
            else
            {
                // 재사용 할 Cell UI 선택
                cellUI = mCellList.Last.Value;

                // 관리 리스트의 순서 변경
                mCellList.RemoveLast();
                mCellList.AddFirst(cellUI);

                // 위치 변경
                (cellUI.transform as RectTransform).anchoredPosition
                    = (Vector2)mCellUIPos_Func?.Invoke(cell, line);
            }

            // cell 갱신
            cellUI.Init(dataIdx);
            cellUI.gameObject.SetActive(!(dataIdx < 0 || dataIdx >= mTotalDataCount));
            dataIdx += dir;
        }
        mCurrentLineIdx = changeLineIdx;
    }



    private void MoveFunc()
    {
        float value = mTime / 0.5f;
        mRectTrs.anchoredPosition = Vector2.LerpUnclamped(MoveVec[0], MoveVec[1], value);

        if (value >= 1) { mMoveFunc_Auto = null; }

        mTime += Time.unscaledDeltaTime;
    }
}
