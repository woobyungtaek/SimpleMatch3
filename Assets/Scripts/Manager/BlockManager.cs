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

    private Dictionary<Type, GameObject> blockPrefabDictByType = new Dictionary<Type, GameObject>();
    private Dictionary<string, GameObject> blockPrefabDictByName = new Dictionary<string, GameObject>();

    private BlockContainer instBlockContainer;
    [SerializeField] private List<BlockContainer> mAllBlockContainer = new List<BlockContainer>();

    private void Awake()
    {
        CreateBlockPrefabDictsByFieldInfoInternal();
    }
    
    public void CreateBlockByBlockDataInTile(Tile tile, Type blockType, int blockNumber, int blockHP, Transform parentOrNull = null)
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

    public Block GetBlockByBlockName(string blockName)
    {
        return GameObjectPool.Instantiate<Block>(blockPrefabDictByName[blockName]);
    }
    public Block GetBlockByBlockType(Type blockType)
    {
        return GameObjectPool.Instantiate<Block>(blockPrefabDictByType[blockType]);
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

        blockPrefabDictByType.Clear();

        blockPrefabDictByName.Clear();

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
            if (blockPrefabDictByType.ContainsKey(type) == false) { blockPrefabDictByType.Add(type, null); }
            if (blockPrefabDictByName.ContainsKey(type.ToString()) == false) { blockPrefabDictByName.Add(type.ToString(), null); }

            prefabObject = (fieldArr[index].GetValue(this) as Block).gameObject;

            blockPrefabDictByType[type] = prefabObject;
            blockPrefabDictByName[type.ToString()] = prefabObject;
        }
    }
}