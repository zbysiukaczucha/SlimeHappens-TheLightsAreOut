using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject currentGem;

    [SerializeField]
    public GemParticles gemParticles;

    [SerializeField]
    public PlayerCam playerCam;
    public CuttingCam cuttingCam;

    [SerializeField]
    public TimingMinigameUI timingMinigameUI;

    [SerializeField]
    public GameObject clientInteractionPanel;

    public int knobPoints = 0;
    public bool lockPlayer = false;

    private static GameManager _instance;
    public static GameManager Instance
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
        knobPoints = 0;
        clientInteractionPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (lockPlayer)
            {
                return;
            }
            //Debug.Log(Input.mousePosition);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    string name = hit.transform.name;
                    //Debug.Log("Hit " + name);

                    if (hit.transform.CompareTag("Client"))
                    {
                        print("Client says hi.");

                        if (clientInteractionPanel.gameObject.activeSelf)
                        {
                            return;
                        }

                        if (!lockPlayer)
                        {
                            LockPlayer();
                        }
                        clientInteractionPanel.gameObject.SetActive(true);
                        clientInteractionPanel.GetComponent<ClientInteraction>().InteractWithClient();
                    }

                    if (hit.transform.CompareTag("RunicTool"))
                    {
                        // Start timing minigame
                        timingMinigameUI.ShowTimingPanel();
                    }

                    if (hit.transform.CompareTag("Knob") || hit.transform.CompareTag("KnobPanel"))
                    {
                        if (!lockPlayer)
                        {
                            LockPlayer();
                        }
                    }

                    if (hit.transform.CompareTag("StonecuttingStation"))
                    {
                        if (!lockPlayer)
                        {
                            LockPlayer();
                            cuttingCam.GetComponent<Camera>().enabled = true;
                            playerCam.GetComponent<Camera>().enabled = false;
                        }
                    }
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (clientInteractionPanel.gameObject.activeSelf)
            {
                return;
            }
            UnlockPlayer();
            EnablePlayerCamera();
        }
    }

    void EnablePlayerCamera()
    {
        cuttingCam.GetComponent<Camera>().enabled = false;
        playerCam.GetComponent<Camera>().enabled = true;
    }

    public void LockPlayer()
    {
        lockPlayer = true;
        CursorManager.Instance.HideCrosshair();
        playerCam.ToggleLockCursor(true);
    }

    public void UnlockPlayer()
    {
        lockPlayer = false;
        CursorManager.Instance.ShowCrosshair();
        playerCam.ToggleLockCursor(false);
    }

}
