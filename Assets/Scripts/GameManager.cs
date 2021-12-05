using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject openingScene, endingScene;
    public TextMeshProUGUI endingText, enemyBtxt, enemyAtxt;
    public EnemyAdaptation enemyAdaptation;
    public EnemyMovement enemy;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        int enemyB = PlayerPrefs.GetInt("EnemyB", 50);
        int enemyA = PlayerPrefs.GetInt("EnemyA", 50);
        SetEnemyBehaviour(enemyB, enemyA);
        enemyBtxt.text = enemyB.ToString();
        enemyAtxt.text = enemyA.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        openingScene.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PlayerDie()
    {
        Time.timeScale = 0f;
        endingScene.SetActive(true);
        endingText.text = "You Lose!";
        AdjustBehaviour();
    }

    public void EnemyDie()
    {
        Time.timeScale = 0f;
        endingScene.SetActive(true);
        endingText.text = "You Win!";
        AdjustBehaviour();
    }

    void AdjustBehaviour()
    {
        PlayerPrefs.SetInt("EnemyB", enemyAdaptation.AdjustReactType());
        PlayerPrefs.SetInt("EnemyA", enemyAdaptation.AdjustAttackType());
    }

    public void SetEnemyBehaviour(int enemyB, int enemyA)
    {
        enemy.blockChance = enemyB;
        enemy.lightAttackChance = enemyA;
    }

    public void ResetEnemyBehaviour()
    {
        SetEnemyBehaviour(50, 50);
        enemyBtxt.text = "50";
        enemyAtxt.text = "50";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("steel-rumble");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
