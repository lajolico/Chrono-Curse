using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraFader : MonoBehaviour
{
    public static CameraFader Instance { get; private set; }
    [SerializeField] CinemachineStoryboard fader;
    public float fadeDuration = 1f;
    public GameObject playerUI;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance of the class already exists, destroy this one
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerUI.SetActive(false);
        fader.m_ShowImage= true;
        fader.m_Alpha = 1;
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        fader.m_Alpha = 1;
        fader.m_ShowImage = true;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fader.m_Alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerUI.SetActive(false);

    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fader.m_Alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerUI.SetActive(true);
        fader.m_ShowImage = false;
    }


}
