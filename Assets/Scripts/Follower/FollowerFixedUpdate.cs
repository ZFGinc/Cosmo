using UnityEngine;

public class FollowerFixedUpdate: Follower
{
    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }
}

