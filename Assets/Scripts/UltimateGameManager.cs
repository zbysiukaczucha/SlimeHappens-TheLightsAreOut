using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateGameManager : MonoBehaviour
{
    static UltimateGameManager instance;
    public static int defaultScore = 30;
    public int score;
    public int dayCount = 1;
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
    
    
    
    public static IEnumerator RestartLevelCoroutine()
    {
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
