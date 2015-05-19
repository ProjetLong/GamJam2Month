using UnityEngine;
using System.Collections;

public class crosshair : MonoBehaviour
{
    public Vector2 crosshairCenter = new Vector2(0.5f, 0.6f);
    public float offseting = 2.0f;
    public Texture textureUp;
    public Texture textureDown;
    public Texture textureRight;
    public Texture textureLeft;
    public float accuracyLoss;
    public float xOffset;
    public float yOffset;

    private Vector3 position;
    private float xOffsetSpeed;
    private float yOffsetSpeed;
    private Color crosshairColor = Color.white;
    private float crosshairAlpha = 1.0f;

    //External Scripts.
    private health healthScript;

    void Start()
    {
        healthScript = transform.root.GetComponent<health>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        float health = 100.0f;
        if (healthScript != null)
        {
            health = healthScript.GetHealth();
        }
        if (health > 0)
        {
            xOffsetSpeed += Input.GetAxis("Mouse X") * Time.deltaTime * 0.2f;
            yOffsetSpeed += Input.GetAxis("Mouse Y") * Time.deltaTime * 0.2f;
        }
        xOffsetSpeed = Mathf.Lerp(xOffsetSpeed, 0, Time.deltaTime * 20.0f);
        yOffsetSpeed = Mathf.Lerp(yOffsetSpeed, 0, Time.deltaTime * 20.0f);
        xOffset += xOffsetSpeed;
        yOffset += yOffsetSpeed;
        xOffset = Mathf.Lerp(xOffset, 0, Time.deltaTime * Mathf.Pow(Mathf.Abs(xOffset), offseting) * offseting * 100);
        yOffset = Mathf.Lerp(yOffset, 0, Time.deltaTime * Mathf.Pow(Mathf.Abs(yOffset), offseting) * offseting * 100);
        position = new Vector3(crosshairCenter.x + xOffset, crosshairCenter.y + yOffset, 0);
        transform.position = position;
    }

    void OnGUI()
    {
        float health = 100.0f;
        if (healthScript != null)
        {
            health = healthScript.GetHealth();
        }
        float alphaWave = 0.1f;
        if (health > 0)
        {
            crosshairAlpha = Mathf.Sin(Time.time) * alphaWave + (1 - alphaWave);
        }
        else
        {
            crosshairAlpha = Mathf.Lerp(crosshairAlpha, 0, Time.deltaTime);
        }
        crosshairColor.a = crosshairAlpha;
        GUI.color = crosshairColor;
        float scale = Screen.width * 0.03f;
        float xPosition = Screen.width * crosshairCenter.x + xOffset * Screen.width - scale * 0.5f;
        float yPosition = Screen.height * (1 - crosshairCenter.y) - yOffset * Screen.height - scale * 0.5f;
        float screenAccuracyDisplay = (accuracyLoss * Screen.width) / 40;
        GUI.DrawTexture(new Rect(xPosition, yPosition + screenAccuracyDisplay, scale, scale), textureUp);
        GUI.DrawTexture(new Rect(xPosition, yPosition - screenAccuracyDisplay, scale, scale), textureDown);
        GUI.DrawTexture(new Rect(xPosition - screenAccuracyDisplay, yPosition, scale, scale), textureRight);
        GUI.DrawTexture(new Rect(xPosition + screenAccuracyDisplay, yPosition, scale, scale), textureLeft);
    }
}
