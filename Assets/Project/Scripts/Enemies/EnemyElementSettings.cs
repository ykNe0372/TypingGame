using System;
using System.Collections.Generic;

[Serializable]
public class EnemyElementSettings {
    public EnemyElementMode Mode;
    public float AttackChance;
    public List<ElementWeight> Elements = new();
}