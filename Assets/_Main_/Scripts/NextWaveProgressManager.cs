using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NextWaveProgressManager : MonoBehaviour
{
    [SerializeField] private GameObject folder;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Awake()
    {
        folder.SetActive(false);
    }

    private void OnEnable()
    {
        EnemySpawnManager.OnWaitNextWave += OnWaitNextWaveCallback;
    }

    private void OnDisable()
    {
        EnemySpawnManager.OnWaitNextWave -= OnWaitNextWaveCallback;
    }

    private void OnWaitNextWaveCallback(float seconds)
    {
        StartCoroutine(BeginProgressBar(seconds));
    }

    private IEnumerator BeginProgressBar(float seconds)
    {
        folder.SetActive(true);
        float t = seconds;
        while (t > 0)
        {
            t -= Time.deltaTime;

            float normalizedT = t / seconds;
            progressBar.value = normalizedT;
            countdownText.text = $"{(int)t}";

            yield return null;
        }

        UIManager.LogToScreen("Get ready! The enemy approaches!", 3);
        folder.SetActive(false);
    }

}
