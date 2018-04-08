using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameType
{
  SINGLEPLAYER,
  MULTIPLAYER
}

public class GameOptions : MonoBehaviour
{
  public static GameOptions instance = null;
  public GameType gameType;
  void Awake()
  {
    if (instance == null) instance = this;
    else if (instance != this) Destroy(gameObject);
    DontDestroyOnLoad(gameObject);
  }
}
