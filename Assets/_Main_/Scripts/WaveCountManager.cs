using TMPro;
using UnityEngine;

public class WaveCountManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI waveCountText;

    private int currentWaveCount = 1;

    private void OnEnable()
    {
        EnemySpawnManager.OnNextLevel += OnNextLevelCallback;
    }

    private void OnDisable()
    {
        EnemySpawnManager.OnNextLevel -= OnNextLevelCallback;
    }

    private void OnNextLevelCallback()
    {
        currentWaveCount++;
        waveCountText.text = $"{currentWaveCount}";
    }

}
