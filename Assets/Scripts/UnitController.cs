using System.Collections;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    protected Rigidbody2D rigidbody2D;
    private Animator animator;

    [HideInInspector]
    public Vector2 move;

    public bool isFacingRight = true;

    [Header("Floating invincibility")]
    public float invincibilityInSeconds = 2.0f;
    bool isInvincible = false;

    [Header("Unit stats")]
    public int maxHealth = 100;
    private int currentHealth;
    public int Health => currentHealth;

    [Header("Unit settings")]
    public float moveSpeed = 5;

    // Start is called before the first frame update
    private void Start()
    {
        tag = "Unit";
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (!isFacingRight) {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody2D.position + Time.deltaTime * moveSpeed * move;
        rigidbody2D.MovePosition(position);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing
        isFacingRight = !isFacingRight;

        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // Update is called once per frame
    private void Update()
    {
        animator.SetBool("isMoving", move.x != 0 || move.y != 0);

        // Don't reverse animation if only moving up or down
        if (move.x < 0 && isFacingRight)
        {
            Flip();
        }
        else if (move.x > 0 && !isFacingRight)
        {
            Flip();
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                Debug.Log($"Hit during i-frames!");
                return;
            }

            if (invincibilityInSeconds > 0)
            {
                isInvincible = true;
                Debug.Log($"i-frames on!");
                StartCoroutine(InvincibilityOff());
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.Instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    private IEnumerator InvincibilityOff()
    {
        yield return new WaitForSeconds(invincibilityInSeconds);
        isInvincible = false;
        Debug.Log($"i-frames went away!");
    }
}
