﻿using UnityEngine;
using UnityEngine.Events;

public interface IControlMode
{
    Vector3 AimDirection { get; }
    Vector3 MoveDirection { get; }
    UnityEvent Melee { get; }
    UnityEvent Fire { get; }
    UnityEvent Dash { get; }
    bool IsAiming { get; }
}