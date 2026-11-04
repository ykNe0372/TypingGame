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

    public void SetItem(GrowthItem item, string description = null) {
        _item = item;
        SetContent(item.Icon, item.ItemName, description ?? item.Description);
    }

    public void SetRelic(RelicData relic) {
        _relic = relic;
        SetContent(relic.Icon, relic.RelicName, relic.Description);
    }

    public void SetBuff(BuffData buff) {
        SetContent(null, buff.BuffName, buff.BuffDescription);
    }

    public void SetOption(string title, string description) {
        SetContent(null, title, description);
    }

    private void SetContent(Sprite icon, string title, string description) {
        _icon.sprite = icon;
        _icon.enabled = icon != null;
        _nameText.text = title;
        _descriptionText.text = description;
    }

    public void SetIndex(int index) {
        _index = index;
    }

    public void SetSelected(bool value) {
        _highlight.SetActive(value);
    }
}