using UnityEngine;
using Unity.Netcode;

public class Ball : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ParticleSystem goalExplosion;
    [SerializeField] public Vector2 respawnPosition;

    private ParticleSystem goalExplosionInstance;
    private NetworkVariable<Vector2> networkedVelocity = new NetworkVariable<Vector2>(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    Rigidbody rb;


    public float speedMultiplier = 1.5f; // Adjusts how strongly players affect the ball

    private Vector2 velocity; // Stores current velocity of the ball


    void FixedUpdate()
    {

    }

    void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("goalCollider"))
        {
            Goal();
        }

    }




    private void Goal()
    {
        print("Goal!");
        Instantiate(goalExplosion, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        Invoke(nameof(respawnBall), 3f);

    }

    private void respawnBall()
    {
        transform.position = respawnPosition;
        velocity = Vector2.zero;
        gameObject.SetActive(true);
    }


}
