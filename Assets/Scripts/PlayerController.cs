#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private GameObject eventSystem;
    private bool walking, isPaused;
    private MusicPlayer mplayer;
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
    private ClockManager clock;
    private SliderController pointer;
    private ScreenFader screenFader;
    private GameManager gameManager;

    private bool isForceSleeping;

    void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        mplayer = GameObject.Find("PersistentDataObject").GetComponent<MusicPlayer>();
        clock = GameObject.Find("Clock").GetComponent<ClockManager>();
        pointer = GetComponent<SliderController>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        dialogController.gameObject.SetActive(true);
        playerOrientation = PlayerOrientation.Down;
        
        isForceSleeping = (PlayerPrefs.GetInt(Constants.Prefs.FORCE_SLEEPING, Constants.Prefs.Defaults.FORCE_SLEEPING) == 1);

        bool hasSavedGame = (PlayerPrefs.GetFloat(Constants.Prefs.GAME_TIME, Constants.Prefs.Defaults.GAME_TIME) > 0);
        if(hasSavedGame)
        {
            LoadPlayerData();
        }

        bool hasChangedFloor = PlayerPrefs.GetInt(Constants.Prefs.CHANGING_FLOOR, Constants.Prefs.Defaults.CHANGING_FLOOR) == 1;
        if(isForceSleeping)
        {
            isForceSleeping = false;
            PlayerPrefs.SetInt(Constants.Prefs.FORCE_SLEEPING, Constants.Prefs.Defaults.FORCE_SLEEPING);
            SavePlayerData();
        }
        else if (hasChangedFloor)
        {
            mplayer.StopLoopedFootsteps();
            transform.position = new Vector3(-0.86f, 3.88f, 0f);
            playerOrientation = PlayerOrientation.Right;

            PlayerPrefs.SetInt(Constants.Prefs.CHANGING_FLOOR, Constants.Prefs.Defaults.CHANGING_FLOOR);
            
            SavePlayerData();
        }

        screenFader = GameObject.Find("Fader").GetComponent<ScreenFader>();

        animator.SetBool("up", playerOrientation == PlayerOrientation.Up);
        animator.SetBool("down", playerOrientation == PlayerOrientation.Down);
        animator.SetBool("left", playerOrientation == PlayerOrientation.Left);
        animator.SetBool("right", playerOrientation == PlayerOrientation.Right);
        animator.SetBool("walking", false);
    }

    void Update()
    {
        ProcessInput();
        if (!isPaused)
        {
            UpdatePlayer();
            StartCoroutine(CheckSleep());
        }
    }

    void ProcessInput()
    {
        walking = false;

        if (!isPaused)
        {
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

            if (Input.GetButtonDown("Action") && interactiveObject != null && !interacting)
            {
                StartCoroutine(Interact());
            }
        }

        if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
    }

    IEnumerator Interact()
    {
        Action action = dialogController.currentAction;

        if(action != null && action.active && action.interactable)
        {
            interacting = true;
            mplayer.PlayInteraction();
        
            walking = false;
            animator.SetBool("walking", walking);

            if(action.tag == Constants.Actions.SUBMIT_THESIS)
            {
                yield return GameObject.FindObjectOfType<GameManager>().EndGame(true);
            }
            else if(action.tag == Constants.Actions.READ_LETTER)
            {
                interacting = false;
                action.active = false;

                Action letterAction = gameManager.GetActionWithTag(Constants.Actions.OPENED_LETTER);
                letterAction.text = Constants.Strings.LETTER_MESSAGE;
                letterAction.active = true;
                interactiveObject = null;

                dialogController.background.rectTransform.sizeDelta += new Vector2(200, 200);
                dialogController.dialogText.rectTransform.sizeDelta += new Vector2(200, 200);
            }
            else
            {
                yield return screenFader.FadeToColor(Constants.Colors.FADE, 0.25f);
                pointer.value += action.statModifier;
                clock.AddMinutes(action.duration);
                yield return screenFader.FadeToColor(Color.clear, 0.25f);
            }
        }

        interacting = false;

        yield return null;
    }

    void UpdatePlayer()
    {
        if (screenFader.isRunning || isForceSleeping)
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

        Collider2D[] wallColliders = Physics2D.OverlapAreaAll(wallRect.min, wallRect.max);

        foreach(Collider2D wallCollider in wallColliders)
        {
            if (wallCollider != null && currentStairs == null)
            {
                walking = walking && wallCollider.isTrigger;
            }
        }

        Rect interactionRect = new Rect(transform.position - new Vector3(0.5f, 0.5f), Vector3.one);
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
                    if(dialogController.Show(interactableController.actions))
                    {
                        interactiveObject = interactionCollider.gameObject;
                        break;
                    }
                }
            }

            interactiveObject = null;
            interacting = false;
        }

        if(interactiveObject == null)
        {
            OnLeavingAction();
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

        if (walking)
        {
            mplayer.PlayFootsteps();
        }

        transform.position += walkingDirection * (walking ? 1 : 0) * walkingSpeed * Time.deltaTime;
        GetComponent<SpriteRenderer>().sortingOrder = (currentStairs != null) ? currentStairs.playerZOrder : (int)-transform.position.y - 1;
    }

    void OnLeavingAction()
    {
        Action action = dialogController.currentAction;
        dialogController.Hide();

        if(action == null)
        {
            return;
        }

        if(action.tag == Constants.Actions.OPENED_LETTER)
        {
            dialogController.background.rectTransform.sizeDelta -= new Vector2(200, 200);
            dialogController.dialogText.rectTransform.sizeDelta -= new Vector2(200, 200);
            
            Action letterAction = gameManager.GetActionWithTag(Constants.Actions.READ_LETTER);
            action.active = false;
            letterAction.active = true;
        }
    }

    IEnumerator CheckSleep()
    {
        if (screenFader.isRunning || isForceSleeping)
        {
            yield return null;
        }
        else
        {
            int hour = clock.GetHours((long) clock.currentGameTime);
            if(hour >= 3 && hour < 8 && !isForceSleeping)
            {
                isForceSleeping = true;

                yield return StartCoroutine(screenFader.FadeToColor(Constants.Colors.FADE, .5f));
                Action action = gameManager.GetActionWithTag(Constants.Actions.SLEEP_NIGHT);
                pointer.value += action.statModifier;
                clock.AddMinutes(action.duration);

                SavePlayerData();

                playerOrientation = PlayerOrientation.Down;
                walking = false;

                animator.SetBool("up", playerOrientation == PlayerOrientation.Up);
                animator.SetBool("down", playerOrientation == PlayerOrientation.Down);
                animator.SetBool("left", playerOrientation == PlayerOrientation.Left);
                animator.SetBool("right", playerOrientation == PlayerOrientation.Right);
                animator.SetBool("walking", walking);

                SceneManager.LoadScene(Constants.Levels.LEVEL_1);
                isForceSleeping = false;
            }
        }
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

    public void LoadPlayerData()
    {

        clock.use24hour = PlayerPrefs.GetInt(Constants.Prefs.USE_24_HOUR_CLOCK, Constants.Prefs.Defaults.USE_24_HOUR_CLOCK) == 1;
        clock.currentGameTime = PlayerPrefs.GetFloat(Constants.Prefs.GAME_TIME, Constants.Prefs.Defaults.GAME_TIME);
        pointer.value = PlayerPrefs.GetFloat(Constants.Prefs.PLAYER_STATUS, Constants.Prefs.Defaults.PLAYER_STATUS);

        if(!isForceSleeping)
        {
            Vector2 newPosition = Vector2.zero;
            newPosition.x = PlayerPrefs.GetFloat(Constants.Prefs.LAST_POSITION_X, Constants.Prefs.Defaults.LAST_POSITION_X);
            newPosition.y = PlayerPrefs.GetFloat(Constants.Prefs.LAST_POSITION_Y, Constants.Prefs.Defaults.LAST_POSITION_Y);
            transform.position = newPosition;

            playerOrientation = (PlayerOrientation) PlayerPrefs.GetInt(Constants.Prefs.LAST_ORIENTATION, Constants.Prefs.Defaults.LAST_ORIENTATION);
        }

        animator.SetBool("up", playerOrientation == PlayerOrientation.Up);
        animator.SetBool("down", playerOrientation == PlayerOrientation.Down);
        animator.SetBool("left", playerOrientation == PlayerOrientation.Left);
        animator.SetBool("right", playerOrientation == PlayerOrientation.Right);
        animator.SetBool("walking", false);
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetFloat(Constants.Prefs.GAME_TIME, clock.currentGameTime);
        PlayerPrefs.SetFloat(Constants.Prefs.PLAYER_STATUS, pointer.value);
        PlayerPrefs.SetString(Constants.Prefs.LAST_LEVEL, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(Constants.Prefs.LAST_POSITION_X, transform.position.x);
        PlayerPrefs.SetFloat(Constants.Prefs.LAST_POSITION_Y, transform.position.y);
        PlayerPrefs.SetInt(Constants.Prefs.LAST_ORIENTATION, (int)playerOrientation);
        PlayerPrefs.SetInt(Constants.Prefs.FORCE_SLEEPING, isForceSleeping ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            isPaused = true;
            eventSystem.SetActive(false);
            SceneManager.LoadSceneAsync(Constants.Levels.PAUSE_MENU, LoadSceneMode.Additive);
            Time.timeScale = 0;
        }
        else
        {
            isPaused = false;
            SceneManager.UnloadScene(Constants.Levels.PAUSE_MENU);
            eventSystem.SetActive(true);
            Time.timeScale = 1;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (PlayerPrefs.GetInt(Constants.Prefs.CHANGING_FLOOR, Constants.Prefs.Defaults.CHANGING_FLOOR) == 1)
            return;

        if (other.tag == "DownstairsTransition")
        {
            StartCoroutine(mplayer.PlayLoopedFootsteps());
            PlayerPrefs.SetInt(Constants.Prefs.CHANGING_FLOOR, 1);
            StartCoroutine(screenFader.FadeToScene(Constants.Levels.LEVEL_0));
        }
        else if (other.tag == "UpstairsTransition")
        {
            StartCoroutine(mplayer.PlayLoopedFootsteps());
            PlayerPrefs.SetInt(Constants.Prefs.CHANGING_FLOOR, 1);
            StartCoroutine(screenFader.FadeToScene(Constants.Levels.LEVEL_1));
        }
    }
}
