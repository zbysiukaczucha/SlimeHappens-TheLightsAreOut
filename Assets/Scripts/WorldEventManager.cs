using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Slimeborne
{
    public class WorldEventManager : MonoBehaviour
    {
        public List<FogWall> fogWalls;
        public UIEnemyHealthBar bossHealthBar;

        public GameObject[] wallsToBeDestroyed;
        
        
        Image blackoutImage;
        float alpha = 0;
        public GameObject endTextUIGameObject;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            bossHealthBar = FindFirstObjectByType<UIEnemyHealthBar>();
            blackoutImage = GameObject.Find("BlackoutImage").GetComponent<Image>();
        }

        public void ActivateBossFight()
        {
            bossHealthBar.SetActive(true);
            
            foreach (FogWall fogWall in fogWalls)
            {
                fogWall.ActivateFogWall();
            }
        }
        
        public void BossDefeated()
        {
            bossHealthBar.SetActive(false);
            
            foreach (FogWall fogWall in fogWalls)
            {
                fogWall.DeactivateFogWall();
            }
        }
        
        public IEnumerator EndGame()
        {
            //Disable player controls
            FindFirstObjectByType<PlayerMovement>().enabled = false;
            FindFirstObjectByType<PlayerAttacker>().enabled = false;
            
            while (true)
            {
                blackoutImage.color = new Color(0, 0, 0, alpha);
                yield return new WaitForSeconds(0.05f);
                alpha += 0.05f;
                if(alpha >= 1)
                {
                    blackoutImage.color = new Color(0, 0, 0, alpha);
                    break;
                }
            }

            // Show end text UI and move it upwards
            endTextUIGameObject.SetActive(true);
            while (true)
            {
                endTextUIGameObject.transform.position += new Vector3(0, 5f, 0) * (Time.deltaTime * 20f);
                yield return null;
                if (endTextUIGameObject.transform.position.y >= Screen.height + endTextUIGameObject.GetComponent<RectTransform>().rect.height)
                {
                    // Go to main menu
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
                    break;
                }
            }
            
        }
    }
}