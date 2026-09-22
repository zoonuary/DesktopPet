namespace DesktopPet.Behavior;

// React는 아직 이 컨트롤러가 소유하지 않는다 (클릭 반응 입력 연결 전).
public sealed class PetBehaviorController
{
    private readonly Random _random;

    private double _remainingSeconds;

    public PetBehaviorController(Random? random = null)
    {
        _random = random ?? new Random();
        _remainingSeconds = NextIdleDuration();
    }

    public PetStateId State { get; private set; } = PetStateId.Idle;

    public PetFacing Facing { get; private set; } = PetFacing.Right;

    public PetMode Mode { get; private set; } = PetMode.Normal;

    public void SetMode(PetMode mode)
    {
        if (Mode == mode)
        {
            return;
        }

        Mode = mode;
        EnterModeDefaultState();
    }

    public void NotifyBoundaryHit()
    {
        if (State == PetStateId.Walk)
        {
            EnterIdle();
        }
    }

    public void EnterDrag()
    {
        State = PetStateId.Drag;
    }

    public void ExitDrag()
    {
        EnterModeDefaultState();
    }

    public void Tick(double elapsedSeconds)
    {
        _remainingSeconds -= elapsedSeconds;
        if (_remainingSeconds > 0)
        {
            return;
        }

        switch (State)
        {
            case PetStateId.Idle:
                ExitIdle();
                break;
            case PetStateId.Walk:
                EnterIdle();
                break;
            case PetStateId.Rest:
                ExitRest();
                break;
            case PetStateId.Sleep:
                EnterIdle();
                break;
        }
    }

    private void EnterModeDefaultState()
    {
        switch (Mode)
        {
            case PetMode.Stay:
                EnterRest();
                break;
            case PetMode.Focus:
                EnterSleep();
                break;
            case PetMode.Normal:
                EnterIdle();
                break;
        }
    }

    private void ExitIdle()
    {
        if (Mode != PetMode.Normal)
        {
            EnterModeDefaultState();
            return;
        }

        if (_random.NextDouble() < 0.35)
        {
            EnterWalk();
        }
        else
        {
            EnterRest();
        }
    }

    private void ExitRest()
    {
        if (Mode != PetMode.Normal)
        {
            EnterModeDefaultState();
            return;
        }

        if (_random.NextDouble() < 0.75)
        {
            EnterIdle();
        }
        else
        {
            EnterSleep();
        }
    }

    private void EnterIdle()
    {
        State = PetStateId.Idle;
        _remainingSeconds = NextIdleDuration();
    }

    private void EnterWalk()
    {
        State = PetStateId.Walk;
        Facing = _random.NextDouble() < 0.5 ? PetFacing.Left : PetFacing.Right;
        _remainingSeconds = NextWalkDuration();
    }

    private void EnterRest()
    {
        State = PetStateId.Rest;
        _remainingSeconds = NextRestDuration();
    }

    private void EnterSleep()
    {
        State = PetStateId.Sleep;
        _remainingSeconds = NextSleepDuration();
    }

    private double NextIdleDuration() => 8.0 + (_random.NextDouble() * 12.0);

    private double NextWalkDuration() => 3.0 + (_random.NextDouble() * 4.0);

    private double NextRestDuration() => 15.0 + (_random.NextDouble() * 25.0);

    private double NextSleepDuration() => 30.0 + (_random.NextDouble() * 60.0);
}
