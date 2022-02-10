using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int score = 0;
    public TextMeshProUGUI scoreLabel;
    public GameObject gameOverContainer;
    public TextMeshProUGUI endGameScoreLabel;
    private HashSet<string> _touchedGreenTriangles = new HashSet<string>();

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;
    }

    private void Update()
    {
        scoreLabel.text = $"Score: {score}";
    }

    public void TouchedTriangleWithId(string id)
    {
        if (_touchedGreenTriangles.Contains(id)) return;

        score += 1;
        _touchedGreenTriangles.Add(id);
    }

    public void EndGame()
    {
        Time.timeScale = 0;
        endGameScoreLabel.text = $"Your score is: {score}";
        gameOverContainer.SetActive(true);
    }

    public void OnPlayAgainPressed()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}