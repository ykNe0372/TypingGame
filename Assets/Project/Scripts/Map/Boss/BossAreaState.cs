public enum BossAreaState {
    None,
    PreOrganize,      // 戦闘前の整理エリア
    Battle,
    PostOrganize,     // 戦闘後の整理エリア
    SelectNextFloor,  // 次階層の行き先を選択
    Complete
}