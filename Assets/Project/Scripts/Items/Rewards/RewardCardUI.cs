using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class RewardCardUI : MonoBehaviour {
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;

    [SerializeField, Header("Selection")] private GameObject _highlight;

    private GrowthItem _item;
    
    public GrowthItem Item => _item;

    public void SetItem(GrowthItem item) {
        _item = item;

        _icon.sprite = item.Icon;
        _nameText.text = item.ItemName;
        _descriptionText.text = item.Description;
    }

    public void SetSelected(bool value) {
        _highlight.SetActive(value);
    }
}