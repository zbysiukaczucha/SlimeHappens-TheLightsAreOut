using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateGameManager : MonoBehaviour
{
    public static UltimateGameManager instance;
    public static int defaultScore = 30;
    public int score = defaultScore;
    public bool enableWallBreak;

    public List<int> destroyedWallNumbers;

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
