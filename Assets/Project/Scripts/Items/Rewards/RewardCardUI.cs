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
    private RelicData _relic;
    private int _index;
    
    public GrowthItem Item => _item;
    public RelicData Relic => _relic;
    public int Index => _index;

    public void SetItem(GrowthItem item) {
        _item = item;

        _icon.sprite = item.Icon;
        _nameText.text = item.ItemName;
        _descriptionText.text = item.Description;
    }

    public void SetRelic(RelicData relic) {
        _relic = relic;

        _icon.sprite = relic.Icon;
        _nameText.text = relic.RelicName;
        _descriptionText.text = relic.Description;
    }

    public void SetIndex(int index) {
        _index = index;
    }

    public void SetSelected(bool value) {
        _highlight.SetActive(value);
    }
}