using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    private bool up, down, left, right;
    private bool walking;

    private float animationTime;

    private Vector3 walkingDirection;
    public int walkingSpeed;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ProcessInput();
        UpdatePlayer();
    }

    void ProcessInput()
    {
        walking = false;
        walkingDirection = Vector2.zero;

        if(Input.GetButton("MoveUp"))
        {
            up = true;
            down = left = right = false;
            walking = true;
            walkingDirection = Vector2.up;
        }

        if(Input.GetButton("MoveDown"))
        {
            down = true;
            up = left = right = false;
            walking = true;
            walkingDirection = Vector2.down;
        }

        if(Input.GetButton("MoveLeft"))
        {
            left = true;
            up = down = right = false;
            walking = true;
            walkingDirection = Vector2.left;
        }

        if(Input.GetButton("MoveRight"))
        {
            right = true;
            up = down = left = false;
            walking = true;
            walkingDirection = Vector2.right;
        }
    }

    void UpdatePlayer()
    {
        animator.SetBool("up", up);
        animator.SetBool("down", down);
        animator.SetBool("left", left);
        animator.SetBool("right", right);
        animator.SetBool("walking", walking);

        transform.position += walkingDirection * walkingSpeed * Time.deltaTime;
        GetComponent<SpriteRenderer>().sortingOrder = (int) -transform.position.y;
    }
}
