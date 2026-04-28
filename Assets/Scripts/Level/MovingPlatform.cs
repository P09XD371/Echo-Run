using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Vector3 nextPosition;

    void Start()
    {
        nextPosition = new Vector3(
            pointA.position.x,
            pointA.position.y,
            0f
        );
    }

    void Update()
    {
        Vector3 targetPosition = new Vector3(
            nextPosition.x,
            nextPosition.y,
            0f
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (nextPosition.x == pointA.position.x &&
                nextPosition.y == pointA.position.y)
            {
                nextPosition = new Vector3(
                    pointB.position.x,
                    pointB.position.y,
                    0f
                );
            }
            else
            {
                nextPosition = new Vector3(
                    pointA.position.x,
                    pointA.position.y,
                    0f
                );
            }
        }
    }
}