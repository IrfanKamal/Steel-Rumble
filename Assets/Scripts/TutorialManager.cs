using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // The Game Manager for the demo

    // Variabels
    public MovementEnemy enemy;
    public MovementPlayer player;
    public Health playerHealth, enemyHealth;
    public BoxCollider enemyCollider;
    public EnemyAdaptation adaptation;
    public Transform playerInitPositon, enemyInitPosition;

    // Text for dialogues
    [TextArea]
    public List<string> openingDialogues;
    [TextArea]
    public List<string> resetDialogues;
    public GameObject dialogueGO;
    public TextMeshProUGUI dialogueTxt;

    public GameObject menuGO;

    bool started = false;
    bool enemyMove = false;


    private void Start()
    {
        ResetCharacter();
        Time.timeScale = 0f;
        player.deathDone.AddListener(CharacterDied);
        player.attack.AddListener(adaptation.CountPlayerAttack);
        player.getDamage.AddListener(adaptation.CountAttackHit);
        enemy.deathDone.AddListener(CharacterDied);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            menuGO.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.P) && !enemyMove)
        {
            EnemyStartMove();
        }
    }

    // Reset enemy and player to starting point
    void ResetCharacter()
    {
        enemyMove = false;
        player.StopMove();
        enemy.StopMove();
        player.transform.position = playerInitPositon.position;
        player.transform.rotation = playerInitPositon.rotation;
        enemy.transform.rotation = enemyInitPosition.rotation;
        enemy.transform.position = enemyInitPosition.position;
        playerHealth.currentHealth = playerHealth.health;
        enemyHealth.currentHealth = enemyHealth.health;
        enemyCollider.enabled = false;
    }

    // Set player to can move
    void PlayerStartMove()
    {
        player.StartMove();
    }

    // Set enemy to can move
    void EnemyStartMove()
    {
        enemyCollider.enabled = true;
        enemy.StartMove();
        enemyMove = true;
    }

    // Coroutine to handle the dialogues
    IEnumerator StartDialogues(List<string> dialogues)
    {
        dialogueGO.SetActive(true);
        for(int i = 0; i < dialogues.Count; i++)
        {
            dialogueTxt.text = dialogues[i];
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            yield return new WaitForFixedUpdate();
        }
        dialogueGO.SetActive(false);
        PlayerStartMove();
    }

    // To adjust enemy movement decision
    void AdjustEnemyMovement(int attackType, int reactType)
    {
        enemy.lightAttackChance = attackType;
        enemy.dodgeChance = reactType;
    }

    // Whenever a character died
    void CharacterDied(string charTag)
    {
        ResetCharacter();
        if (charTag == "Player")
        {
            PlayerStartMove();
        }
        else
        {
            int attackType = adaptation.AdjustAttackType();
            int reactType = adaptation.AdjustReactType();
            AdjustEnemyMovement(attackType, reactType);
            List<string> newDialogues = resetDialogues;
            if (attackType >= 50)
            {
                newDialogues[1] += " light attack";
            }
            else if (attackType < 50)
            {
                newDialogues[1] += " heavy attack";
            }
            if (reactType >= 50)
            {
                newDialogues[2] += " dodging";
            }
            else if (reactType < 50)
            {
                newDialogues[2] += " blocking";
            }
            adaptation.ResetCount();
            StartCoroutine(StartDialogues(newDialogues));
        }
    }

    // For Play button
    public void PlayGame()
    {
        Time.timeScale = 1f;
        menuGO.SetActive(false);
        if (!started)
        {
            StartCoroutine(StartDialogues(openingDialogues));
            started = true;
        }
    }

    // For quit button
    public void ExitGame()
    {
        Application.Quit();
    }
}
