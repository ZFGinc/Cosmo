using UnityEngine;

public sealed class FollowerUpdate: Follower
{
    private void Update()
    {
        Move(Time.deltaTime);
    }
}

