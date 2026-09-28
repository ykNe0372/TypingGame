using UnityEngine;

public class DamagePopupManager : MonoBehaviour {
    [SerializeField] private DamagePopup _popupPrefab;
    [SerializeField] private Canvas _canvas;
    [SerializeField, Header("表示座標")] private Vector3 _worldOffset = new(0.5f, 1.0f, 0f);

    public void Show(DamageContext ctx) {
        int damage = Mathf.FloorToInt(Mathf.Max(0f, ctx.FinalDamage));

        DamagePopup popup = Instantiate(_popupPrefab, _canvas.transform);
        popup.Initialize(damage);
        
        Vector3 worldPosition = ctx.Target.transform.position + _worldOffset;    // 敵右上に表示するよう調整
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);  // 敵座標をスクリーン座標に変更
        RectTransform canvasRect = _canvas.transform as RectTransform;           // スクリーン座標をローカル座標に変換
        RectTransform popupRect = popup.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 localPosition
        );

        popupRect.anchoredPosition = localPosition;  // Canvas 上の座標として設定
    }
}