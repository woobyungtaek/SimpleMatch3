using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionReserData
{
    public static void Enqueue(this IReserveData reserveData, Tile tile)
    {
        reserveData.RouteTileQueue.Enqueue(tile);
    }

    public static void ClearQueue(this IReserveData reserveData)
    {
        reserveData.RouteTileQueue.Clear();
    }

    public static void CopyQueue(this IReserveData reserveData, Queue<Tile> routeQueue)
    {
        while (routeQueue.Count != 0)
        {
            reserveData.Enqueue(routeQueue.Dequeue());
        }
    }
    public static void CopyQueue(this IReserveData reserveData, IReserveData oriData)
    {
        while (oriData.RouteTileQueue.Count != 0)
        {
            Tile dest = oriData.RouteTileQueue.Dequeue();
            if (reserveData is BlockContainer)
            {
                (reserveData as BlockContainer).DestTile = dest;
            }
            reserveData.Enqueue(dest);
        }
    }
}

public interface IReserveData
{
    bool IsFixed { get; }

    Queue<Tile> RouteTileQueue { get; }
}

public class ReserveData : IReserveData, IReUseObject
{
    private Queue<Tile> mRouteTileQueue = new Queue<Tile>();

    public Queue<Tile> RouteTileQueue { get => mRouteTileQueue; }

    public bool IsFixed { get => false; }

    public void ResetObject() { }
}
