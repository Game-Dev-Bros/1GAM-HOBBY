#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private const string FLOOR_CHANGE_KEY = "ChangedFloor";
    public static bool IsChangingLevels = true;

    private Animator animator;

    private bool walking;

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
     
    private int lastStep = 0;
    public StairsController currentStairs;


    private ScreenFader screenFader;

    void Awake()
    {
        var hasChangedFloors = PlayerPrefs.GetInt(FLOOR_CHANGE_KEY, -1);
        Debug.Log("has changed floor "+hasChangedFloors);

        animator = GetComponent<Animator>();
        dialogController.gameObject.SetActive(true);
        playerOrientation = PlayerOrientation.Down;


        if (hasChangedFloors == 1)
        {
            if (SceneManager.GetActiveScene().name == "Level 0")
            {
                transform.position = new Vector3(-1.4f, 3.88f, 0f);
                animator.SetBool("right", true);
                animator.SetBool("walking", false);
                playerOrientation = PlayerOrientation.Right;
            }
            else if (SceneManager.GetActiveScene().name == "Level 1")
            {
                transform.position = new Vector3(-1f, 3.97f, 0f);
                animator.SetBool("right", true);
                animator.SetBool("walking", false);
                playerOrientation = PlayerOrientation.Right;
            }

            PlayerPrefs.SetInt(FLOOR_CHANGE_KEY, 0);
        }

        screenFader = GameObject.Find("Fader").GetComponent<ScreenFader>();
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
            walking = true;
            playerOrientation = PlayerOrientation.Up;
        }

        if (Input.GetButton("MoveDown"))
        {
            walking = true;
            playerOrientation = PlayerOrientation.Down;
        }

        if (Input.GetButton("MoveLeft"))
        {
            walking = true;
            playerOrientation = PlayerOrientation.Left;
        }

        if (Input.GetButton("MoveRight"))
        {
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
        if (screenFader.isRunning)
        {
            return;
        }
        
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
            if (wallCollider != null && currentStairs == null)
            {
                walking = walking && wallCollider.isTrigger;
            }
        }

        Collider2D[] interactionColliders = Physics2D.OverlapAreaAll(interactionRect.min, interactionRect.max);
        
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

        if(currentStairs != null)
        {
            switch(currentStairs.ascendingDirection)
            {
                case PlayerOrientation.Up:
                case PlayerOrientation.Down:
                    walking = walking && (playerOrientation == PlayerOrientation.Up || playerOrientation == PlayerOrientation.Down);
                    break;
                case PlayerOrientation.Left:
                case PlayerOrientation.Right:
                    walking = walking && (playerOrientation == PlayerOrientation.Left || playerOrientation == PlayerOrientation.Right);
                    break;
            }
        }

        animator.SetBool("up", playerOrientation == PlayerOrientation.Up);
        animator.SetBool("down", playerOrientation == PlayerOrientation.Down);
        animator.SetBool("left", playerOrientation == PlayerOrientation.Left);
        animator.SetBool("right", playerOrientation == PlayerOrientation.Right);
        animator.SetBool("walking", walking);

        transform.position += walkingDirection * (walking ? 1 : 0) * walkingSpeed * Time.deltaTime;
        GetComponent<SpriteRenderer>().sortingOrder = (currentStairs != null) ? currentStairs.playerZOrder : (int)-transform.position.y - 1;
    }
    
    public void SetStairs(StairsController stairs)
    {
        currentStairs = stairs;

        float stepDx = 0, stepDy = 0;
        float currentStep = 0;

        if(stairs != null)
        {
            switch(stairs.ascendingDirection)
            {
                case PlayerOrientation.Up:
                case PlayerOrientation.Down:
                    break;

                case PlayerOrientation.Left:
                    currentStep = (stairs.bounds.max.x - transform.position.x) / (stairs.bounds.max.x - stairs.bounds.min.x);
                    stepDy = stairs.bounds.size.y / stairs.steps;
                    if(playerOrientation == PlayerOrientation.Right)
                    {
                        stepDy = -stepDy;
                    }
                    break;

                case PlayerOrientation.Right:
                    currentStep = (transform.position.x - stairs.bounds.min.x) / (stairs.bounds.max.x - stairs.bounds.min.x);
                    stepDy = stairs.bounds.size.y / stairs.steps;
                    if(playerOrientation == PlayerOrientation.Left)
                    {
                        stepDy = -stepDy;
                    }
                    break;
            }

            currentStep = (int) (currentStep * stairs.steps);
        }

        if(lastStep != currentStep)
        {
            lastStep = (int) currentStep;
            Vector3 newPosition = transform.position;
            newPosition.x += stepDx;
            newPosition.y += stepDy;
            transform.position = newPosition;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (PlayerPrefs.GetInt(FLOOR_CHANGE_KEY, -1) == 1)
            return;

        if (other.tag == "DownstairsTransition")
        {
            Debug.Log(other.transform.name);
            PlayerPrefs.SetInt(FLOOR_CHANGE_KEY, 1);
            StartCoroutine(screenFader.FadeToScene("Level 0"));
        }
        else if (other.tag == "UpstairsTransition")
        {
            Debug.Log(other.transform.name);
            PlayerPrefs.SetInt(FLOOR_CHANGE_KEY, 1);
            StartCoroutine(screenFader.FadeToScene("Level 1"));
        }
    }
}
