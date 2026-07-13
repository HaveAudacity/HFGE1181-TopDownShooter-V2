using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;
    }
}