using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RarityLevel
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Loot", menuName = "ChronoCurse/Loot")]
public class Loot : ScriptableObject
{
    [Header("Loot Details:")]
    [SerializeField] private string lootName;
    [SerializeField] private Sprite sprite;

    [Space, Header("Loot Amount: ")]
    [SerializeField] public int minAmount;
    [SerializeField] public int maxAmount;

    [Space, Header("Loot Rarity: ")]
    [SerializeField] private RarityLevel rarityLevel;

    [Space, Header("Loot Gold Amount: ")]
    [SerializeField] public int minGoldAmount;
    [SerializeField] public int maxGoldAmount;

    [Space, Header("Health Amount: ")]
    [SerializeField] public int minHealthAmount;
    [SerializeField] public int maxHealthAmount;

    public Sprite Sprite => sprite;

    /// <summary>
    /// Gets the chance for a loot item of the given rarity to drop.
    /// </summary>
    /// <returns>The chance for the loot item to drop returns a boolean value if our loot can drop.</returns>
    public bool Drop()
    {
        int rarityChance = 0;

        switch (rarityLevel)
        {
            case RarityLevel.Common:
                rarityChance = 80;
                break;
            case RarityLevel.Uncommon:
                rarityChance = 50;
                break;
            case RarityLevel.Rare:
                rarityChance = 20;
                break;
            case RarityLevel.Epic:
                rarityChance = 10;
                break;
            case RarityLevel.Legendary:
                rarityChance = 5;
                break;
            default:
                rarityChance = 0;
                break;
        }

        return Random.Range(0, 100) < rarityChance;
    }

    /// <summary>
    /// Get the amount we want to drop
    /// </summary>
    /// <returns></returns>
    public int GetAmount()
    {
        return Random.Range(minAmount, maxAmount + 1);
    }

    /// <summary>
    /// Generates a random gold amount within the range specified by minGoldAmount and maxGoldAmount.
    /// </summary>
    /// <returns>The generated gold amount.</returns>
    public int GenerateGoldAmount()
    {
        return Random.Range(minGoldAmount, maxGoldAmount + 1);
    }

    public int GenerateHealthAmount()
    {
        return Random.Range(minHealthAmount, maxHealthAmount + 1);
    }
}