using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Relic")]
public class RelicData : ScriptableObject {
    public string RelicName;
    // public Sprite icon;
    public List<RelicEffect> effects;
}