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
        ProcessInput();
    }

    void ProcessInput()
    {
        bool up = false, down = false, left = false, right = false;

        if(Input.GetButton("MoveUp"))
        {
            up = true;
            down = left = right = false;
        }

        if(Input.GetButton("MoveDown"))
        {
            down = true;
            up = left = right = false;
        }

        if(Input.GetButton("MoveLeft"))
        {
            left = true;
            up = down = right = false;
        }

        if(Input.GetButton("MoveRight"))
        {
            right = true;
            up = down = left = false;
        }
        
        animator.SetBool("up", up);
        animator.SetBool("down", down);
        animator.SetBool("left", left);
        animator.SetBool("right", right);
    }
}
