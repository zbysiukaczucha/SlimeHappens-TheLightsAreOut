using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ShineHappens
{
    public class MoveCamera : MonoBehaviour
    {
        public Transform camPos;


        void Update()
        {
            if (GameManager.Instance.lockPlayer)
            {
                return;
            }

            transform.position = camPos.position;
        }
    }
}