using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    public Texture2D cursorBasicTexture;

    [SerializeField]
    public Texture2D cursorClickTexture;

    [SerializeField]
    GameObject crosshair;

    [SerializeField]
    Sprite crosshairBasic;

    [SerializeField]
    Sprite crosshairHover;

    Image crosshairImage;
    Vector2 cursorHotspot;

    private static CursorManager _instance;
    public static CursorManager Instance
    {
        get
        {
            if (_instance is null)
            {
                Debug.LogError("Game Manager is NULL");
            }
            return _instance;
        }
    }
    public void Awake()
    {
        _instance = this;
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        crosshairImage = crosshair.GetComponent<Image>();
        cursorHotspot = new Vector2(cursorBasicTexture.width/2, cursorBasicTexture.height/2);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(GameManager.Instance.lockPlayer)
            {
                Cursor.SetCursor(cursorClickTexture, cursorHotspot, CursorMode.Auto);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (GameManager.Instance.lockPlayer)
            {
                Cursor.SetCursor(cursorBasicTexture, cursorHotspot, CursorMode.Auto);
            }
        }
    }

    public void ChangeCrosshairHover()
    {
        crosshairImage.sprite = crosshairHover;
    }

    public void ChangeCrosshairBasic()
    {
        crosshairImage.sprite = crosshairBasic;
    }

    public void HideCrosshair()
    {
        crosshair.SetActive(false);
    }

    public void ShowCrosshair()
    {
        crosshair.SetActive(true);
    }

}
