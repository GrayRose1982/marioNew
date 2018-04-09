using System;

#region  Character
[Flags]
public enum PlayerType
{
    None = 0,
    Small = 1 << 0,
    Grown = 1 << 1,
    Ice = 1 << 2,
    Winged = 1 << 3,
    Fire = 1 << 4,
    Super = 1 << 5,
}

[Flags]
public enum PlayerAction
{
    Idle = 0,
    Jump = 1 << 0,
    Carry = 1 << 1,
    Run = 1 << 2,
    Crouch = 1 << 3,
    Boot = 1 << 4,
    Climb = 1 << 5,
    SpeedUp = 1 << 6,
    Drag = 1 << 7,
    DragWall = 1 << 8,
}

public enum SpeedUp
{
    None = 0,
    Left = 1,
    Right = 2
}

public enum AttackType
{
    None,
    Jump,
    Fire,
    Ice,
    Hit,
    Dead,
    Reflect
}
#endregion

#region Button state
public enum PlayerMoveDirection
{
    None = 0,
    Left = 1,
    Right = 2
}

public enum JumpState
{
    None = 0,
    Press = 1,
    Hold = 2,
    Release = 3,
}

#endregion

#region Direction

public enum DirectionPipe
{
    Top,
    Bottom,
    Left,
    Right
}

#endregion
