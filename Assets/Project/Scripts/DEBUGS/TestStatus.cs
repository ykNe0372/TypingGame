using UnityEngine;

public class TestStatus : MonoBehaviour {
    [SerializeField] private Character _character;

    private void Start() {
        Debug.Log("PhysicalAttack: " + _character.GetFinalStatus(StatusType.PhysicalAttack));
        Debug.Log("MagicAttack: " + _character.GetFinalStatus(StatusType.MagicAttack));
        Debug.Log("Defence: " + _character.GetFinalStatus(StatusType.Defense));
    }
}