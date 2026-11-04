using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class RewardSelectionUI : MonoBehaviour {
    [SerializeField] private RewardCardUI _cardPrefab;
    [SerializeField] private Transform _cardRoot;

    private readonly List<RewardCardUI> _cards = new();
    private int _currentIndex;
    private bool _isOpen;
    private bool _waitKeyRelease;

    public event Action<int> OnSelected;    // 何番目が選ばれているかだけ知る

    private void Update() {
        if (!_isOpen) return;
        if (_waitKeyRelease) {
            if (!Keyboard.current.anyKey.isPressed) _waitKeyRelease = false;
            return;
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) MoveLeft();
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) MoveRight();
        if (Keyboard.current.spaceKey.wasPressedThisFrame) Decide();
    }

    private void MoveLeft() {
        if (_cards.Count == 0) return;
        --_currentIndex;
        if (_currentIndex < 0) _currentIndex = _cards.Count - 1;
        RefreshSelection();
    }

    private void MoveRight() {
        if (_cards.Count == 0) return;
        ++_currentIndex;
        if (_currentIndex >= _cards.Count) _currentIndex = 0;
        RefreshSelection();
    }

    private void Decide() {
        if (_cards.Count == 0) return;
        OnSelected?.Invoke(_currentIndex);
    }

    // 選択表示の更新
    private void RefreshSelection() {
        for (int i=0; i<_cards.Count; ++i) {
            _cards[i].SetSelected(i == _currentIndex);
        }
    }

    public void OpenItems(IReadOnlyList<GrowthItem> items, IReadOnlyList<string> descriptions = null) {
        if (descriptions != null && descriptions.Count != items.Count) {
            Debug.LogError("Reward selection item descriptions must match the item count.");
            Close();
            return;
        }
        if (!BeginOpen(items.Count)) return;

        for (int i = 0; i < items.Count; ++i) {
            var card = Instantiate(_cardPrefab, _cardRoot);
            card.SetItem(items[i], descriptions == null ? null : descriptions[i]);
            AddCard(card);
        }

        RefreshSelection();
    }

    public void OpenRelics(IReadOnlyList<RelicData> relics) {
        if (!BeginOpen(relics.Count)) return;

        foreach (var relic in relics) {
            var card = Instantiate(_cardPrefab, _cardRoot);
            card.SetRelic(relic);
            AddCard(card);
        }

        RefreshSelection();
    }

    public void OpenBuffs(IReadOnlyList<BuffData> buffs) {
        if (!BeginOpen(buffs.Count)) return;

        foreach (var buff in buffs) {
            var card = Instantiate(_cardPrefab, _cardRoot);
            card.SetBuff(buff);
            AddCard(card);
        }

        RefreshSelection();
    }

    public void OpenOptions(IReadOnlyList<string> titles, IReadOnlyList<string> descriptions) {
        if (titles.Count != descriptions.Count) {
            Debug.LogError("Reward selection option titles and descriptions must have the same count.");
            Close();
            return;
        }
        if (!BeginOpen(titles.Count)) return;

        for (int i = 0; i < titles.Count; ++i) {
            var card = Instantiate(_cardPrefab, _cardRoot);
            card.SetOption(titles[i], descriptions[i]);
            AddCard(card);
        }

        RefreshSelection();
    }

    private bool BeginOpen(int count) {
        if (count == 0) {
            Debug.LogError("Cannot open reward selection UI without any options.");
            Close();
            return false;
        }

        ClearCards();
        gameObject.SetActive(true);
        _isOpen = true;
        _waitKeyRelease = Keyboard.current.anyKey.isPressed;
        _currentIndex = 0;
        return true;
    }

    private void AddCard(RewardCardUI card) {
        card.SetIndex(_cards.Count);
        _cards.Add(card);
    }

    public void Close() {
        _isOpen = false;
        ClearCards();
        gameObject.SetActive(false);
    }

    private void ClearCards() {
        foreach (var card in _cards) Destroy(card.gameObject);
        _cards.Clear();
    }
}