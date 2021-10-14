using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EMatchType
{
    noType,
    HLine,
    VLine,
    Square,
    Cross,
    FiveLine
}
[System.Serializable]
public class MatchInfo : Pool<MatchInfo>
{
    public int ID { get => mMatchId; set => mMatchId = value; }
    public int MatchNumber { get => mMatchNumber; set => mMatchNumber = value; }
    public EMatchType MatchType { get => mMatchType; }
    public List<Tile> MatchTileList { get => mMatchTileList; }
    public System.Type MergeBlockTypeOrNull
    {
        get
        {
            int matchCount = mMatchTileList.Count;
            System.Type resultType = null;
            if (MergePosTileOrNull == null) { return null; }
            switch (MatchType)
            {
                case EMatchType.HLine:
                    resultType = typeof(HorizontalBombBlock);
                    if(mbInputTile)
                    {
                        if(InputDir.x != 0) { break; }
                        resultType = typeof(VerticalBombBlock);
                    }
                    break;
                case EMatchType.VLine:
                    resultType = typeof(VerticalBombBlock);
                    if (mbInputTile)
                    {
                        if (InputDir.y != 0) { break; }
                        resultType = typeof(HorizontalBombBlock);
                    }
                    break;
                case EMatchType.Square:
                    resultType = typeof(HomingBombBlock);
                    break;
                case EMatchType.Cross:
                    resultType = typeof(AroundBombBlock);
                    break;
                case EMatchType.FiveLine:
                    resultType = typeof(ColorBombBlock);
                    break;
                default:
                    resultType = null;
                    break;
            }
            return resultType;
        }
    }
    public Tile MergePosTileOrNull { get => mMergePosTile; }
    

    [SerializeField] private int mMatchId = -1;
    [SerializeField] private int mMatchNumber = -1;
    [SerializeField] private EMatchType mMatchType = EMatchType.noType;
    [SerializeField] private List<Tile> mMatchTileList = new List<Tile>();
    [SerializeField] private List<Tile> mOverLapTileList = new List<Tile>();
    [SerializeField] private Tile mMergePosTile;
    [SerializeField] private bool mbInputTile;
    [SerializeField] private Vector2 InputDir;

    private static string mNoTypeStr = EMatchType.noType.ToString();
    private string mMatchTypeStr = mNoTypeStr;

    private static Dictionary<string, Dictionary<string, EMatchType>> matchTypeChangeConditionDict = new Dictionary<string, Dictionary<string, EMatchType>>();
    private static Dictionary<string, EMatchType> stringMatchTypeDict = new Dictionary<string, EMatchType>();

    public static void CreateMatchTypeChangeConditionDict()
    {
        foreach (EMatchType matchType in System.Enum.GetValues(typeof(EMatchType)))
        {
            if (stringMatchTypeDict.ContainsKey(matchType.ToString())) { continue; }
            stringMatchTypeDict.Add(matchType.ToString(), matchType);
        }
        AddMatchTypeConditionInternal(EMatchType.noType, EMatchType.HLine, EMatchType.HLine);
        AddMatchTypeConditionInternal(EMatchType.noType, EMatchType.VLine, EMatchType.VLine);
        AddMatchTypeConditionInternal(EMatchType.noType, EMatchType.Square, EMatchType.Square);
        AddMatchTypeConditionInternal(EMatchType.noType, EMatchType.Cross, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.noType, EMatchType.FiveLine, EMatchType.FiveLine);

        AddMatchTypeConditionInternal(EMatchType.HLine, EMatchType.VLine, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.HLine, EMatchType.Square, EMatchType.Square);
        AddMatchTypeConditionInternal(EMatchType.HLine, EMatchType.Cross, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.HLine, EMatchType.FiveLine, EMatchType.FiveLine);

        AddMatchTypeConditionInternal(EMatchType.VLine, EMatchType.HLine, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.VLine, EMatchType.Square, EMatchType.Square);
        AddMatchTypeConditionInternal(EMatchType.VLine, EMatchType.Cross, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.VLine, EMatchType.FiveLine, EMatchType.FiveLine);

        AddMatchTypeConditionInternal(EMatchType.Cross, EMatchType.HLine, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.Cross, EMatchType.VLine, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.Cross, EMatchType.Square, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.Cross, EMatchType.FiveLine, EMatchType.FiveLine);

        AddMatchTypeConditionInternal(EMatchType.Square, EMatchType.HLine, EMatchType.Square);
        AddMatchTypeConditionInternal(EMatchType.Square, EMatchType.VLine, EMatchType.Square);
        AddMatchTypeConditionInternal(EMatchType.Square, EMatchType.Cross, EMatchType.Cross);
        AddMatchTypeConditionInternal(EMatchType.Square, EMatchType.FiveLine, EMatchType.FiveLine);
    }

