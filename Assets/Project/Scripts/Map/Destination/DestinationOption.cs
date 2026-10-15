using UnityEngine;
using TMPro;

public class DestinationOption : MonoBehaviour {
    [SerializeField] private TMP_Text _keyText;
    [SerializeField] private TMP_Text _destinationText;

    // 追加：数字キーと行き先名を設定する
    public void Initialize(int key, string destinationName) {
        _keyText.text = key.ToString();
        _destinationText.text = destinationName;
    }
}