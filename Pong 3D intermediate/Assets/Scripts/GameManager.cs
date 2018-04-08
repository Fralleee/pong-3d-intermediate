using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Tasks
/*
    Increase ball max speed with game time
    Check rules (if player has scored 10 or time is up)
    Handle UI
    Handle where to send ball in start of round
*/


public class GameManager : MonoBehaviour
{

  public enum StateType
  {
    NEWROUND, // Display score, await inputs
    FREEZE,   // Countdown from 5 (first time), nextState = LIVE
    LIVE,     // WHEN A ROUND IS LIVE, nextState: ROUNDEND
    GAMEEND   // WHEN GAME HAS ENDED, await input and change scene or replay
  }

  private Transform ball;

  [SerializeField] private GameObject uiBackground;
  [SerializeField] private GameObject countdownLabelText;
  [SerializeField] private GameObject countdownText;
  [SerializeField] private GameObject readyLabel;
  [SerializeField] private GameObject player1ReadyText;
  [SerializeField] private GameObject player2ReadyText;
  [SerializeField] private GameObject player1ScoreText;
  [SerializeField] private GameObject player2ScoreText;
  [SerializeField] private GameObject mainMenuButton;
  [SerializeField] private GameObject pad1;
  [SerializeField] private GameObject pad2;

  public static GameManager instance = null;
  private GameOptions options;
  private float timer = 180f;

  // Stuff that gets reset every new game
  [SerializeField] private StateType state;
  public int player1Score;
  public int player2Score;
  private int ballLevel; // this controls ball max speed
  private int roundNo = 1;
  private bool isPaused = false;
  private bool roundStarted = false;
  private bool player1Ready = false;
  private bool player2Ready = false;
  private int startFreezeTime = 5;
  private int freezeTime = 3;
  private float roundStartTime;

  void Awake()
  {
    if (instance == null) instance = this;
    else if (instance != this) Destroy(gameObject);
    //DontDestroyOnLoad(gameObject);
    ball = GameObject.FindGameObjectWithTag("Ball").transform;
    options = GameObject.Find("_GameOptions").GetComponent<GameOptions>();
    InitGame();
  }

  void Update()
  {
    timer -= Time.deltaTime;
    HandlePause();
    switch (state)
    {
      case StateType.NEWROUND:
        NewRoundState();
        break;
      case StateType.FREEZE:
        FreezeState();
        break;
      case StateType.LIVE:
        LiveState();
        break;
      case StateType.GAMEEND:
        GameEndState();
        break;
      default:
        Debug.Log("ERROR: Unknown game state: " + state);
        break;
    }
  }

  void InitGame()
  {
    uiBackground.SetActive(false);
    countdownLabelText.SetActive(false);
    countdownText.SetActive(false);
    player1Score = 0;
    state = StateType.NEWROUND;
    ballLevel = 1;
  }

