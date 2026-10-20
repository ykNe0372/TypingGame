using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
    public static GameStateManager Instance;
    public event Action<GameState> OnStateChanged;

    public GameState CurrentState { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ChangeState(GameState nextState) {
        CurrentState = nextState;
        Debug.Log($"GameState: {nextState}");

        OnStateChanged?.Invoke(nextState);
    }

    // 指定状態か
    public bool IsState(GameState state) {
        return CurrentState == state;
    }
}