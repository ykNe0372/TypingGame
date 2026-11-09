using System.Collections;
using TMPro;
using UnityEngine;

public class SpecialChargeNotification : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private TextMeshProUGUI _notification;
    [SerializeField] private float _displayDuration = 1.2f;
    [SerializeField] private float _fadeDuration = 0.25f;
    [SerializeField] private float _popDuration = 0.2f;

    private Coroutine _showRoutine;

    private void Awake() {
        _notification.enabled = false;
    }

    private void OnEnable() {
        _player.OnSpecialLevelCharged += ShowCharge;
    }

    private void OnDisable() {
        _player.OnSpecialLevelCharged -= ShowCharge;

        if (_showRoutine != null) {
            StopCoroutine(_showRoutine);
            _showRoutine = null;
        }

        _notification.enabled = false;
    }

    private void ShowCharge(int level) {
        if (_showRoutine != null) StopCoroutine(_showRoutine);
        _showRoutine = StartCoroutine(ShowChargeRoutine(level));
    }

    private IEnumerator ShowChargeRoutine(int level) {
        _notification.text = $"必殺技 Lv.{level}\nチャージ完了！";
        _notification.enabled = true;

        Color baseColor = _notification.color;
        RectTransform rect = _notification.rectTransform;
        Vector3 baseScale = rect.localScale;
        float duration = Mathf.Max(0.01f, _displayDuration);
        float fadeDuration = Mathf.Clamp(_fadeDuration, 0f, duration);
        float popDuration = Mathf.Max(0.01f, _popDuration);
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;

            float popProgress = Mathf.Clamp01(elapsed / popDuration);
            float easedPop = 1f - Mathf.Pow(1f - popProgress, 3f);
            rect.localScale = baseScale * Mathf.Lerp(0.75f, 1f, easedPop);

            Color color = baseColor;
            if (fadeDuration > 0f) {
                color.a *= 1f - Mathf.Clamp01((elapsed - duration + fadeDuration) / fadeDuration);
            }
            _notification.color = color;

            yield return null;
        }

        rect.localScale = baseScale;
        _notification.color = baseColor;
        _notification.enabled = false;
        _showRoutine = null;
    }
}
