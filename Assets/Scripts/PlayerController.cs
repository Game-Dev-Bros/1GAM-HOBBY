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

    public enum PlayerOrientation
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    private PlayerOrientation playerOrientation;
     
    void Awake()
    {
        animator = GetComponent<Animator>();
        dialogController.gameObject.SetActive(true);
        playerOrientation = PlayerOrientation.Down;
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
            playerOrientation = PlayerOrientation.Up;
        }

        if (Input.GetButton("MoveDown"))
        {
            down = true;
            up = left = right = false;
            walking = true;
            playerOrientation = PlayerOrientation.Down;
        }

        if (Input.GetButton("MoveLeft"))
        {
            left = true;
            up = down = right = false;
            walking = true;
            playerOrientation = PlayerOrientation.Left;
        }

        if (Input.GetButton("MoveRight"))
        {
            right = true;
            up = down = left = false;
            walking = true;
            playerOrientation = PlayerOrientation.Right;
        }

        if(Input.GetButton("Action") && interactiveObject != null)
        {
            interacting = true;
        }
    }

    void UpdatePlayer()
    {
        switch(playerOrientation)
        {
            case PlayerOrientation.Up:
                walkingDirection = Vector2.up;
                break;
            case PlayerOrientation.Down:
                walkingDirection = Vector2.down;
                break;
            case PlayerOrientation.Left:
                walkingDirection = Vector2.left;
                break;
            case PlayerOrientation.Right:
                walkingDirection = Vector2.right;
                break;
        }

        Rect wallRect = new Rect(transform.position - new Vector3(0.5f, 0.5f) + walkingDirection * 0.15f, Vector3.one);
        Rect interactionRect = new Rect(transform.position - new Vector3(0.5f, 0.5f) + walkingDirection, Vector3.one);

        Collider2D[] wallColliders = Physics2D.OverlapAreaAll(wallRect.min, wallRect.max);

        foreach(Collider2D wallCollider in wallColliders)
        {
            if (wallCollider != null)
            {
                walking = walking && wallCollider.isTrigger;
            }
        }

        Collider2D[] interactionColliders = Physics2D.OverlapAreaAll(wallRect.min, wallRect.max);
        
        if(interactionColliders.Length == 0)
        {
            interactiveObject = null;
            interacting = false;
            dialogController.Hide();
        }

        foreach(Collider2D interactionCollider in interactionColliders)
        {
            if(interactionCollider.tag == "Interactive")
            {
                InteractableController interactableController = interactionCollider.gameObject.GetComponent<InteractableController>();

                if(interactableController.playerOrientation == PlayerOrientation.None || interactableController.playerOrientation == playerOrientation)
                {
                    interactiveObject = interactionCollider.gameObject;
                    dialogController.Show(interactableController.actions);
                    break;
                }
                else
                {
                    interactiveObject = null;
                    interacting = false;
                    dialogController.Hide();
                }
            }
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
