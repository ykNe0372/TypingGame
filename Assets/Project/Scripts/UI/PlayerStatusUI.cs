using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour {
    [SerializeField] private Character _player;
    [Header("HP")]
    [SerializeField] private RectTransform _hpMask;
    [SerializeField] private Image _hpLiquidHead;
    [SerializeField] private GameObject _hpRightCap;
    [SerializeField] private float _hpBarMaxWidth;
    [SerializeField] private Material _hpLiquidMaterial;
    [Header("MP")]
    [SerializeField] private RectTransform _mpMask;
    [SerializeField] private Image _mpLiquidHead;
    [SerializeField] private GameObject _mpRightCap;
    [SerializeField] private float _mpBarMaxWidth;
    [SerializeField] private Material _mpLiquidMaterial;

    private void Awake() {
        _player.OnHPChanged += UpdateHP;
        _player.OnMPChanged += UpdateMP;
    }

    private void UpdateHP(float currentHP, float maxHP) {
        float rate = Mathf.Clamp01(currentHP / maxHP);
        RectTransform rect = _hpLiquidHead.rectTransform;
        
        Vector2 maskSize = _hpMask.sizeDelta;
        maskSize.x = _hpBarMaxWidth * rate;
        _hpMask.sizeDelta = maskSize;

        rect.anchoredPosition = new Vector2(maskSize.x + rect.rect.width / 2, rect.anchoredPosition.y);
        Material newHPLiquidMaterial = (rate >= 1.0f) ? null : _hpLiquidMaterial;
        _hpLiquidHead.material = newHPLiquidMaterial;

        _hpRightCap.SetActive(rate >= 1.0f);
    }

    private void UpdateMP(float currentMP, float maxMP) {
        float rate = Mathf.Clamp01(currentMP / maxMP);
        RectTransform rect = _mpLiquidHead.rectTransform;

        Vector2 maskSize = _mpMask.sizeDelta;
        maskSize.x = _mpBarMaxWidth * rate;
        _mpMask.sizeDelta = maskSize;

        rect.anchoredPosition = new Vector2(maskSize.x + rect.rect.width / 2, rect.anchoredPosition.y);
        Material newMPLiquidMaterial = (rate >= 1.0f) ? null : _mpLiquidMaterial;
        _mpLiquidHead.material = newMPLiquidMaterial;

        _mpRightCap.SetActive(rate >= 1.0f);
    }
}