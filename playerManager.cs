using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    public Color[] playerColors = new Color[2];


    private static int colorIndex = 0;
    private static int playerCount = 0;
    [SerializeField] public TilemapCollider2D goalA;
    [SerializeField] public Tilemap goalA_sprite;
    [SerializeField] public TilemapCollider2D goalB;
    [SerializeField] public Tilemap goalB_sprite;
    private Color playerColor;


    private void Awake()
    {
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    void OnPlayerJoined(PlayerInput player)
    {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        newPlayerMovement playerController = player.GetComponent<newPlayerMovement>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = playerColors[colorIndex % playerColors.Length];
            colorIndex++; // Move to the next color for the next player
        }
        playerColor = spriteRenderer.color;

        if (playerCount == 0) // First player
        {
            playerController.assignedGoal = goalB; // Attack Goal B
            player.gameObject.tag = "PlayerOne";
            goalA_sprite.color = playerColor;
        }
        else if (playerCount == 1) // Second player
        {
            Vector2 newPosition = player.transform.position;
            newPosition.x *= -1f;
            player.transform.position = newPosition;

            playerController.assignedGoal = goalA; // Attack Goal A
            player.gameObject.tag = "PlayerTwo";
            goalB_sprite.color = playerColor;
        }

        playerCount++;
    }
}
