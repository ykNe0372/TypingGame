using System.Collections;
using UnityEngine;

public class SkillSelectionUI : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private RectTransform[] _slots;
    [SerializeField] private RectTransform _gear;

    private Coroutine _moveRoutine;

    private void OnEnable() {
        _player.OnSkillChanged += MoveGear;      
    }

    private void OnDisable() {
        _player.OnSkillChanged -= MoveGear;
    }

    private void Start() {
        MoveGear(_player.CurrentSkillIndex, false);
    }

    private void MoveGear(int index) {
        MoveGear(index, true);
    }

    private void MoveGear(int index, bool animate) {
        if (index < 0 || index >= _slots.Length) {
            Debug.LogError($"Skill slot index {index} is out of range.");
            return;
        }

        if (_moveRoutine != null) StopCoroutine(_moveRoutine);

        Vector2 target = _slots[index].anchoredPosition;
        if (!animate) {
            _gear.anchoredPosition = target;
            return;
        }

        _moveRoutine = StartCoroutine(AnimateGear(target));
    }

    private IEnumerator AnimateGear(Vector2 target) {
        Vector2 start = _gear.anchoredPosition;
        const float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            float time = Mathf.Clamp01(elapsed / duration);
            _gear.anchoredPosition = Vector2.Lerp(start, target, time);
            yield return null;
        }

        _gear.anchoredPosition = target;
        _moveRoutine = null;
    }
}