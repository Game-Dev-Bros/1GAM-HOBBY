#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    private bool up, down, left, right;
    private bool walking;

    private float animationTime;

    private Vector3 walkingDirection;
    public int walkingSpeed;

    public GameObject interactiveObject;
    public bool interacting;

    public DialogController dialogController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        dialogController.gameObject.SetActive(true);
    }

    void Update()
    {
        ProcessInput();
        UpdatePlayer();
    }

    void ProcessInput()
    {
        walking = false;

        if (Input.GetButton("MoveUp"))
        {
            up = true;
            down = left = right = false;
            walking = true;
            walkingDirection = Vector2.up;
        }

        if (Input.GetButton("MoveDown"))
        {
            down = true;
            up = left = right = false;
            walking = true;
            walkingDirection = Vector2.down;
        }

        if (Input.GetButton("MoveLeft"))
        {
            left = true;
            up = down = right = false;
            walking = true;
            walkingDirection = Vector2.left;
        }

        if (Input.GetButton("MoveRight"))
        {
            right = true;
            up = down = left = false;
            walking = true;
            walkingDirection = Vector2.right;
        }

        if(Input.GetButton("Action") && interactiveObject != null)
        {
            interacting = true;
        }
    }

    void UpdatePlayer()
    {
        Rect wallRect = new Rect(transform.position - new Vector3(0.5f, 0.5f) + walkingDirection * 0.15f, Vector3.one);
        Rect interactionRect = new Rect(transform.position - new Vector3(0.5f, 0.5f) + walkingDirection, Vector3.one);

        Collider2D wallCollider = Physics2D.OverlapArea(wallRect.min, wallRect.max);
        if (wallCollider != null)
        {
            walking = false;
        }

        Collider2D interactionCollider = Physics2D.OverlapArea(wallRect.min, wallRect.max);
        if(interactionCollider != null && interactionCollider.tag == "Interactive")
        {
            InteractableController interactableController = interactionCollider.gameObject.GetComponent<InteractableController>();

            if(interactableController != null)
            {
                interactiveObject = interactionCollider.gameObject;
                dialogController.Show(interactableController.actions);
            }
        }
        else
        {
            interactiveObject = null;
            interacting = false;
            dialogController.Hide();
        }

        animator.SetBool("up", up);
        animator.SetBool("down", down);
        animator.SetBool("left", left);
        animator.SetBool("right", right);
        animator.SetBool("walking", walking);

        transform.position += walkingDirection * (walking ? 1 : 0) * walkingSpeed * Time.deltaTime;
        GetComponent<SpriteRenderer>().sortingOrder = (int)-transform.position.y - 1;
    }
}
