using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAdaptation : MonoBehaviour
{
    // Class for enemy movement adaptation

    // variables
    public int minChance, maxChance;
    int lightAttackHitCount, heavyAttackHitCount, playerLightAttackCount, playerHeavyAttackCount;

    // To reset the count
    public void ResetCount()
    {
        lightAttackHitCount = 0;
        heavyAttackHitCount = 0;
        playerLightAttackCount = 0;
        playerHeavyAttackCount = 0;
    }

    // Counting what the player did
    public void CountPlayerAttack(int attackSq)
    {
        if (attackSq < 4)
            playerLightAttackCount++;
        else
            playerHeavyAttackCount++;
    }

    // Counting when player got hit
    public void CountAttackHit(bool blockable)
    {
        if (blockable)
            lightAttackHitCount++;
        else
            heavyAttackHitCount++;
    }

    // Adjusting what attack to do
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

    // Adjusting what reaction to do
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
