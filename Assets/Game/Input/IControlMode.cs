﻿using UnityEngine;
using UnityEngine.Events;

public interface IControlMode
{
    Vector3 AimDirection { get; }
    Vector3 MoveDirection { get; }
    UnityEvent Melee { get; }
    UnityEvent Fire { get; } // start firing
    bool IsFiring { get; } // Fire continuously
    UnityEvent Dash { get; }
    bool IsAiming { get; }

    bool AnyButtonPressed { get; } // for restarts etc...
}