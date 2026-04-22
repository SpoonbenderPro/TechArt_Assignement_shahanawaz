using UnityEngine;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("Canvas Root")]
    [SerializeField] private GameObject canvasroot;

    [Header("Panels")]
    [SerializeField] private GameObject startpanel;
    [SerializeField] private GameObject hudpanel;
    [SerializeField] private GameObject crashpanel;
    [SerializeField] private GameObject gameoverpanel;

    [Header("Game State")]
    [SerializeField] private bool pauseAtStart = true;

    private bool hasGameStarted;
    private bool isGameOver;

    public bool CanPlay => hasGameStarted && !isGameOver;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        EnsureCanvasActive();

        if (pauseAtStart)
        {
            ShowStartState();
        }
        else
        {
            StartGame();
        }
    }

    private void EnsureCanvasActive()
    {
        if (canvasroot != null)
        {
            canvasroot.SetActive(true);
        }
    }

    private void HideAllPanels()
    {
        if (startpanel != null) startpanel.SetActive(false);
        if (hudpanel != null) hudpanel.SetActive(false);
        if (crashpanel != null) crashpanel.SetActive(false);
        if (gameoverpanel != null) gameoverpanel.SetActive(false);
    }

    private void ShowStartState()
    {
        hasGameStarted = false;
        isGameOver = false;

        Time.timeScale = 0f;

        EnsureCanvasActive();
        HideAllPanels();

        if (startpanel != null)
        {
            startpanel.SetActive(true);
            startpanel.transform.SetAsLastSibling();
        }

        Canvas.ForceUpdateCanvases();
        UnityEngine.Debug.Log("ShowStartState");
    }

    public void StartGame()
    {
        hasGameStarted = true;
        isGameOver = false;

        Time.timeScale = 1f;

        EnsureCanvasActive();
        HideAllPanels();

        if (hudpanel != null)
        {
            hudpanel.SetActive(true);
            hudpanel.transform.SetAsLastSibling();
        }

        Canvas.ForceUpdateCanvases();
        UnityEngine.Debug.Log("StartGame");
    }

    public void ShowCrashFlash()
    {
        if (crashpanel != null)
        {
            crashpanel.SetActive(true);
            crashpanel.transform.SetAsLastSibling();
        }

        Canvas.ForceUpdateCanvases();
    }

    public void HideCrashFlash()
    {
        if (crashpanel != null)
        {
            crashpanel.SetActive(false);
        }

        Canvas.ForceUpdateCanvases();
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        EnsureCanvasActive();
        HideAllPanels();

        if (gameoverpanel != null)
        {
            gameoverpanel.SetActive(true);
            gameoverpanel.transform.SetAsLastSibling();
        }

        Canvas.ForceUpdateCanvases();
        Time.timeScale = 0f;

        UnityEngine.Debug.Log("GameOver UI shown");
        UnityEngine.Debug.Log(
            "Final UI State -> Start: " + GetPanelState(startpanel) +
            " | HUD: " + GetPanelState(hudpanel) +
            " | Crash: " + GetPanelState(crashpanel) +
            " | GameOver: " + GetPanelState(gameoverpanel)
        );
    }

    private string GetPanelState(GameObject panel)
    {
        if (panel == null)
        {
            return "NULL";
        }

        return panel.activeSelf ? "ON" : "OFF";
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}