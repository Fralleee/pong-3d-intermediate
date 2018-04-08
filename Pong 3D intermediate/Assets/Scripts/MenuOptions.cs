using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour
{
  private GameOptions options;

  void Awake()
  {
    options = GameObject.Find("_GameOptions").GetComponent<GameOptions>();
  }

  public void SinglePlayer()
  {
    options.gameType = GameType.SINGLEPLAYER;
    PlayScene();
  }

  public void MultiPlayer()
  {
    options.gameType = GameType.MULTIPLAYER;
    PlayScene();
  }

  public void Quit()
  {
    Application.Quit();
  }

  void PlayScene()
  {
    SceneManager.LoadScene("PlayScene");
  }

  public void MenuScene()
  {
    SceneManager.LoadScene("MenuScene");
  }
}
