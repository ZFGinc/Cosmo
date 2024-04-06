using UnityEngine;

public class FollowerUpdate: Follower
{
    private void Update()
    {
        Move(Time.deltaTime);
    }
}

