using UnityEngine;
using UnityEngine.UI;

public class DebugTypingSlider : MonoBehaviour {
    [SerializeField] private TypingManager _typingMg;
    [SerializeField] private Slider _minSlider;
    [SerializeField] private Slider _maxSlider;
    [SerializeField] private TMPro.TextMeshProUGUI _minText;
    [SerializeField] private TMPro.TextMeshProUGUI _maxText;

    public void Debug_OnSliderChanged() {
        int min = (int)_minSlider.value;
        int max = (int)_maxSlider.value;
        _typingMg.SetLengthRange(min, max);
        Debug.Log($"[DEBUG] min: {min}, max: {max}");
        UpdateLabel();
    }

    void UpdateLabel() {
        _minText.text = $"Min: {(int)_minSlider.value}";
        _maxText.text = $"Max: {(int)_maxSlider.value}";
    }
}