using UnityEngine;

public class PlayerStatusUI : MonoBehaviour {
    [SerializeField] private Character _player;
    [Header("HP")]
    [SerializeField] private RectTransform _hpMask;
    [SerializeField] private RectTransform _hpLiquidHead;
    [SerializeField] private GameObject _hpRightCap;
    [SerializeField] private float _hpBarMaxWidth;

    private void Awake() {
        _player.OnHPChanged += UpdateHP;
    }

    private void UpdateHP(float currentHP, float maxHP) {
        float rate = Mathf.Clamp01(currentHP / maxHP);
        
        Vector2 maskSize = _hpMask.sizeDelta;
        maskSize.x = _hpBarMaxWidth * rate;
        _hpMask.sizeDelta = maskSize;

        _hpLiquidHead.anchoredPosition = new Vector2(maskSize.x + _hpLiquidHead.rect.width / 2, _hpLiquidHead.anchoredPosition.y);
        _hpRightCap.SetActive(rate >= 1.0f);
    }
}