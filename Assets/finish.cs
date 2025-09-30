using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что столкновение произошло с игроком
        if (other.CompareTag("Player"))
        {
            Debug.Log("Победа!");
            // Здесь можно добавить действия при победе:
            // - Показать экран победы
            // - Перейти на следующий уровень
            // - Остановить управление игроком
        }
    }
}