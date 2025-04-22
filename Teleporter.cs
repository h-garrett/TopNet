using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform rootObject = other.transform.root;

        Rigidbody2D rb = rootObject.GetComponent<Rigidbody2D>();
        Vector2 currentVelocity = rb.linearVelocity;

        // Teleport the ball to the other side of the map
        rootObject.transform.position = new Vector2(-rootObject.transform.position.x, rootObject.transform.position.y);

        // Restore the velocity after teleportation
        rb.linearVelocity = currentVelocity;
    }
}
