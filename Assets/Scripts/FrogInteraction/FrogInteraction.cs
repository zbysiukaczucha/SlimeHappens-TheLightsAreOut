using Slimeborne;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FrogInteraction : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerText;

    [SerializeField]
    TextMeshProUGUI enemyText;
    
    [SerializeField]
    Button responseButton1;
    [SerializeField]
    Button responseButton2;
    [SerializeField]
    Button responseButton3;

    TextMeshProUGUI responseButtonText1;
    TextMeshProUGUI responseButtonText2;
    TextMeshProUGUI responseButtonText3;

    UnityAndGeminiV3 gemini;
    BossAISensor frogScript;
    GameObject playerCharacter;
    PlayerAttacker playerAttacker;
    InputHandler inputHandler;
    CameraHandler camScript;

    public bool fakingGeminiEnabled = true;
    public bool getFakeResponse = true;

    bool nextLine = false;
    int replyNumber = 0;

    IEnumerator waitingTextAnimationCR;

    List<Character> predefinedCharacters = new List<Character>() {
        new Character("Goob", "angry", 3, true),
    };

    Character currentCharacter;

    private void Awake()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("Gemini");
        if (array[0].TryGetComponent(out UnityAndGeminiV3 geminiScript)) gemini = geminiScript;
        else gameObject.SetActive(false);
        playerCharacter = GameObject.Find("PlayerCharacter");
        playerAttacker = playerCharacter.GetComponent<PlayerAttacker>();
        inputHandler = playerCharacter.GetComponent<InputHandler>();
        responseButtonText1 = responseButton1.transform.GetComponentInChildren<TextMeshProUGUI>();
        responseButtonText2 = responseButton2.transform.GetComponentInChildren<TextMeshProUGUI>();
        responseButtonText3 = responseButton3.transform.GetComponentInChildren<TextMeshProUGUI>();
        ButtonsSetInteractable(false);

        GameObject[] enemyObj = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject obj in enemyObj)
        {
            if(obj.TryGetComponent(out BossAISensor script)) frogScript = script;
        }

        camScript = GameObject.Find("CameraHolder").GetComponent<CameraHandler>();
    }

    public void runNextLine(int number)
    {
        nextLine = true;
        replyNumber = number;
    }

    public void InteractWithClient()
    {
        currentCharacter = GetRandom(predefinedCharacters);
        //currentCharacter = new Character("Goob", "elder man", 3, true);
        StartCoroutine(clientInteractionCR());
    }

    // Client interaction singular loop
    IEnumerator clientInteractionCR()
    {
        inputHandler.LockPlayer(true);
        SetSameReply("...");

        // Do the start of the interaction
        playerText.text = "...?";
        enemyText.text = "Frog says...";

        /*string botInstructions = $"You are a new client, a {currentCharacter.type} named {currentCharacter.name} visiting gem smith creating " +
            $"magical amulets in their workshop. You want the gem smith to create you an amulet that has " +
            $"{getStr(currentCharacter.wantedEffect)} as its magical property. ";*/
        string botInstructions = $"You are a {currentCharacter.type} frog named {currentCharacter.name} guarding a precious gem.";

        // Wait for the connection attempt to end
        yield return SendChatCR(botInstructions);

        print("Connection attempt finished");

        // If couldn't connect to Gemini
        if (gemini.connectionFailure)
        {
            enemyText.text = "<color=red><size=30>Gemini unavailable, running default dialog.</color></size>\n\n" +
                $"I'm {currentCharacter.name}, I'm a(n) {currentCharacter.type} frog guarding the gem.\n" +
                $"You shall not pass!";
        }

        SetSameReply("Continue");

        // Wait for the player to press one of the given buttons
        yield return WaitForButtonPress();
        enemyText.text = "Frog says...";
        bool end = false;
        bool startFight = false;

        playerText.text = $"I really need this gem. Could you let me through?";

        // If client is likely to argue on the pricing, give Gemini the instruction to do so
        if (currentCharacter.bargainingTimes > 0)
        {
            currentCharacter.bargainingTimes--;

            // Send chat
            yield return SendChatCR($"Don't agree in a way according to your character ({currentCharacter.type}).");

            if (gemini.connectionFailure)
            {
                enemyText.text = $"No ({currentCharacter.type}ly).";
            }

            SetReplies(bargainReplies);
            if(currentCharacter.bargainingTimes == 0)
            {
                startFight = true;
            }
        }


        while (currentCharacter.bargainingTimes > 0)
        {
            print("===================== NEW LOOP =====================");
            currentCharacter.bargainingTimes--;

            // Wait for the player to press one of the given buttons
            yield return WaitForButtonPress();
            enemyText.text = "Frog says...";


            // Depending on what the player chose
            switch (replyNumber)
            {
                case 0: // BEING NICE
                        // If the character still has bargainingTimes > 0 after the -1 decrease
                    playerText.text = "Please let me go though.";
                    if (currentCharacter.bargainingTimes > 0)
                    {
                        yield return SendChatCR($"Disagree in a way according to your character ({currentCharacter.type}). ");

                        if (gemini.connectionFailure)
                        {
                            // Depending on which time it is that the character is insisting on lowering the price
                            enemyText.text =  $"No ({currentCharacter.type})";
                        }
                    }
                    else
                    {
                        yield return SendChatCR($"Agree on letting them through in your character ({currentCharacter.type}). " +
                            $"Finish the interaction.");

                        if (gemini.connectionFailure)
                        {
                            enemyText.text = $"Fine, you can go ({currentCharacter.type}ly)\n";
                        }
                        end = true;
                    }
                    break;

                case 1: // THREATENING
                    // Instruct Gemini according to the frog's easilyThreatened value
                    playerText.text = $"I am threatening you!";

                    if (currentCharacter.easilyThreatened)
                    {
                        yield return SendChatCR("You got scared. Let them through.");

                        if (gemini.connectionFailure)
                        {
                            enemyText.text = "Oh no! ;-;\n Okay, you can go...";
                        }
                        end = true;
                    }
                    else
                    {
                        yield return SendChatCR($"The threat is not getting to you. React accordingly to your character " +
                            $"({currentCharacter.type}). ");

                        if (gemini.connectionFailure)
                        {
                            enemyText.text = "That doesn't scare me. My answer is still no.";
                        }
                    }
                    break;

                case 2: // CHOOSING TO FIGHT
                        // End interaction
                    playerText.text = "Then I'll fight you!";
                    yield return SendChatCR($"Give short pre-attack reaction accordingly to your character ({currentCharacter.type}).");

                    if (gemini.connectionFailure)
                    {
                        enemyText.text = "BRING IT ON!";
                    }
                    end = true;
                    startFight = true;
                    break;
            }

            if (end)
            {
                break;
            }

        }

        SetSameReply("Finish");

        // Wait for the player to press one of the given buttons
        yield return WaitForButtonPress();

        // Switch off the client interaction panel
        gameObject.SetActive(false);
        //GameManager.Instance.UnlockPlayer();

        frogScript.ToggleFrogAttack(startFight);
        inputHandler.LockPlayer(false);
    }

    IEnumerator SendChatCR(string botInstructions)
    {
        gemini.botInstructions = botInstructions;
        waitingTextAnimationCR = WaitingTextAnimation();
        StartCoroutine(waitingTextAnimationCR);
        if (fakingGeminiEnabled)
        {
            gemini.SendFakeChat(getFakeResponse);
        }
        else
        {
            gemini.SendChat();
        }

        yield return new WaitUntil(() => gemini.connectionAttemptFin);
        StopCoroutine(waitingTextAnimationCR);
        gemini.connectionAttemptFin = false;
    }


    IEnumerator WaitForButtonPress()
    {
        ButtonsSetInteractable(true);
        yield return new WaitUntil(() => nextLine);
        nextLine = false;
        ButtonsSetInteractable(false);
    }

    void ButtonsSetInteractable(bool value)
    {
        responseButton1.interactable = value;
        responseButton2.interactable = value;
        responseButton3.interactable = value;
    }

    // Setting replies on response buttons
    void SetReplies((string reply1, string reply2, string reply3) replies)
    {
        responseButtonText1.text = replies.reply1;
        responseButtonText2.text = replies.reply2;
        responseButtonText3.text = replies.reply3;
    }

    // Setting replies on response buttons
    void SetSameReply(string reply)
    {
        responseButtonText1.text = reply;
        responseButtonText2.text = reply;
        responseButtonText3.text = reply;
    }
    (string reply1, string reply2, string reply3) bargainReplies = ("Ask nicely", "Threaten", "Fight");

    // Do losowania czegoś randomowego z listy
    T GetRandom<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }


    IEnumerator WaitingTextAnimation()
    {
        while (true)
        {
            if (!gemini.connectionAttemptFin)
            {
                enemyText.text = "Frog says";
                for (int i = 0; i < 4; i++)
                {
                    yield return new WaitForSeconds(0.3f);
                    if (!gemini.connectionAttemptFin)
                    {
                        enemyText.text += ".";
                    }
                }
            }
        }

    }


}
