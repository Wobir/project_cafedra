using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TMP_Text tutorialText;
    private int step = 0;
    private bool bonusCollected = false;
    private bool secretOpened = false;
    private bool secretCrystalCollected = false;
    private bool reachedFinish = false;
    private float timer = 0f;

    void Start()
    {
        ShowStep();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        switch (step)
        {
            case 0:
                if (keyboard.wKey.isPressed || keyboard.aKey.isPressed || keyboard.sKey.isPressed || keyboard.dKey.isPressed)
                    NextStep();
                break;
            case 1:
                if (keyboard.digit1Key.wasPressedThisFrame || keyboard.digit2Key.wasPressedThisFrame || keyboard.digit3Key.wasPressedThisFrame)
                    NextStep();
                break;
            case 2:
                if (mouse.leftButton.wasPressedThisFrame)
                    NextStep();
                break;
            case 3:
                if (keyboard.qKey.wasPressedThisFrame || keyboard.eKey.wasPressedThisFrame)
                    NextStep();
                break;
            case 4:
                if (keyboard.escapeKey.wasPressedThisFrame)
                {
                    timer = 0f;
                    NextStep();
                }
                break;
            case 5:
                timer += Time.unscaledDeltaTime;
                if (timer >= 3f)
                    NextStep();
                break;
            case 6:
                if (bonusCollected)
                    NextStep();
                break;
            case 7:
                if (secretOpened)
                    NextStep();
                break;
            case 8:
                if (secretCrystalCollected)
                    NextStep();
                break;
            case 9:
                if (reachedFinish)
                    NextStep();
                break;
        }
    }

    void ShowStep()
    {
        switch (step)
        {
            case 0: tutorialText.text = "Используй WASD, чтобы наклонять лабиринт."; break;
            case 1: tutorialText.text = "Нажми 1 / 2 / 3, чтобы сменить цель камеры."; break;
            case 2: tutorialText.text = "Зажми ЛКМ и поворачивай камеру."; break;
            case 3: tutorialText.text = "Поверни камеру на 90° клавишами Q или E."; break;
            case 4: tutorialText.text = "Нажми ESC, чтобы открыть меню."; break;
            case 5: tutorialText.text = "В меню отображаются кристаллы и время прохождения."; break;
            case 6: tutorialText.text = "Собери бонус - Это такой кристалл."; break;
            case 7: tutorialText.text = "Находи на уровнях коробки с секретом."; break;
            case 8: tutorialText.text = "Внутри секрета находится ещё один кристалл."; break;
            case 9: tutorialText.text = "Теперь дойди до финиша, чтобы завершить уровень."; break;
            case 10: tutorialText.text = "На финише ты получаешь звёзды: 1 — за прохождение, 2 — за время, 3 — за все кристаллы."; break;
            case 11: tutorialText.text = "Обучение завершено, Удачи!"; break;
        }
    }

    void NextStep()
    {
        step++;
        timer = 0f;
        ShowStep();
    }

    public void OnBonusCollected() => bonusCollected = true;
    public void OnSecretOpened() => secretOpened = true;
    public void OnSecretCrystalCollected() => secretCrystalCollected = true;
    public void OnFinishReached() => reachedFinish = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bonus") || other.CompareTag("Secret"))
        {
            if (other.CompareTag("Bonus"))
                OnBonusCollected();
            else
                OnSecretCrystalCollected();

            
        }
        else if (other.CompareTag("Secret"))
        {
            OnSecretOpened();
        }
        else if (other.CompareTag("Finish"))
        {
            OnFinishReached();
        }
    }
}
