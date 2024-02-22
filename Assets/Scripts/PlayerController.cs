using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    UnitController controller;
    Rigidbody2D body2D;

    [Header("Commands")]
    public InputAction projectileAction;

    [Header("Prefabs")]
    public GameObject projectilePrefab;

    private void Start()
    {
        controller = GetComponent<UnitController>();
        body2D = GetComponent<Rigidbody2D>();
        projectileAction.Enable();

        projectileAction.performed += Launch;
    }

    private void Update()
    {
        controller.move = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        controller.move.Normalize();
    }

    void Launch(InputAction.CallbackContext context)
    {
        if (!projectilePrefab)
            return;

        GameObject projectileObject = Instantiate(projectilePrefab, body2D.position + Vector2.up * 0.2f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.originShooter = gameObject;
        if (!controller.isFacingRight) projectile.FlipSprite();
        projectile.Launch(controller.isFacingRight ? Vector2.right : Vector2.left, 300);
        // animator.SetTrigger("Launch");
    }

}