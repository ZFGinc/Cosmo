using UnityEngine;

public sealed class FollowerFixedUpdate: Follower
{
    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }
}

