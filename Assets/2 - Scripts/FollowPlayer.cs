using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    [SerializeField] private bool lockZ = false;


    private void FixedUpdate()
    {
        Vector3 newPos = player.position;
        
        if (lockX)
            newPos.x = transform.position.x;

        if (lockY)
            newPos.y = transform.position.y;

        if (lockZ)
            newPos.z = transform.position.z;

        transform.position = newPos;
    }
}
