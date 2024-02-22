using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitController : MonoBehaviour
{
    protected Rigidbody2D body2D;
    private Animator animator;
    public GameObject projectilePrefab;
    private UIHealthIcon healthBar;

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

    private void Awake()
    {
        tag = "Unit";
        currentHealth = maxHealth;
    }

    // Start is called before the first frame update
    private void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<UIHealthIcon>();
        SetHealthBarValue();

        if (!isFacingRight)
        {
            FlipSprite();
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    private void FixedUpdate()
    {
        Vector2 position = body2D.position + Time.deltaTime * moveSpeed * move;
        body2D.MovePosition(position);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        FlipSprite();
    }

    private void FlipSprite()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        healthBar.FlipSprite();
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

        if (Input.GetButtonDown("Projectile"))
        {
            Launch();
        }
    }

    void Launch()
    {
        if (!projectilePrefab)
            return;

        GameObject projectileObject = Instantiate(projectilePrefab, body2D.position + Vector2.up * 0.3f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.originShooter = gameObject;
        if (!isFacingRight) projectile.FlipSprite();
        projectile.Launch(isFacingRight ? Vector2.right : Vector2.left, 300);
        // animator.SetTrigger("Launch");
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
        else if (amount == 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        SetHealthBarValue();

        if (!IsAlive())
        {
            animator.SetTrigger("Dead");
            move = Vector2.zero;
            Destroy(gameObject, 1.15f);
        }
    }

    private void SetHealthBarValue()
    {
        int uiHealthValue = (int)Mathf.Ceil(currentHealth / 10);
        healthBar.SetHealthValue((int)Mathf.Ceil(currentHealth / 10));
        healthBar.gameObject.SetActive(uiHealthValue < 10);
    }

    private IEnumerator InvincibilityOff()
    {
        yield return new WaitForSeconds(invincibilityInSeconds);
        isInvincible = false;
        Debug.Log($"i-frames went away!");
    }
}
