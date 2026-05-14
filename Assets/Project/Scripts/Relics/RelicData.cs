using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Relic")]
public class RelicData : ScriptableObject {
    public string relicName;
    // public Sprite icon;
    public List<RelicEffect> effects;
}