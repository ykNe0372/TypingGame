using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Growth Item")]
public class GrowthItem : ScriptableObject {

    [SerializeField] private List<Effect> _effects = new();

    public List<Effect> GetEffects() {
        return _effects;
    }
}