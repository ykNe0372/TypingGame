public class TimeDuration : BuffDuration {
    private float _remainingTime;

    public TimeDuration(float seconds) {
        _remainingTime = seconds;
    }

    public override bool IsExpired => _remainingTime <= 0f;

    public override void Tick(float deltaTime) {
        _remainingTime -= deltaTime;
    }

}