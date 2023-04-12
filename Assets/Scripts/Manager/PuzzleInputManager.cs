using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleInputManager : MonoBehaviour
{
    public static NormalTile SelectTileOrNull
    {
        get => mSelectTile;
        set
        {
            mSelectTile = value;

            if (mSelectTile == null)
            {
                mSelectBlockEffect.SetActive(false);
                return;
            }
            if (mSelectBlockEffect == null) { return; }
            mSelectBlockEffect.transform.position = mSelectTile.gameObject.transform.position;
            mSelectBlockEffect.SetActive(true);
        }
    }
    public static NormalTile TargetTileOrNull
    {
        get => mTargetTile;
        set
        {
            mTargetTile = value;
            if (mSelectBlockEffect == null) { return; }
            mSelectBlockEffect.SetActive(false);
        }
    }

    [SerializeField] private bool mbOnDrag = false;

    private const float SWAP_SEC = 0.25f;
    private Vector3 mRayPos;
    private RaycastHit2D mHit;
    private Camera mMainCamera;
    private WaitForSeconds mWaitSwap = new WaitForSeconds(SWAP_SEC);

    private Vector2 mDragPos;
    private float mDragAngle;
    private float mPiQuater = Mathf.PI * 0.25f;
    private int mDragSwapDir;

    [SerializeField] private static NormalTile mSelectTile;
    [SerializeField] private static NormalTile mTargetTile;
    [SerializeField] private NormalTile mInputTile;

    [SerializeField] private GameObject mSelectEffectPrefab;
    private static GameObject mSelectBlockEffect;

    private delegate void PlayerInput();
    PlayerInput TouchInput;
    PlayerInput DragInput;

    private System.Action mCurrentInputFunc = null;
    private System.Action mPreviouseInputFunc = null;

    private PlayerSkill mCurrentSelectSkill;


    private void Start()
    {
        mMainCamera = Camera.main;

        ObserverCenter observerCenter = ObserverCenter.Instance;
        observerCenter.AddObserver(ExcuteInputStateOnByNoti, EGameState.Input.ToString());
        observerCenter.AddObserver(ExcuteMatchSwapByNoti, EGameState.MatchSwap.ToString());
        observerCenter.AddObserver(ExcuteReturnSwapByNoti, EGameState.ReturnSwap.ToString());
        observerCenter.AddObserver(ExcuteChangeMoveCountByNoti, Message.RefreshMoveCount);
        observerCenter.AddObserver(ExcutePauseByNoti, Message.PauseGame);
        observerCenter.AddObserver(ExcuteResumeByNoti, Message.ResumeGame);

        observerCenter.AddObserver(ExcuteNormalModeByNoti, Message.OnNormalMode);
        observerCenter.AddObserver(ExcuteTutorialModeByNoti, Message.OnTutoMode);

        // 해머가 아니라 스킬, 플레이어스킬을 받아서 설정해야한다.
        observerCenter.AddObserver(ExcuteSkillStateOnByNoti, EGameState.PlayerSkill.ToString());

        TouchInput = CheckTouchInput;
        DragInput = CheckDrag;

        if (mSelectBlockEffect == null)
        {
            mSelectBlockEffect = GameObjectPool.Instantiate(mSelectEffectPrefab);
            mSelectBlockEffect.SetActive(false);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C) == true)
        {
            ObserverCenter.Instance.SendNotification(Message.ClearMissionCheat);
            return;
        }
#endif
        mCurrentInputFunc?.Invoke();
    }

    private void InputModeFunc()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TouchInput();
        }
        if (Input.GetMouseButtonUp(0))
        {
            mbOnDrag = false;
        }
        if (mbOnDrag)
        {
            DragInput();
        }
    }
    private void OneTileSkillModeFunc()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OneTileSkill_Down();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OneTileSkill_Up();
        }
    }
    private void TwoTileSkillModeFunc()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TwoTileSkill_Down();
        }
        if (Input.GetMouseButtonUp(0))
        {
            TwoTileSkill_Up();
        }
    }

    private void CheckTouchInput()
    {
        Vector2 differenceVector;

        GetMousePositionNormalTile(out mInputTile);
        if (mInputTile == null) { return; }
        if (mInputTile.BlockContainerOrNull == null) { return; }
        if (mInputTile.BlockContainerOrNull.IsFixed) { return; }
#if UNITY_EDITOR
        #region Test용 블록 체인지
        if (Input.GetKey(KeyCode.BackQuote) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 0, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha1) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 1, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha2) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 2, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha3) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 3, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha4) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 4, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha9) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(RockBlock), -1, 3);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha8) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(BoxBlock), -1, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha7) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(BarrelBlock), -1, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha6) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(VineBlock), -1, 1);
            mInputTile = null;
            return;
        }
        #endregion
