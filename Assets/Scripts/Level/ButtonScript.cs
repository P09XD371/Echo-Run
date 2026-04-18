using UnityEngine;


public class ButtonScript : MonoBehaviour
{
    public GameObject door;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (door != null)
            {
                door.SetActive(false);
                Debug.Log("Door opened!");
            }
            else
            {
                Debug.LogWarning("Door not assigned in ButtonScript on " + gameObject.name);
            }
        }
    }

}
