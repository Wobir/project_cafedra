using UnityEngine;
using TMPro;

public class ProgressDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI crystalsText;

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (GameSaveManager.Instance == null) return;

        int totalStars = GameSaveManager.Instance.data.totalStars;
        int totalBonuses = GameSaveManager.Instance.data.totalBonuses;

        if (starsText != null)
            starsText.text = $"{totalStars}";

        if (crystalsText != null)
            crystalsText.text = $"{totalBonuses}";
    }
}
