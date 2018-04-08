using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseOptions
{
  public void Quit()
  {
    Application.Quit();
  }
  public void MenuScene()
  {
    SceneManager.LoadScene("MenuScene");
  }
}