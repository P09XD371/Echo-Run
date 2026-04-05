using UnityEngine;

[System.Serializable]
public struct Frame
{
    public Vector2 position;
    public bool flipX;

    public Frame(Vector2 pos, bool flip)
    {
        position = pos;
        flipX = flip;
    }
}