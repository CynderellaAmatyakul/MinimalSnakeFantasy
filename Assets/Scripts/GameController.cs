using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject pauseUI;
    private HeroController player;

    private bool isPause = false;

    void Start()
    {
        player = GetComponent<HeroController>();
        gameOverUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPause)
            {
                isPause = true;
                Time.timeScale = 0f;
                pauseUI.SetActive(true);
            }
            else if (isPause)
            {
                isPause = false;
                Time.timeScale = 1f;
                pauseUI.SetActive(false);
            }
        }
    }

    public void TriggerGameOver()
    {
        player.enabled = false;

        gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainGame");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