    public override void Dispose()
    {
        MatchNumber = -1;
        mMatchType = EMatchType.noType;
        mMatchTypeStr = mNoTypeStr;
        mMergePosTile = null;
        InputDir = Vector2.zero;
        mbInputTile = false;

        mMatchTileList.Clear();
        mOverLapTileList.Clear();
    }
    public void SetMatchInfoByData(int blockNumber, EMatchType matchType, List<Tile> matchList)
    {
        MatchNumber = blockNumber;
        AddMatchInfoByTypeAndList(matchType, matchList);
    }
    public void AddMatchInfoByTypeAndList(EMatchType matchType, List<Tile> addTileList)
    {
        SetMatchType(matchType);

        int loopcount = addTileList.Count;
        for (int index = 0; index < loopcount; index++)
        {
            if (!mMatchTileList.Contains(addTileList[index])) { continue; }
            if (mOverLapTileList.Contains(addTileList[index])) { continue; }
            mOverLapTileList.Add(addTileList[index]);
        }
        for (int index = 0; index < loopcount; index++)
        {
            if (mMatchTileList.Contains(addTileList[index])) { continue; }
            mMatchTileList.Add(addTileList[index]);
        }

        if(mMatchTileList.Count <= 3) { return; }
        if (MatchTileList.Contains(PuzzleInputManager.TargetTileOrNull))
        {
            mbInputTile = true;
            mMergePosTile = PuzzleInputManager.TargetTileOrNull;
            InputDir = mMergePosTile.Coordi - PuzzleInputManager.SelectTileOrNull.Coordi;
            return;
        }
        if (MatchTileList.Contains(PuzzleInputManager.SelectTileOrNull))
        {
            mbInputTile = true;
            mMergePosTile = PuzzleInputManager.SelectTileOrNull;
            InputDir = PuzzleInputManager.TargetTileOrNull.Coordi -  mMergePosTile.Coordi;
            return;
        }
        if (mOverLapTileList.Count != 0)
        {
            mMergePosTile = mOverLapTileList[mOverLapTileList.Count - 1];
            return;
        }
        mMergePosTile = mMatchTileList[Random.Range(0, mMatchTileList.Count)];
    }
    public bool IsOverlaps(List<Tile> checkTileList)
    {
        int loopCount = checkTileList.Count;
        for (int index = 0; index < loopCount; index++)
        {
            if (mMatchTileList.Contains(checkTileList[index]))
            {
                return true;
            }
        }
        return false;
    }

    private void SetMatchType(EMatchType requestType)
    {
        string requestStr = GetGameStateString(requestType);
        if (!matchTypeChangeConditionDict.ContainsKey(mMatchTypeStr)) { return; }
        if (!matchTypeChangeConditionDict[mMatchTypeStr].ContainsKey(requestStr)) { return; }
        mMatchType = matchTypeChangeConditionDict[mMatchTypeStr][requestStr];
        mMatchTypeStr = GetGameStateString(mMatchType);
    }
    private string GetGameStateString(EMatchType gameState)
    {
        foreach (KeyValuePair<string, EMatchType> checkMatchType in stringMatchTypeDict)
        {
            if (checkMatchType.Value != gameState) { continue; }
            return checkMatchType.Key;
        }
        return null;
    }
    private static void AddMatchTypeConditionInternal(EMatchType currentType, EMatchType nextType, EMatchType changeType)
    {
        string currentStr = currentType.ToString();
        string nextStr = nextType.ToString();

        if (!(matchTypeChangeConditionDict.ContainsKey(currentStr)))
        {
            matchTypeChangeConditionDict.Add(currentStr, new Dictionary<string, EMatchType>());
        }
        if (matchTypeChangeConditionDict[currentStr].ContainsKey(nextStr)) { return; }
        matchTypeChangeConditionDict[currentStr].Add(nextStr, changeType);
    }

}
