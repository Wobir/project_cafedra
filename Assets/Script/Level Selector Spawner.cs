using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // обязательно

public class LevelSelect : MonoBehaviour
{
    public GameObject buttonPrefab; // префаб кнопки
    public Transform contentParent; // родитель для кнопок
    public string[] levels; // массив имён сцен

    void Start()
    {
        if (buttonPrefab == null || contentParent == null || levels == null || levels.Length == 0)
        {
            Debug.LogError("Проверьте настройки LevelSelect!");
            return;
        }

        foreach (string levelName in levels)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, contentParent);

            // Для TextMeshPro
            TMP_Text tmpText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
                tmpText.text = levelName;

            // Добавляем обработчик кнопки
            buttonObj.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelName));
        }
    }

    void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
