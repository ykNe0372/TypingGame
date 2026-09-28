using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, Header("表示時間")] private float _displayDuration = 1f;

    public void Initialize(float damage) {
        _text.text = damage.ToString();
        Destroy(gameObject, _displayDuration);
    }
}