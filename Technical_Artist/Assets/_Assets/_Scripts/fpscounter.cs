using UnityEngine;

public class fpscounter : MonoBehaviour
{
    [SerializeField] private float updateInterval = 0.5f;
    [SerializeField] private Vector2 screenOffset = new Vector2(20f, 20f);
    [SerializeField] private int fontSize = 32;

    private float timer;
    private int frames;
    private float fps;
    private float ms;

    private void Update()
    {
        frames++;
        timer += Time.unscaledDeltaTime;

        if (timer >= updateInterval)
        {
            fps = frames / timer;
            ms = 1000f / Mathf.Max(fps, 0.0001f);

            frames = 0;
            timer = 0f;
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;

        if (fps >= 55f)
        {
            style.normal.textColor = Color.green;
        }
        else if (fps >= 30f)
        {
            style.normal.textColor = Color.yellow;
        }
        else
        {
            style.normal.textColor = Color.red;
        }

        GUI.Label(
            new Rect(screenOffset.x, screenOffset.y, 320f, 80f),
            "FPS : " + fps.ToString("F1") + "\nMS : " + ms.ToString("F1"),
            style
        );
    }
}