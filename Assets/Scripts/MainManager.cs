using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;
    public TextMeshProUGUI GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private const string GAME_OVER_HIGH_SCORE = "GAME OVER\nHigh Score Achieved!\n\n<size=75%>Press SPACE to enter the Hall of Fame</size>";
    private const string GAME_OVER_RESTART = "GAME OVER\n\n<size=75%>Press SPACE to restart</size>";


    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        // display the highest score
        HighScoreText.text = "Best Score: " + DataManager.Instance.GetHighestScore();

        // disable the game over text
        GameOverText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // enough points achieved, 
                if (DataManager.Instance.QualifiesForHighScore(m_Points))
                {
                    // insert/ update a high score record
                    DataManager.Instance.InsertHighScore(m_Points);

                    // go to the high scores menu
                    SceneManager.LoadScene(1);
                }
                // restart the level
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }                
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;

        // show the appropriate game over text
        GameOverText.text = DataManager.Instance.QualifiesForHighScore(m_Points) ?
            GAME_OVER_HIGH_SCORE : GAME_OVER_RESTART;
        
        GameOverText.gameObject.SetActive(true);
    }
}
