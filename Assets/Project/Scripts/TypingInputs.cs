using System.Collections.Generic;
using System.Linq;

public class TypingInput {
    private List<string> _candidates;
    private string _currentInput = "";

    public void SetCandidates(List<string> candidates) {
        _candidates = candidates;
        _currentInput = "";
    }

    // 戻り値（0: 無効, 1: 継続, 2: 完了）
    public int Input(char c) {
        _currentInput += c;

        // prefix一致する候補だけ残す
        var matched = _candidates
            .Where(x => x.StartsWith(_currentInput))
            .ToList();

        if (matched.Count == 0) {
            _currentInput = _currentInput[..^1]; // ミス
            return 0;
        }

        _candidates = matched;
        if (_candidates.All(x => x.Length == _currentInput.Length)) return 2; // 完全一致チェック
        return 1;
    }

    public string GetCurrent() => _currentInput;
    public string GetCurrentCandidate() => _candidates.OrderBy(x => x.Length).First();
}