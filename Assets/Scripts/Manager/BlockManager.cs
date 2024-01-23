using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class BlockManager : SceneSingleton<BlockManager>
{
    [SerializeField] private Transform GameTransform;

    [Header("Basic")]
    [SerializeField] private GameObject blockContainerPrefab;
    [SerializeField] private NormalBlock normalBlockPrefab;

    [Header("Gimmick")]
    [SerializeField] private RockBlock rockBlockPrefab;
    [SerializeField] private BoxBlock boxBlockPrefab;
    [SerializeField] private BarrelBlock barrelBlockPrefab;
    [SerializeField] private VineBlock vineBlockPrefab;

    [Header("Explosion")]
    [SerializeField] private VerticalBombBlock verticalBombBlockPrefab;
    [SerializeField] private HorizontalBombBlock horizontalBombBlockPrefab;
    [SerializeField] private HomingBombBlock homingBombBlockPrefab;
    [SerializeField] private CrossBombBlock crossBombBlockPrefab;
    [SerializeField] private AroundBombBlock aroundBombBlockPrefab;
    [SerializeField] private ColorBombBlock colorBombBlockPrefab;

    [Header("MergeExplosion")]
    [SerializeField] private BigVerticalBombBlock bigVerticalBombBlockPrefab;
    [SerializeField] private BigHorizontalBombBlock bigHorizontalBombBlockPrefab;
    [SerializeField] private BigAroundBombBlock bigAroundBombBlockPrefab;
    [SerializeField] private BigCrossBombBlock bigCrossBombBlockPrefab;
    [SerializeField] private ColorChangeBombBlock colorChangeBombBlockPrefab;
    [SerializeField] private HomingChangeBombBlock homingChangeBombBlockPrefab;
    [SerializeField] private TripleHomingBombBlock tripleChangeBombBlockPrefab;
    [SerializeField] private AllClearBombBlock allClearBombBlockPrefab;


    private Dictionary<Type, GameObject> mBlockPrefabDict = new Dictionary<Type, GameObject>();

    private BlockContainer instBlockContainer;
    [SerializeField] private List<BlockContainer> mAllBlockContainer = new List<BlockContainer>();


    [Header("Block Animation Curve")]
    [SerializeField] public AnimationCurve BlockArriveAniCurve;
    [SerializeField] public AnimationCurve BlockMoveAniCurve;
    [SerializeField] public AnimationCurve BlockPushAniCurve;
    [SerializeField] public AnimationCurve BlockMatchEffectAniCurve;

    private void Awake()
    {
        CreateBlockPrefabDictsByFieldInfoInternal();
    }
    

    public void CreateBlockInBlockContainerReserve(BlockContainer blockContainer , Type blockType, int blockNumber, int blockHP, Transform parentOrNull = null)
    {
        Block createBlock;

        createBlock = GetBlockByBlockType(blockType);
        createBlock.SetBlockData(blockNumber, blockHP);

        blockContainer.AddBlockToReserveList(createBlock);
        blockContainer.ClearQueue();
    }
    public void CreateBlockByBlockDataInTile(Tile tile, Type blockType, int blockNumber, int blockHP, Transform parentOrNull = null)
    {
        Block createBlock;
        if (tile.BlockContainerOrNull == null)
        {
            tile.BlockContainerOrNull = GetEmptyBlockContainer(parentOrNull);
        }
        tile.BlockContainerOrNull.RemoveAllBlock();
        tile.BlockContainerOrNull.transform.position = tile.transform.position;

        createBlock = GetBlockByBlockType(blockType);
        createBlock.SetBlockData(blockNumber, blockHP);

        tile.BlockContainerOrNull.AddBlockToBlockList(createBlock);
        tile.BlockContainerOrNull.ClearQueue();
    }
    public Block GetCreateBlockByBlockDataInTile(Tile tile, Type blockType, int blockNumber, int blockHP, Transform parentOrNull = null)
    {
        Block createBlock;
        if (tile.BlockContainerOrNull == null)
        {
            tile.BlockContainerOrNull = GetEmptyBlockContainer(parentOrNull);
        }
        tile.BlockContainerOrNull.transform.position = tile.transform.position;

        createBlock = GetBlockByBlockType(blockType);
        createBlock.SetBlockData(blockNumber, blockHP);

        tile.BlockContainerOrNull.AddBlockToBlockList(createBlock);
        tile.BlockContainerOrNull.ClearQueue();
        return createBlock;
    }

    public BlockContainer GetEmptyBlockContainer(Transform parentOrNull = null)
    {
        instBlockContainer = GameObjectPool.Instantiate<BlockContainer>(blockContainerPrefab, GameTransform);
        instBlockContainer.transform.localScale = Vector3.one;

        if(!mAllBlockContainer.Contains(instBlockContainer))
        {
            mAllBlockContainer.Add(instBlockContainer);
        }
        return instBlockContainer;
    }

    public Block GetBlockByBlockType(Type blockType)
    {
        return GameObjectPool.Instantiate<Block>(mBlockPrefabDict[blockType]);
    }

    public void ClearAllBlockContainers()
    {
        int loopCount = mAllBlockContainer.Count;
        for(int index =0; index < loopCount; index++)
        {
            mAllBlockContainer[index].RemoveAllBlock();
            GameObjectPool.ReturnObject(mAllBlockContainer[index].gameObject);
        }
    }

    private void CreateBlockPrefabDictsByFieldInfoInternal()
    {
        Type type;
        GameObject prefabObject;
        bool bBlock = false;

        mBlockPrefabDict.Clear();

        FieldInfo[] fieldArr = typeof(BlockManager).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

        int loopCount = fieldArr.Length;
        for (int index = 0; index < loopCount; index++)
        {
            if (fieldArr[index].GetValue(this) == null) { continue; }

            bBlock = false;
            type = fieldArr[index].GetValue(this).GetType();
            while(type != null)
            {
                if (type.BaseType == typeof(Block)) { bBlock = true; break; }
                type = type.BaseType;
            }
            if (!bBlock) { continue; }

            type = fieldArr[index].GetValue(this).GetType();
            if (mBlockPrefabDict.ContainsKey(type) == false) { mBlockPrefabDict.Add(type, null); }
            
            prefabObject = (fieldArr[index].GetValue(this) as Block).gameObject;

            mBlockPrefabDict[type] = prefabObject;
        }
    }
}