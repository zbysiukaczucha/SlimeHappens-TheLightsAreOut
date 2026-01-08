using System;
using System.Collections;
using System.Collections.Generic;
using Slimeborne;
using UnityEngine;
using UnityEngine.UIElements;

public class UltimateGameManager : MonoBehaviour
{
    public static UltimateGameManager instance;
    public static int defaultScore = 30;
    public int score = defaultScore;
    public bool enableWallBreak;

    public bool isLevel = false;

    public List<int> destroyedWallNumbers;
    public List<string> collectedPickUpItems;
    public Vector3 playerPosition = new Vector3(-0.792999625f,-0.29700008f,3.77599907f);
    public Quaternion playerRotation;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public static UltimateGameManager Instance
    {
        get
        {
            if (instance is null)
            {
                Debug.LogError("Ultimate Game Manager is NULL");
            }
            return instance;
        }
    }

    private void Start()
    {
        enableWallBreak = false;
    }



    public static IEnumerator RestartLevelCoroutine()
    {
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
