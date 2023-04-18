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

    public Slider slider;

    public Text textProgress;

    public Text tipText;

    private List<string> tips = new List<string> {"Don't die! As you only have one life.", "Save your progress by beating dungeons.",
      "Use potions to replenish health or stamina.",
      "Avoid obstacles by moving left or right, and up or down.",
      "Hope you're enjoying our game!", }; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            textProgress.text = $"Loading {Mathf.RoundToInt(progress * 100f)}%";

            yield return null;
        }

        loadingScreen.SetActive(false);

        SceneManager.UnloadSceneAsync("LoadingScreen");

    }
}
