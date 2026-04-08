using UnityEngine;

[System.Serializable]
public struct Frame
{
    public Vector2 position;
    public bool flipX;
    public bool attack;

    public Frame(Vector2 pos, bool flip, bool attackNow)
    {
        position = pos;
        flipX = flip;
        attack = attackNow;
    }
}