#endif
        if (SelectTileOrNull == null)
        {
            mbOnDrag = true;
            SelectTileOrNull = mInputTile;
        }
        else
        {
            if (SelectTileOrNull == mInputTile) { SelectTileOrNull = null; }
            else
            {
                differenceVector = SelectTileOrNull.Coordi - mInputTile.Coordi;
                if (differenceVector.sqrMagnitude <= 1)
                {
                    TargetTileOrNull = mInputTile;
                    PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchSwap);
                }
                else
                {
                    mbOnDrag = true;
                    SelectTileOrNull = mInputTile;
                }
            }
        }
    }
    private void CheckDrag()
    {
        mDragPos = mMainCamera.ScreenToWorldPoint(Input.mousePosition) - SelectTileOrNull.transform.position;

        if (mDragPos.sqrMagnitude >= 0.333f)
        {
            mDragAngle = mDragPos.normalized.y * Vector2.down.y;
            if (mDragAngle >= -1f && mDragAngle < -mPiQuater)
            {
                mDragSwapDir = 0;
            }
            else if (mDragAngle > mPiQuater && mDragAngle <= 1)
            {
                mDragSwapDir = 2;
            }
            else
            {
                if (mDragPos.x >= 0)
                {
                    mDragSwapDir = 1;
                }
                else
                {
                    mDragSwapDir = 3;
                }
            }

            TargetTileOrNull = SelectTileOrNull.GetAroundNormalTileByDir(mDragSwapDir) as NormalTile;
            if (TargetTileOrNull == null) { return; }
            if (TargetTileOrNull.BlockContainerOrNull == null) { return; }
            if (TargetTileOrNull.BlockContainerOrNull.IsFixed) { return; }

            mbOnDrag = false;
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchSwap);
        }
    }

    private void CheckTouchInput_Tuto()
    {
        Vector2 differenceVector;

        GetMousePositionNormalTile(out mInputTile);
        if (mInputTile == null) { return; }
        if (mInputTile.BlockContainerOrNull == null) { return; }
        if (mInputTile.BlockContainerOrNull.IsFixed) { return; }
#if UNITY_EDITOR
        #region Test용 블록 체인지
        if (Input.GetKey(KeyCode.BackQuote) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 0, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha1) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 1, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha2) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 2, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha3) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 3, 1);
            mInputTile = null;
            return;
        }
        if (Input.GetKey(KeyCode.Alpha4) == true)
        {
            mInputTile.RemoveBlockContainer();
            BlockManager.Instance.CreateBlockByBlockDataInTile(mInputTile, typeof(NormalBlock), 4, 1);
            mInputTile = null;
            return;
        }
        #endregion
