using UnityEngine;

public class PlayerStatusUI : MonoBehaviour {
    [SerializeField] private Character _player;
    [Header("HP")]
    [SerializeField] private RectTransform _hpMask;
    [SerializeField] private RectTransform _hpLiquidHead;
    [SerializeField] private GameObject _hpRightCap;
    [SerializeField] private float _hpBarMaxWidth;
    [Header("MP")]
    [SerializeField] private RectTransform _mpMask;
    [SerializeField] private RectTransform _mpLiquidHead;
    [SerializeField] private GameObject _mpRightCap;
    [SerializeField] private float _mpBarMaxWidth;


    private void Awake() {
        _player.OnHPChanged += UpdateHP;
        _player.OnMPChanged += UpdateMP;
    }

    private void UpdateHP(float currentHP, float maxHP) {
        float rate = Mathf.Clamp01(currentHP / maxHP);
        
        Vector2 maskSize = _hpMask.sizeDelta;
        maskSize.x = _hpBarMaxWidth * rate;
        _hpMask.sizeDelta = maskSize;

        _hpLiquidHead.anchoredPosition = new Vector2(maskSize.x + _hpLiquidHead.rect.width / 2, _hpLiquidHead.anchoredPosition.y);
        _hpRightCap.SetActive(rate >= 1.0f);
    }

    private void UpdateMP(float currentMP, float maxMP) {
        float rate = Mathf.Clamp01(currentMP / maxMP);

        Vector2 maskSize = _mpMask.sizeDelta;
        maskSize.x = _mpBarMaxWidth * rate;
        _mpMask.sizeDelta = maskSize;

        _mpLiquidHead.anchoredPosition = new Vector2(maskSize.x + _mpLiquidHead.rect.width / 2, _mpLiquidHead.anchoredPosition.y);
        _mpRightCap.SetActive(rate >= 1.0f);
    }
}