  void HandlePause()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      if (isPaused)
      {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("PauseScene");
      }
      else
      {
        isPaused = true;
        Time.timeScale = 0;
        SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);
      }
    }
  }

  void NewRoundState()
  {
    readyLabel.SetActive(true);
    player1ReadyText.SetActive(true);
    if (options.gameType == GameType.SINGLEPLAYER) player1ReadyText.GetComponent<RectTransform>().localPosition = new Vector3(0, -40.5f, 0);
    if (options.gameType == GameType.MULTIPLAYER) player2ReadyText.SetActive(true);
    if (CheckIfPlayersReady())
    {
      readyLabel.SetActive(false);
      player1ReadyText.SetActive(false);
      player2ReadyText.SetActive(false);
      roundStartTime = Time.time + (roundNo == 1 ? startFreezeTime : freezeTime);
      roundStarted = false;
      state = StateType.FREEZE;
    }
  }

  void FreezeState()
  {
    if (Time.time > roundStartTime) state = StateType.LIVE;
    else
    {
      pad1.GetComponent<PadController>().enabled = false;
      if (options.gameType == GameType.MULTIPLAYER) pad2.GetComponent<PadController>().enabled = false;
      else pad2.GetComponent<AIController>().enabled = false;
      float secondsLeft = roundStartTime - Time.time;
      if (roundNo == 1 && secondsLeft > 3) countdownLabelText.SetActive(true);
      else countdownLabelText.SetActive(false);
      countdownText.SetActive(true);
      uiBackground.SetActive(true);
      string text = secondsLeft > 3 ? ((int)secondsLeft + 1).ToString() : secondsLeft > 2 ? "READY" : secondsLeft > 1 ? "SET" : "GO";
      countdownText.GetComponent<Text>().text = text;
    }
  }

  void LiveState()
  {
    if (!roundStarted)
    {
      uiBackground.SetActive(false);
      countdownLabelText.SetActive(false);
      countdownText.SetActive(false);
      roundStarted = true;
      float xPower = Random.Range(2, 4);
      float zPower = Random.Range(10, 20);
      zPower = roundNo % 2 == 0 ? zPower * -1 : zPower;
      Rigidbody body = ball.GetComponent<Rigidbody>();
      body.AddForce(new Vector3(Random.Range(xPower, xPower), 0, zPower));
    }
    else
    {
      pad1.GetComponent<PadController>().enabled = true;
      if (options.gameType == GameType.MULTIPLAYER) pad2.GetComponent<PadController>().enabled = true;
      else pad2.GetComponent<AIController>().enabled = true;
      if (ball.position.z < -30) Score(ref player2Score, ref player2ScoreText);
      else if (ball.position.z > 30) Score(ref player1Score, ref player1ScoreText);
    }
  }

  void GameEndState()
  {
    // Move this to a function
    pad1.GetComponent<PadController>().enabled = false;
    if (options.gameType == GameType.MULTIPLAYER) pad2.GetComponent<PadController>().enabled = false;
    else pad2.GetComponent<AIController>().enabled = false;
    countdownLabelText.SetActive(true);
    countdownText.SetActive(true);
    mainMenuButton.SetActive(true);
    string winner = player1Score > player2Score ? "PLAYER 1" : "PLAYER 2";
    countdownLabelText.GetComponent<Text>().text = "GAME OVER";
    countdownText.GetComponent<Text>().text = winner + " WON";
  }

  public void OnApplicationQuit()
  {
    instance = null;
  }

  bool CheckIfPlayersReady()
  {
    if (Input.GetButtonDown("Start_P1")) player1Ready = !player1Ready;
    if (options.gameType == GameType.MULTIPLAYER && Input.GetButtonDown("Start_P2")) player2Ready = !player2Ready;
    if (player1Ready) player1ReadyText.GetComponent<Text>().color = new Color(139f / 255f, 255f / 255f, 139f / 255f);
    if (player2Ready) player2ReadyText.GetComponent<Text>().color = new Color(139f / 255f, 255f / 255f, 139f / 255f);
    return options.gameType == GameType.MULTIPLAYER ? player1Ready && player2Ready : player1Ready;
  }

  public void Score(ref int score, ref GameObject scoreText)
  {
    // Add points to the winner
    roundNo += 1;
    score += 1;
    scoreText.GetComponent<Text>().text = score.ToString();

    // Reset stuff
    ball.transform.position = new Vector3(0, 0.5f, 0);
    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
    player1Ready = false;
    player2Ready = false;
    player1ReadyText.GetComponent<Text>().color = new Color(255f / 255f, 139f / 255f, 139f / 255f);
    player2ReadyText.GetComponent<Text>().color = new Color(255f / 255f, 139f / 255f, 139f / 255f);

    // Game ended or new round
    if (score == 10 || timer <= 0) state = StateType.GAMEEND;
    else state = StateType.NEWROUND;
  }

  public void MenuScene()
  {
    SceneManager.LoadScene("MenuScene");
  }
}
