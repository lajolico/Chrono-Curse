using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public GameObject loadingScreen;

    public GameManager loadingDungeon;

    public Slider slider;

    public TextMeshProUGUI textProgress;

    public TextMeshProUGUI tipText;

    private List<string> tips = new List<string> {"Don't die! As you only have one life.", "Save your progress by beating dungeons.",
      "Use potions to replenish health or stamina.",
      "Avoid obstacles by moving left or right, and up or down.",
      "Hope you're enjoying our game!", }; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // LoadSceneAsync("Dungeon");
            //loadingDungeon.LoadDungeonScene(true);
        }else
        {
            Destroy(gameObject);
        }
    }
    public void LoadSceneAsync(string sceneName)
    {
        int randomIndex = Random.Range(0, tips.Count);

        string randomTip = tips[randomIndex];

        tipText.text = randomTip;

        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!operation.isDone)
        {
            Debug.Log("What are we doing???");
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            textProgress.text = $"Loading {Mathf.RoundToInt(progress * 100f)}%";

            yield return new WaitForSeconds(2);

            yield return null;
        }

        loadingScreen.SetActive(false);
    }
}
