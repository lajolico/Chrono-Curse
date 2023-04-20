using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counters : MonoBehaviour
{
    private PlayerManager playerData;
    public GameObject data;

    public GameObject levelText;
    public GameObject xpText;
    public GameObject goldText;

    public TextMeshProUGUI levelRendered;
    public TextMeshProUGUI xpRendered;
    public TextMeshProUGUI goldRendered;

    private int level;
    private int xp;
    private int gold;

    private int levelTemp, xpTemp, goldTemp;

    private void Awake()
    {
        levelRendered = levelText.GetComponent<TextMeshProUGUI>();
        xpRendered = xpText.GetComponent<TextMeshProUGUI>();
        goldRendered = goldText.GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerData = data.GetComponent<PlayerManager>();
        // int gold = playerData.GetComponent<Gold>();
        level = playerData.Level;
        gold = playerData.Gold;
        
    }

    // Update is called once per frame
    void Update()
    {
        levelTemp = playerData.Level;
        goldTemp = playerData.Gold;
        Debug.Log(goldTemp);
        // if (levelTemp != level)
        // {
        //     levelRendered.text = ToString(levelTemp);
        // }

        // if (goldTemp != gold)
        // {
        //     goldRendered.text = ToString(gold);
        // }
    }
}
