using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, Header("表示時間")] private float _displayDuration = 1f;

    public void Initialize(float damage) {
        Initialize(damage.ToString());
    }

    public void Initialize(string text) {
        _text.text = text;
        Destroy(gameObject, _displayDuration);
    }
}