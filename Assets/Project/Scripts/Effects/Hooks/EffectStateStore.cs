using System.Collections.Generic;

// Effect / RelicEffect（ScriptableObject）ごとの実行時状態をキャラクター単位で保持する
// ScriptableObject はアセットとして共有されるため、スタック数やタイマーをフィールドに持つと
// 全キャラクターで共有されたり、エディタ上で値が残ったりする。状態はここに置く。
public class EffectStateStore {
    private readonly Dictionary<object, object> _states = new();

    public T Get<T>(object key) where T : class, new() {
        if (_states.TryGetValue(key, out var state) && state is T typed) return typed;

        typed = new T();
        _states[key] = typed;
        return typed;
    }

    public void Remove(object key) {
        _states.Remove(key);
    }
}
