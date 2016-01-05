﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    private bool up, down, left, right;
    private bool walking;

    private Vector2 previousPosition, nextPosition;
    public float walkingSpeed;

    private float animationTime;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        walking = false;

        ProcessInput();
        UpdatePlayer();
    }

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

    void UpdatePlayer()
    {
        AnimatorStateInfo clip = animator.GetCurrentAnimatorStateInfo(0);

        if(clip.normalizedTime < animationTime)
        {
            previousPosition = nextPosition;
        }
        else if(clip.IsName("Walk Up"))
        {
            nextPosition = previousPosition + Vector2.up;
        }
        else if(clip.IsName("Walk Down"))
        {
            nextPosition = previousPosition + Vector2.down;
        }
        else if(clip.IsName("Walk Left"))
        {
            nextPosition = previousPosition + Vector2.left;
        }
        else if(clip.IsName("Walk Right"))
        {
            nextPosition = previousPosition + Vector2.right;
        }

        animationTime = clip.normalizedTime;
        transform.position = Vector2.Lerp(previousPosition, nextPosition, animationTime);
    }
}
