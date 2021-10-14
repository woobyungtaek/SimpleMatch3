using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileArea
{
    public static readonly List<Vector2> square = new List<Vector2>()
    {
        new Vector2(1,0),new Vector2(1,1),new Vector2(0,1)
    };
    
    public static readonly List<Vector2> vertical = new List<Vector2>()
    {
        new Vector2(0,-13),
        new Vector2(0,-12),
        new Vector2(0,-11),
        new Vector2(0,-10),
        new Vector2(0,-9),
        new Vector2(0,-8),
        new Vector2(0,-7),
        new Vector2(0,-6),
        new Vector2(0,-5),
        new Vector2(0,-4),
        new Vector2(0,-3),
        new Vector2(0,-2),
        new Vector2(0,-1),
        new Vector2(0,1),
        new Vector2(0,2),
        new Vector2(0,3),
        new Vector2(0,4),
        new Vector2(0,5),
        new Vector2(0,6),
        new Vector2(0,7),
        new Vector2(0,8),
        new Vector2(0,9),
        new Vector2(0,10),
        new Vector2(0,11),
        new Vector2(0,12),
        new Vector2(0,13)
    };
    public static readonly List<Vector2> bigvertical = new List<Vector2>()
    {
        new Vector2(-1,-12), new Vector2(0,-13), new Vector2(1,-12),
        new Vector2(-1,-11), new Vector2(0,-12), new Vector2(1,-11),
        new Vector2(-1,-10), new Vector2(0,-11), new Vector2(1,-10),
        new Vector2(-1,-9), new Vector2(0,-10), new Vector2(1,-9),
        new Vector2(-1,-8),  new Vector2(0,-9),  new Vector2(1,-8),
        new Vector2(-1,-7),  new Vector2(0,-8),  new Vector2(1,-7),
        new Vector2(-1,-6),  new Vector2(0,-7),  new Vector2(1,-6),
        new Vector2(-1,-5),  new Vector2(0,-6),  new Vector2(1,-5),
        new Vector2(-1,-4),  new Vector2(0,-5),  new Vector2(1,-4),
        new Vector2(-1,-3),  new Vector2(0,-4),  new Vector2(1,-3),
        new Vector2(-1,-2),  new Vector2(0,-3),  new Vector2(1,-2),
        new Vector2(-1,-1),  new Vector2(0,-2),  new Vector2(1,-1),
        new Vector2(-1,-0),  new Vector2(0,-1),  new Vector2(1,-0),

        new Vector2(-1,0),   new Vector2(0,1),   new Vector2(1,0),
        new Vector2(-1,1),   new Vector2(0,2),   new Vector2(1,1),
        new Vector2(-1,2),   new Vector2(0,3),   new Vector2(1,2),
        new Vector2(-1,3),   new Vector2(0,4),   new Vector2(1,3),
        new Vector2(-1,4),   new Vector2(0,5),   new Vector2(1,4),
        new Vector2(-1,5),   new Vector2(0,6),   new Vector2(1,5),
        new Vector2(-1,6),   new Vector2(0,7),   new Vector2(1,6),
        new Vector2(-1,7),   new Vector2(0,8),   new Vector2(1,7),
        new Vector2(-1,8),   new Vector2(0,9),   new Vector2(1,8),
        new Vector2(-1,9),  new Vector2(0,10),  new Vector2(1,9),
        new Vector2(-1,10),  new Vector2(0,11),  new Vector2(1,10),
        new Vector2(-1,11),  new Vector2(0,12),  new Vector2(1,11),
        new Vector2(-1,12),  new Vector2(0,13),  new Vector2(1,12)
    };

    public static readonly List<Vector2> around = new List<Vector2>()
    {
        new Vector2(0,-1),        new Vector2(1,-1),        new Vector2(1,0),
        new Vector2(1,1),                                   new Vector2(0,1),
        new Vector2(-1,1),        new Vector2(-1,0),        new Vector2(-1,-1)
    };
    public static readonly List<Vector2> bigaround = new List<Vector2>()
    {
                            new Vector2(-1,-2), new Vector2(0,-2), new Vector2(1,-2),
        new Vector2(-2,-1), new Vector2(-1,-1), new Vector2(0,-1), new Vector2(1,-1),new Vector2(2,-1),
        new Vector2(-2,0),  new Vector2(-1,0),                     new Vector2(1,0), new Vector2(2,0),
        new Vector2(-2,1),  new Vector2(-1,1),  new Vector2(0,1),  new Vector2(1,1), new Vector2(2,1),
                            new Vector2(-1,2),  new Vector2(0,2),  new Vector2(1,2)
    };

    public static readonly List<Vector2> smallcross = new List<Vector2>()
    {
        new Vector2(0,-1),new Vector2(1,0),new Vector2(0,1),new Vector2(-1,0)
    };
    public static readonly List<Vector2> cross = new List<Vector2>()
    {
        new Vector2(0,-13),
        new Vector2(0,-12),
        new Vector2(0,-11),
        new Vector2(0,-10),
        new Vector2(0,-9),
        new Vector2(0,-8),
        new Vector2(0,-7),
        new Vector2(0,-6),
        new Vector2(0,-5),
        new Vector2(0,-4),
        new Vector2(0,-3),
        new Vector2(0,-2),
        new Vector2(0,-1),
        new Vector2(-13,0),new Vector2(-12,0),new Vector2(-11,0),new Vector2(-10,0),new Vector2(-9,0),new Vector2(-8,0),new Vector2(-7,0),new Vector2(-6,0),new Vector2(-5,0),new Vector2(-4,0),new Vector2(-3,0),new Vector2(-2,0),new Vector2(-1,0),
        new Vector2(1,0),new Vector2(2,0),new Vector2(3,0),new Vector2(4,0),new Vector2(5,0),new Vector2(6,0),new Vector2(7,0),new Vector2(8,0),new Vector2(9,0),new Vector2(10,0),new Vector2(11,0),new Vector2(12,0),new Vector2(13,0),
        new Vector2(0,1),
        new Vector2(0,2),
        new Vector2(0,3),
        new Vector2(0,4),
        new Vector2(0,5),
        new Vector2(0,6),
        new Vector2(0,7),
        new Vector2(0,8),
        new Vector2(0,9),
        new Vector2(0,10),
        new Vector2(0,11),
        new Vector2(0,12),
        new Vector2(0,13)
    };
    public static readonly List<Vector2> bigcross = new List<Vector2>()
    {
                                                                                                                                                                                                                                                new Vector2(-1,-13),new Vector2(0,-13),new Vector2(1,-13),
                                                                                                                                                                                                                                                new Vector2(-1,-12),new Vector2(0,-12),new Vector2(1,-12),
                                                                                                                                                                                                                                                new Vector2(-1,-11),new Vector2(0,-11),new Vector2(1,-11),
                                                                                                                                                                                                                                                new Vector2(-1,-10),new Vector2(0,-10),new Vector2(1,-10),
                                                                                                                                                                                                                                                new Vector2(-1,-9),new Vector2(0,-9),new Vector2(1,-9),
                                                                                                                                                                                                                                                new Vector2(-1,-8),new Vector2(0,-8),new Vector2(1,-8),
                                                                                                                                                                                                                                                new Vector2(-1,-7),new Vector2(0,-7),new Vector2(1,-7),
                                                                                                                                                                                                                                                new Vector2(-1,-6),new Vector2(0,-6),new Vector2(1,-6),
                                                                                                                                                                                                                                                new Vector2(-1,-5),new Vector2(0,-5),new Vector2(1,-5),
                                                                                                                                                                                                                                                new Vector2(-1,-4),new Vector2(0,-4),new Vector2(1,-4),
                                                                                                                                                                                                                                                new Vector2(-1,-3),new Vector2(0,-3),new Vector2(1,-3),
                                                                                                                                                                                                                                                new Vector2(-1,-2),new Vector2(0,-2),new Vector2(1,-2),
        new Vector2(-13,-1),new Vector2(-12,-1),new Vector2(-11,-1),new Vector2(-10,-1),new Vector2(-9,-1),new Vector2(-8,-1),new Vector2(-7,-1),new Vector2(-6,-1),new Vector2(-5,-1),new Vector2(-4,-1),new Vector2(-3,-1),new Vector2(-2,-1),new Vector2(-1,-1),new Vector2(0,-1),new Vector2(1,-1),new Vector2(2,-1),new Vector2(3,-1),new Vector2(4,-1),new Vector2(5,-1),new Vector2(6,-1),new Vector2(7,-1),new Vector2(8,-1),new Vector2(9,-1),new Vector2(10,-1),new Vector2(11,-1),new Vector2(12,-1),new Vector2(13,-1),
        new Vector2(-13,0), new Vector2(-12,0), new Vector2(-11,0), new Vector2(-10,0), new Vector2(-9,0), new Vector2(-8,0), new Vector2(-7,0), new Vector2(-6,0), new Vector2(-5,0), new Vector2(-4,0), new Vector2(-3,0), new Vector2(-2,0), new Vector2(-1,0),                   new Vector2(1,0), new Vector2(2,0), new Vector2(3,0), new Vector2(4,0), new Vector2(5,0), new Vector2(6,0), new Vector2(7,0), new Vector2(8,0), new Vector2(9,0), new Vector2(10,0), new Vector2(11,0), new Vector2(12,0), new Vector2(13,0),
        new Vector2(-13,1), new Vector2(-12,1), new Vector2(-11,1), new Vector2(-10,1), new Vector2(-9,1), new Vector2(-8,1), new Vector2(-7,1), new Vector2(-6,1), new Vector2(-5,1), new Vector2(-4,1), new Vector2(-3,1), new Vector2(-2,1), new Vector2(-1,1), new Vector2(0,1), new Vector2(1,1), new Vector2(2,1), new Vector2(3,1), new Vector2(4,1), new Vector2(5,1), new Vector2(6,1), new Vector2(7,1), new Vector2(8,1), new Vector2(9,1), new Vector2(10,1), new Vector2(11,1), new Vector2(12,1), new Vector2(13,1),
                                                                                                                                                                                                                                                new Vector2(-1,2), new Vector2(0,2), new Vector2(1,2),
                                                                                                                                                                                                                                                new Vector2(-1,3), new Vector2(0,3), new Vector2(1,3),
                                                                                                                                                                                                                                                new Vector2(-1,4), new Vector2(0,4), new Vector2(1,4),
                                                                                                                                                                                                                                                new Vector2(-1,5), new Vector2(0,5), new Vector2(1,5),
                                                                                                                                                                                                                                                new Vector2(-1,6), new Vector2(0,6), new Vector2(1,6),
                                                                                                                                                                                                                                                new Vector2(-1,7), new Vector2(0,7), new Vector2(1,7),
                                                                                                                                                                                                                                                new Vector2(-1,8), new Vector2(0,8), new Vector2(1,8),
                                                                                                                                                                                                                                                new Vector2(-1,9), new Vector2(0,9), new Vector2(1,9),
                                                                                                                                                                                                                                                new Vector2(-1,10),new Vector2(0,10),new Vector2(1,10),
                                                                                                                                                                                                                                                new Vector2(-1,11),new Vector2(0,11),new Vector2(1,11),
                                                                                                                                                                                                                                                new Vector2(-1,12),new Vector2(0,12),new Vector2(1,12),
                                                                                                                                                                                                                                                new Vector2(-1,13),new Vector2(0,13),new Vector2(1,13)
    };

    public static readonly List<Vector2> horizontal = new List<Vector2>()
    {
        new Vector2(-13,0),new Vector2(-12,0),new Vector2(-11,0),new Vector2(-10,0),new Vector2(-9,0),new Vector2(-8,0),new Vector2(-7,0),new Vector2(-6,0),new Vector2(-5,0),new Vector2(-4,0),new Vector2(-3,0),new Vector2(-2,0),new Vector2(-1,0),        new Vector2(1,0),new Vector2(2,0),new Vector2(3,0),new Vector2(4,0),new Vector2(5,0),new Vector2(6,0),new Vector2(7,0),new Vector2(8,0),new Vector2(9,0),new Vector2(10,0),new Vector2(11,0),new Vector2(12,0),new Vector2(13,0)
    };
    public static readonly List<Vector2> bighorizontal = new List<Vector2>()
    {
        new Vector2(-13,-1),new Vector2(-12,-1),new Vector2(-11,-1),new Vector2(-10,-1),new Vector2(-9,-1),new Vector2(-8,-1),new Vector2(-7,-1),new Vector2(-6,-1),new Vector2(-5,-1),new Vector2(-4,-1),new Vector2(-3,-1),new Vector2(-2,-1),new Vector2(-1,-1),new Vector2(0,-1),new Vector2(1,-1),new Vector2(2,-1),new Vector2(3,-1),new Vector2(4,-1),new Vector2(5,-1),new Vector2(6,-1),new Vector2(7,-1),new Vector2(8,-1),new Vector2(9,-1),new Vector2(10,-1),new Vector2(11,-1),new Vector2(12,-1),new Vector2(13,-1),
        new Vector2(-13,0), new Vector2(-12,0), new Vector2(-11,0), new Vector2(-10,0), new Vector2(-9,0), new Vector2(-8,0), new Vector2(-7,0), new Vector2(-6,0), new Vector2(-5,0), new Vector2(-4,0), new Vector2(-3,0), new Vector2(-2,0), new Vector2(-1,0),                   new Vector2(1,0), new Vector2(2,0), new Vector2(3,0), new Vector2(4,0), new Vector2(5,0), new Vector2(6,0), new Vector2(7,0), new Vector2(8,0), new Vector2(9,0), new Vector2(10,0), new Vector2(11,0), new Vector2(12,0), new Vector2(13,0),
        new Vector2(-13,1), new Vector2(-12,1), new Vector2(-11,1), new Vector2(-10,1), new Vector2(-9,1), new Vector2(-8,1), new Vector2(-7,1), new Vector2(-6,1), new Vector2(-5,1), new Vector2(-4,1), new Vector2(-3,1), new Vector2(-2,1), new Vector2(-1,1), new Vector2(0,1), new Vector2(1,1), new Vector2(2,1), new Vector2(3,1), new Vector2(4,1), new Vector2(5,1), new Vector2(6,1), new Vector2(7,1), new Vector2(8,1), new Vector2(9,1), new Vector2(10,1), new Vector2(11,1), new Vector2(12,1), new Vector2(13,1)
    };
}
