using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBombBlock : BombBlock
{
    public Type ChangeType { get => mChangeType; set => mChangeType = value; }

    private Type mChangeType = null;

    public override void ExplosionBombBlock()
    {
        BaseExplosionBomobBlock();
    }
}
