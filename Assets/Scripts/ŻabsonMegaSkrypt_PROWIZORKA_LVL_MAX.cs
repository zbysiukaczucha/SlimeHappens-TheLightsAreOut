using System;
using UnityEngine;

public class ŻabsonMegaSkrypt_PROWIZORKA_LVL_MAX : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    //This is going to be REALLY ugly, but I really want to sleep at least a few hours. I'm terribly sorry, future me/zbychu :(

    public Animator animator;

    [Header("Attack Animations")] public String TongueSweep;
    public String TongueSlam;
    public String TongueSnap;
    public String RollR;
    public String Charge;
    public String ArmSmash;
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
