using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    public CharacterData.Stats actualStats;

    public CharacterData.Stats Stats {
        get { return actualStats; }
        set {
            actualStats = value;
        }
    }

    float health;

    #region Current Stat Properties
    public float CurrentHealth
    {
        get { return health; }

        // If we try and set the current health, the UI interface
        // on the pause screen will also be updated.
        set
        {
            // Check if the value has changed
            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }
    #endregion

    [Header("Visuals")]
    public ParticleSystem damageEffect; // If damage is dealt.
    public ParticleSystem blockedEffect; // If armor completely blocks damage

    // Experience and level of the player
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    // Class for defining a level range and the corresponding experience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    // I-Frames
    [Header("I-Frames")]
    public float invicibilityDuration; // Duration of invincibility after taking damage
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;

    PlayerCollector collector;
    PlayerInventory inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;

    PlayerAnimator playerAnimator;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        if (CharacterSelector.instance)
            CharacterSelector.instance.DestroySigleton();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        // Assign the variables
        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;

        playerAnimator = GetComponent<PlayerAnimator>();
        if (characterData.controller)
            playerAnimator.SetAnimatorController(characterData.controller);
    }

    void Start()
    {
        // Spawns the starting weapon
        inventory.Add(characterData.StartingWeapon);

        // Initialize the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        // If the invincibility timer has reached 0, set the invincibility flag to false
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
        collector.SetRadius(actualStats.magnet);
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();

        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        // Check if the player has enough experience to level up
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceIncrease = 0;

            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceIncrease;

            UpdateLevelText();

            GameManager.instance.StartLevelUp();

            if (experience >= experienceCap)
                LevelUpChecker();
        }
    }

    void UpdateExpBar()
    {
        // Update exp bar fill amount
        expBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        // Update level text
        levelText.text = "LV " + level.ToString();
    }

    public void TakeDamage(float dmg)
    {
        // If the player is not currently invincible, reduce health and start invincibility
        if (!isInvincible)
        {
            // Take armor into account before dealing the damage.
            dmg -= actualStats.armor;

            if (dmg > 0)
            {
                // Deal the damage.
                CurrentHealth -= dmg;

                // If there is a damage effect assigned, play it
                if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

                if (CurrentHealth <= 0)
                {
                    Kill();
                }

                UpdateHealthBar();
            }
            else
            {
                // If there is a blocked effect assigned, play it.
                if (blockedEffect) Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }

            invincibilityTimer = invicibilityDuration;
            isInvincible = true;
        }
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth; // Cap health to max
            }

            UpdateHealthBar();
        }
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth; // Cap health to max
            }

            UpdateHealthBar();
        }
    }
}
