﻿using UnityEngine;

public interface IControllable
{
    void Move(Vector3 direction);
    void Jump();
    void PickUp();
    void Action();
}