#endif
        if (SelectTileOrNull == null)
        {
            mbOnDrag = true;
            SelectTileOrNull = mInputTile;
            if (TutorialManager.Instance.IsSwapMode)
            {
                if (SelectTileOrNull.Coordi != TutorialManager.Instance.SelectCoordi)
                {
                    mbOnDrag = false;
                    SelectTileOrNull = null;
                    return;
                }
            }
        }
        else
        {
            if (SelectTileOrNull == mInputTile)
            {
                SelectTileOrNull = null;
            }
            else
            {
                differenceVector = SelectTileOrNull.Coordi - mInputTile.Coordi;
                if (differenceVector.sqrMagnitude <= 1)
                {
                    TargetTileOrNull = mInputTile;
                    if (TutorialManager.Instance.IsSwapMode)
                    {
                        if (TargetTileOrNull.Coordi != TutorialManager.Instance.TargetCoordi)
                        {
                            mbOnDrag = false;
                            SelectTileOrNull = null;
                            TargetTileOrNull = null;
                            Debug.Log("Target 지정된 대상이 아닙니다.");
                            return;
                        }
                    }
                    PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchSwap);
                }
                else
                {
                    mbOnDrag = true;
                    SelectTileOrNull = mInputTile;
                    if (TutorialManager.Instance.IsSwapMode)
                    {
                        if (SelectTileOrNull.Coordi != TutorialManager.Instance.SelectCoordi)
                        {
                            mbOnDrag = false;
                            SelectTileOrNull = null;
                            Debug.Log("Select 지정된 대상이 아닙니다.");
                        }
                    }
                }
            }
        }
    }
    private void CheckDrag_Tuto()
    {
        mDragPos = mMainCamera.ScreenToWorldPoint(Input.mousePosition) - SelectTileOrNull.transform.position;

        if (mDragPos.sqrMagnitude >= 0.7f)
        {
            mDragAngle = mDragPos.normalized.y * Vector2.down.y;
            if (mDragAngle >= -1f && mDragAngle < -mPiQuater)
            {
                mDragSwapDir = 0;
            }
            else if (mDragAngle > mPiQuater && mDragAngle <= 1)
            {
                mDragSwapDir = 2;
            }
            else
            {
                if (mDragPos.x >= 0)
                {
                    mDragSwapDir = 1;
                }
                else
                {
                    mDragSwapDir = 3;
                }
            }

            TargetTileOrNull = SelectTileOrNull.GetAroundNormalTileByDir(mDragSwapDir) as NormalTile;
            if (TargetTileOrNull == null) { return; }
            if (TargetTileOrNull.BlockContainerOrNull == null) { return; }
            if (TargetTileOrNull.BlockContainerOrNull.IsFixed) { return; }

            if (TutorialManager.Instance.IsSwapMode)
            {
                if (TargetTileOrNull.Coordi != TutorialManager.Instance.TargetCoordi)
                {
                    mbOnDrag = false;
                    SelectTileOrNull = null;
                    TargetTileOrNull = null;
                    Debug.Log("Target 지정된 대상이 아닙니다.");
                    return;
                }
            }

            mbOnDrag = false;
            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchSwap);
        }
    }

    private void SwapSelectTargetTileBlockContainer()
    {
        mCurrentInputFunc = null;

        BlockContainer blockContainer = TargetTileOrNull.BlockContainerOrNull;
        TargetTileOrNull.BlockContainerOrNull = SelectTileOrNull.BlockContainerOrNull;
        SelectTileOrNull.BlockContainerOrNull = blockContainer;

        TargetTileOrNull.BlockContainerOrNull.StartMovePositionByTile(TargetTileOrNull, SWAP_SEC);
        SelectTileOrNull.BlockContainerOrNull.StartMovePositionByTile(SelectTileOrNull, SWAP_SEC);
    }
    private IEnumerator SwapBlockContainerCoroutine(bool IsMatch)
    {
        SwapSelectTargetTileBlockContainer();
        yield return mWaitSwap;
        yield return null;
        if (IsMatch) { PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.MatchCheck); }
        else { PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Input); }
    }
    private void GetMousePositionNormalTile(out NormalTile tile)
    {
        tile = null;
        mRayPos = mMainCamera.ScreenToWorldPoint(Input.mousePosition);
        mHit = Physics2D.Raycast(mRayPos, Vector3.forward * 100f);
        if (mHit.collider == null) { return; }
        tile = mHit.collider.GetComponent<NormalTile>();
    }


    #region One Tile Mode
    private void OneTileSkill_Down()
    {
        GetMousePositionNormalTile(out mInputTile);
        if (mInputTile == null) { return; }
        if (mInputTile.BlockContainerOrNull == null) { return; }
        SelectTileOrNull = mInputTile;
    }
    private void OneTileSkill_Up()
    {
        if (SelectTileOrNull == null) { return; }

        GetMousePositionNormalTile(out mInputTile);
        if (mInputTile == null) { return; }
        if (mInputTile.BlockContainerOrNull == null) { return; }
        TargetTileOrNull = mInputTile;

        if (SelectTileOrNull.Coordi != TargetTileOrNull.Coordi)
        {
            SelectTileOrNull = null;
            TargetTileOrNull = null;
            return;
        }
        mCurrentInputFunc = null;

        // 등록된 스킬을 실행시킨다.
        mCurrentSelectSkill?.DoSkill(SelectTileOrNull);
    }
    #endregion

    #region Two Tile Mode
    private void TwoTileSkill_Down()
    {
        GetMousePositionNormalTile(out mInputTile);
        if (mInputTile == null) { return; }
        if (mInputTile.BlockContainerOrNull == null) { mInputTile = null; return; }
    }
    private void TwoTileSkill_Up()
    {
        if (mInputTile == null) { return; }
        var downTileCoordi = mInputTile.Coordi;

        // 입력 타일 확인도 스킬별로 다르기 때문에 따로 하는게 좋을듯 하다.

        // 업 했을때 타일을 가져오고 확인
        GetMousePositionNormalTile(out mInputTile);
        if (mInputTile == null) { return; }
        if (mInputTile.BlockContainerOrNull == null) { mInputTile = null; return; }

        // 업 했을때 다른 좌표라면 취소한다.
        if (downTileCoordi != mInputTile.Coordi) { mInputTile = null; return; }

        // 제대로 타일이 입력된 경우
        // > selectTile이 없는 경우 바로 설정
        if (SelectTileOrNull == null)
        {
            SelectTileOrNull = mInputTile;
            mInputTile = null;
            return;
        }

        // 처음 선택한 타일과 같은 경우 취소
        if (SelectTileOrNull.Coordi == mInputTile.Coordi)
        {
            SelectTileOrNull = null;
            mInputTile = null;
            return;
        }

        // 두번째 타일로 설정 후 스킬 발동
        TargetTileOrNull = mInputTile;
        mInputTile = null;

        mCurrentInputFunc = null;
        mCurrentSelectSkill?.DoSkill(SelectTileOrNull, TargetTileOrNull);

        SelectTileOrNull = null;
        TargetTileOrNull = null;
    }
    #endregion


    private void ExcuteInputStateOnByNoti(Notification noti)
    {
        SelectTileOrNull = null;
        TargetTileOrNull = null;
        mInputTile = null;
        if (TileMapManager.Instance.MoveCount <= 0)
        {
            mCurrentInputFunc = null;
        }
        else
        {
            mCurrentInputFunc = InputModeFunc;
        }
    }
    private void ExcuteSkillStateOnByNoti(Notification noti)
    {
        mCurrentSelectSkill = (noti.Data as SkillNotiArgs).CurrentPlayerSkill;
        switch (mCurrentSelectSkill.SkillType)
        {
            case ESkillType.OneTile:
                mCurrentInputFunc = OneTileSkillModeFunc;
                break;
            case ESkillType.TwoTile:
                mCurrentInputFunc = TwoTileSkillModeFunc;
                break;
        }

    }
    private void ExcuteMatchSwapByNoti(Notification noti)
    {
        StartCoroutine(SwapBlockContainerCoroutine(true));
    }
    private void ExcuteReturnSwapByNoti(Notification noti)
    {
        if (SelectTileOrNull == null || TargetTileOrNull == null)
        {
            Debug.Log("리턴 스왑 : Input 상태로 변경");
            //Input 변경 전에 MatchPossible 상태 체크해야함
            //>> 매치가 불가능 한 경우 > 섞는다.
            //>> 매치가 가능 한 경우 > Input
            TileMapManager.Instance.IsMatched = false;
            TileMapManager.Instance.MatchPossibleCheck();

            PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Input);
            return;
        }
        StartCoroutine(SwapBlockContainerCoroutine(false));
    }
    private void ExcuteChangeMoveCountByNoti(Notification noti)
    {
        int movecount = TileMapManager.Instance.MoveCount;
        if (movecount <= 0) { return; }
        if (PuzzleManager.Instance.CurrentState != EGameState.Input) { return; }
        mCurrentInputFunc = InputModeFunc;
    }

    private void ExcuteTutorialModeByNoti(Notification noti)
    {
        TouchInput = null;
        DragInput = null;
        TouchInput = CheckTouchInput_Tuto;
        DragInput = CheckDrag_Tuto;
    }
    private void ExcuteNormalModeByNoti(Notification noti)
    {
        TouchInput = null;
        DragInput = null;
        TouchInput = CheckTouchInput;
        DragInput = CheckDrag;
    }

    private void ExcutePauseByNoti(Notification noti)
    {
        mPreviouseInputFunc = mCurrentInputFunc;
        mCurrentInputFunc = null;
    }
    private void ExcuteResumeByNoti(Notification noti)
    {
        mCurrentInputFunc = mPreviouseInputFunc;
    }
}