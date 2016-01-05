using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        walking = false;

        ProcessInput();
    }

    bool up = false;
    bool down = false;
    bool left = false;
    bool right = false;
    bool walking = false;

    void ProcessInput()
    {
        if(Input.GetButton("MoveUp"))
        {
            up = true;
            down = left = right = false;
            walking = true;
        }

        if(Input.GetButton("MoveDown"))
        {
            down = true;
            up = left = right = false;
            walking = true;
        }

        if(Input.GetButton("MoveLeft"))
        {
            left = true;
            up = down = right = false;
            walking = true;
        }

        if(Input.GetButton("MoveRight"))
        {
            right = true;
            up = down = left = false;
            walking = true;
        }
        
        animator.SetBool("up", up);
        animator.SetBool("down", down);
        animator.SetBool("left", left);
        animator.SetBool("right", right);
        animator.SetBool("walking", walking);
    }
}
