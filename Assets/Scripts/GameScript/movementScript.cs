using UnityEngine;
using UnityEngine.InputSystem; 

public class movementScript : MonoBehaviour
{
    public float speed = 5f;

    public bool moveLeft = false;
    public bool moveRight = false;
    public bool ignoreTeammates = false; // Check this box in the inspector if you want this character to ignore teammates (e.g., soldiers ignore other soldiers)

    // [Header("Team Settings")]
    // Here is your checkbox! 
    // Unchecked (false) = Soldier, Checked (true) = Enemy.
    // public bool isEnemy = false; 

    private Rigidbody2D rb;
    private Collider2D myCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();

        if (ignoreTeammates)
        {
            IgnoreTeammates();
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            ChangeDirection(true, false);
        }
        else if (Keyboard.current.aKey.wasReleasedThisFrame || Keyboard.current.leftArrowKey.wasReleasedThisFrame)
        {
            ChangeDirection(false, moveRight); 
        }

        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            ChangeDirection(false, true);
        }
        else if (Keyboard.current.dKey.wasReleasedThisFrame || Keyboard.current.rightArrowKey.wasReleasedThisFrame)
        {
            ChangeDirection(moveLeft, false);
        }
    }

    void FixedUpdate()
    {
        float currentDirection = 0f;

        if (moveLeft)
        {
            currentDirection = -1f; 
        }
        else if (moveRight)
        {
            currentDirection = 1f; 
        }

        rb.linearVelocity = new Vector2(currentDirection * speed, rb.linearVelocity.y);
    }

    public void ChangeDirection(bool goLeft, bool goRight)
    {
        moveLeft = goLeft;
        moveRight = goRight;
    }

    public void IgnoreTeammates()
    {
        movementScript[] allPiecesInGame = FindObjectsOfType<movementScript>();

        foreach (movementScript piece in allPiecesInGame)
        {
            // We now check if the Unity Tags match instead of the old string!
            if (piece.gameObject.CompareTag(this.gameObject.tag) && piece.gameObject != this.gameObject)
            {
                Collider2D teammateCollider = piece.GetComponent<Collider2D>();

                if (teammateCollider != null && this.myCollider != null)
                {
                    Physics2D.IgnoreCollision(this.myCollider, teammateCollider);
                }
            }
        }
    }

    public void ignoreTeammatesToggle()
    {
        IgnoreTeammates();
    }
}