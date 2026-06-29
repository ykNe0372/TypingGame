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

    public event Action<int> OnSelected;    // 何番目が選ばれているかだけ知る

    private void Update() {
        if (!_isOpen) return;
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) MoveLeft();
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) MoveRight();
        if (Keyboard.current.spaceKey.wasPressedThisFrame) Decide();
    }

    private void MoveLeft() {
        --_currentIndex;
        if (_currentIndex < 0) _currentIndex = _cards.Count - 1;
        RefreshSelection();
    }

    private void MoveRight() {
        ++_currentIndex;
        if (_currentIndex >= _cards.Count) _currentIndex = 0;
        RefreshSelection();
    }

    private void Decide() {
        OnSelected?.Invoke(_currentIndex);
    }

    // 選択表示の更新
    private void RefreshSelection() {
        for (int i=0; i<_cards.Count; ++i) {
            _cards[i].SetSelected(i == _currentIndex);
        }
    }

    public void OpenItems(List<GrowthItem> items) {
        gameObject.SetActive(true);
        _isOpen = true;
        _currentIndex = 0;
        _cards.Clear();  // 念の為初期化

        foreach (var item in items) {
            var card = Instantiate(_cardPrefab, _cardRoot);
            card.SetItem(item);
            card.SetIndex(_currentIndex);
            _cards.Add(card);
        }

        RefreshSelection();
    }

    public void OpenRelics(List<RelicData> relics) {
        gameObject.SetActive(true);
        _isOpen = true;
        _currentIndex = 0;
        _cards.Clear();  // 念の為初期化

        foreach (var relic in relics) {
            var card = Instantiate(_cardPrefab, _cardRoot);
            card.SetRelic(relic);
            card.SetIndex(_currentIndex);
            _cards.Add(card);
        }

        RefreshSelection();
    }

    public void Close() {
        _isOpen = false;
        foreach (var card in _cards) Destroy(card.gameObject);
        _cards.Clear();
        gameObject.SetActive(false);
    }
}