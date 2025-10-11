#nullable enable
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class FinishTrigger : MonoBehaviour
{
    [SerializeField] private string levelSelectSceneName="LevelSelector";

    private void OnTriggerEnter(Collider other){ if(other.CompareTag("Player") && !string.IsNullOrWhiteSpace(levelSelectSceneName)) SceneManager.LoadScene(levelSelectSceneName); }
    public void SetLevelSelectScene(string scene){ if(!string.IsNullOrWhiteSpace(scene)) levelSelectSceneName=scene; }
}
