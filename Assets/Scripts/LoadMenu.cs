using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadMenu : MonoBehaviour
{
    public static void LoadLevel() =>SceneManager.LoadScene("MainMenu");
}
