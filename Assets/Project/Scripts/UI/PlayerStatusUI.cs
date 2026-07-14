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
        
        Vector2 size = _hpMask.sizeDelta;
        size.x = _hpBarMaxWidth * rate;
        _hpMask.sizeDelta = size;

        _hpLiquidHead.anchoredPosition = new Vector2(size.x, _hpLiquidHead.anchoredPosition.y); // まだ改良の余地あり（HP残量が多い時に HPLiquidHead の位置が少しズレる）
        _hpRightCap.SetActive(rate >= 1.0f);
    }
}