using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterCaveTrigger : MonoBehaviour
{
    [SerializeField]
    Image blackoutImage;

    float alpha = 0;

    private void Start()
    {
        blackoutImage.color = new Color(0, 0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(blackout());
    }

    IEnumerator blackout()
    {
        while (true)
        {
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0.05f);
            alpha += 0.05f;
            if(alpha >= 1)
            {
                break;
            }
        }
        yield return new WaitForSeconds(0.5f);
        UltimateGameManager.Instance.isLevel = true;
        SceneManager.LoadScene("Level");
    }
}
