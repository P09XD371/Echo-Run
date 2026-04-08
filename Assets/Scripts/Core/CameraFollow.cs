using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    public Transform target;
    void LateUpdate()
    {
        Vector3 pos = target.position;
        pos.y += yOffset;
        pos.z = -10f;

        float ppu = 16f;
        pos.x = Mathf.Round(pos.x * ppu) / ppu;
        pos.y = Mathf.Round(pos.y * ppu) / ppu;

        transform.position = pos;
    }
}