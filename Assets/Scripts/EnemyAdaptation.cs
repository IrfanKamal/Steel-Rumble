using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAdaptation : MonoBehaviour
{
    public int minChance, maxChance;
    int lightAttackHitCount, heavyAttackHitCount, playerLightAttackCount, playerHeavyAttackCount;

    EnemyMovement mainScript;

    // Start is called before the first frame update
    void Start()
    {
        mainScript = GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetCount()
    {
        lightAttackHitCount = 0;
        heavyAttackHitCount = 0;
        playerLightAttackCount = 0;
        playerHeavyAttackCount = 0;
    }

    public void CountPlayerAttack(int attackSq)
    {
        if (attackSq < 4)
            playerLightAttackCount++;
        else
            playerHeavyAttackCount++;
    }

    public void CountAttackHit(bool blockable)
    {
        if (blockable)
            lightAttackHitCount++;
        else
            heavyAttackHitCount++;
    }

    public int AdjustAttackType()
    {
        if (lightAttackHitCount > 0 || heavyAttackHitCount > 0)
        {
            int adjustedChance = Mathf.RoundToInt(lightAttackHitCount / (lightAttackHitCount + heavyAttackHitCount) * 100);
            if (adjustedChance < minChance)
            {
                adjustedChance = minChance;
            }
            if (adjustedChance > maxChance)
                adjustedChance = maxChance;
            return adjustedChance;
        }
        else
            return 50;
    }

    public int AdjustReactType()
    {
        if (playerHeavyAttackCount > 0 || playerLightAttackCount > 0)
        {
            int adjustedChance = Mathf.RoundToInt(playerHeavyAttackCount / (playerHeavyAttackCount + playerLightAttackCount) * 100);
            if (adjustedChance < minChance)
            {
                adjustedChance = minChance;
            }
            if (adjustedChance > maxChance)
                adjustedChance = maxChance;
            return adjustedChance;
        }
        else
            return 50;
    }
}
