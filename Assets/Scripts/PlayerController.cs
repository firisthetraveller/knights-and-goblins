using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    UnitController controller;
    Rigidbody2D body2D;

    [Header("Commands")]
    public InputAction projectileAction;
    public InputAction talkAction;

    [Header("Prefabs")]
    public GameObject projectilePrefab;

    private void Start()
    {
        controller = GetComponent<UnitController>();
        body2D = GetComponent<Rigidbody2D>();
        projectileAction.Enable();
        talkAction.Enable();

        projectileAction.performed += Launch;
        talkAction.performed += FindFriend;
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

    void FindFriend(InputAction.CallbackContext context)
    {
        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(body2D.position + (0.1f + 0.1f * i) * Vector2.down, controller.isFacingRight ? Vector2.right : Vector2.left, 1.5f, LayerMask.GetMask("Units"));
           // Debug.Log("Raycast");

            if (hit.collider != null)
            {
                UnitController otherController = hit.collider.gameObject.GetComponent<UnitController>();

                if (otherController.HasDialogue()) {
                    UIHandler.Instance.DisplayDialogue(otherController.NPCName, otherController.lines[Random.Range(0, otherController.lines.Count)]);
                } else {
                    Debug.Log($"Unit {otherController.NPCName} hit, but no dialogue has been set.");
                }
                return;
            }
        }
    }
}