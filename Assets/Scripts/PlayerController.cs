﻿using System.Collections;
#if UNITY_EDITOR
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

    private Vector3 startingPosition;

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
    private DayCounterScript daysRemaining;

    private bool isForceSleeping;
    
    private bool lastActionInteractable;
    private string lastActionText;

    void Awake()
    {
        startingPosition = transform.position;

        daysRemaining = GameObject.Find("RemainingDays").GetComponent<DayCounterScript>();
        eventSystem = GameObject.Find("EventSystem");
        mplayer = GameObject.Find("PersistentDataObject").GetComponent<MusicPlayer>();
        clock = GameObject.Find("Clock").GetComponent<ClockManager>();
        pointer = GetComponent<SliderController>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        dialogController.gameObject.SetActive(true);
        playerOrientation = PlayerOrientation.Down;
        screenFader = GameObject.Find("Fader").GetComponent<ScreenFader>();

        isForceSleeping = (PlayerPrefs.GetInt(Constants.Prefs.FORCE_SLEEPING, Constants.Prefs.Defaults.FORCE_SLEEPING) == 1);

        bool hasSavedGame = (PlayerPrefs.GetFloat(Constants.Prefs.GAME_TIME, Constants.Prefs.Defaults.GAME_TIME) > 0);
        if(hasSavedGame)
        {
            LoadPlayerData();
        }

        bool hasChangedFloor = PlayerPrefs.GetInt(Constants.Prefs.CHANGING_FLOOR, Constants.Prefs.Defaults.CHANGING_FLOOR) == 1;
        if(isForceSleeping)
        {
            gameManager.Reset();
            daysRemaining.ShowRemainingDays();
            StartCoroutine(DisplayDailyMessage());
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

        UpdatePlayerAnimation(playerOrientation, false);
    }

    private void UpdatePlayerAnimation(PlayerOrientation playerOrientation, bool walking)
    {
        animator.SetBool("up", playerOrientation == PlayerOrientation.Up);
        animator.SetBool("down", playerOrientation == PlayerOrientation.Down);
        animator.SetBool("left", playerOrientation == PlayerOrientation.Left);
        animator.SetBool("right", playerOrientation == PlayerOrientation.Right);
        animator.SetBool("walking", walking);
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

        if (Input.GetButtonDown("Pause") && !screenFader.isRunning)
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
                if(gameManager.CanExecuteAction(action))
                {
                    if(action.tag == Constants.Actions.SLEEP_NIGHT)
                    {
                        yield return screenFader.FadeToColor(Constants.Colors.FADE, 0.25f);

                        interactiveObject = null;
                        dialogController.Hide();

                        transform.position = startingPosition;
                        playerOrientation = PlayerOrientation.Down;

                        UpdatePlayerAnimation(playerOrientation, false);
                        UpdateSpriteZOrder();
                        UpdateStats(action);

                        daysRemaining.ShowRemainingDays();
                        StartCoroutine(DisplayDailyMessage());
                    }
                    else
                    {
                        yield return screenFader.FadeToColor(Constants.Colors.FADE, 0.25f);
                        UpdateStats(action);
                        yield return screenFader.FadeToColor(Color.clear, 0.25f);
                    }
                }
                else
                {
                    interacting = false;
                    dialogController.currentAction = null;

                    lastActionInteractable = action.interactable;
                    lastActionText = action.text;
                    action.interactable = false;
                    action.text = Constants.Strings.WAIT_MESSAGE;
                }
            }
        }

        interacting = false;

        yield return null;
    }

    void UpdateStats(Action action)
    {
        pointer.value += action.statModifier;
        clock.AddMinutes((int) Random.Range(action.duration * .9f, action.duration * 1.1f));
        PlayerPrefs.SetFloat(action.tag, clock.currentGameTime);
    }

    IEnumerator DisplayDailyMessage()
    {
        Action sleepAction = gameManager.GetActionWithTag(Constants.Actions.SLEEP_NIGHT);
        sleepAction.active = false
            ;
        yield return new WaitForSeconds(3.5f);

        string message = "";
        if(clock.GetRemainingDays() == 0)
        {
            message = Constants.Strings.LAST_DAY_MESSAGE;
        }
        else
        {
            message = Constants.Strings.DAILY_MOTIVATIONS[Random.Range(0, Constants.Strings.DAILY_MOTIVATIONS.Length)];
        }

        Action dailyAction = gameManager.GetActionWithTag(Constants.Actions.DAILY_MESSAGE);

        dailyAction.active = true;
        dailyAction.text = message;
        dialogController.background.rectTransform.sizeDelta += new Vector2(50, 20);
        dialogController.dialogText.rectTransform.sizeDelta += new Vector2(50, 20);

        yield return new WaitForSeconds(2.5f);
        dailyAction.active = false;
        dailyAction.text = "";
        dialogController.background.rectTransform.sizeDelta -= new Vector2(50, 20);
        dialogController.dialogText.rectTransform.sizeDelta -= new Vector2(50, 20);

        sleepAction.active = true;
    }

    void UpdatePlayer()
    {
        if (screenFader.isRunning || daysRemaining.isRunning || isForceSleeping)
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

        if(interactiveObject != null)
        {
            for(int i = 0; i < interactionColliders.Length; i++)
            {
                Collider2D interactionCollider = interactionColliders[i];

                // Give highest priority to the same task from the previous frame
                if(interactiveObject == interactionCollider.gameObject)
                {
                    Collider2D temp = interactionColliders[0];
                    interactionColliders[0] = interactionCollider;
                    interactionColliders[i] = temp;
                    break;
                }
            }
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

        UpdateSpriteZOrder();

        UpdatePlayerAnimation(playerOrientation, walking);

        if (walking)
        {
            mplayer.PlayFootsteps();
        }

        transform.position += walkingDirection * (walking ? 1 : 0) * walkingSpeed * Time.deltaTime;
    }

    void UpdateSpriteZOrder()
    {
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
        else if(action.interactable == false)
        {
            action.interactable = lastActionInteractable;
            action.text = lastActionText;
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
            if(hour >= 3 && hour < 6 && !isForceSleeping)
            {
                isForceSleeping = true;

                yield return StartCoroutine(screenFader.FadeToColor(Constants.Colors.FADE, .5f));
                Action action = gameManager.GetActionWithTag(Constants.Actions.SLEEP_NIGHT);
                UpdateStats(action);

                SavePlayerData();

                playerOrientation = PlayerOrientation.Down;
                walking = false;
                
                UpdatePlayerAnimation(playerOrientation, walking);

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
        
        UpdatePlayerAnimation(playerOrientation, false);
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
