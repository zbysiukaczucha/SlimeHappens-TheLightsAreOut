using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClientInteraction : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerText;

    [SerializeField]
    TextMeshProUGUI clientText;

    [SerializeField]
    UnityAndGeminiV3 gemini;

    [SerializeField]
    Button responseButton1;
    [SerializeField]
    Button responseButton2;
    [SerializeField]
    Button responseButton3;

    TextMeshProUGUI responseButtonText1;
    TextMeshProUGUI responseButtonText2;
    TextMeshProUGUI responseButtonText3;

    public bool fakingGeminiEnabled = true;
    public bool getFakeResponse = true;

    int price = 50;
    float bargainedPercentage = 0.8f;
    bool nextLine = false;
    int replyNumber = 0;

    IEnumerator waitingTextAnimationCR;

    List<Character> predefinedCharacters = new List<Character>() {
        new Character("Goob", "elder man", Effect.PainReduction, 3, true),
    };

    Character currentCharacter;

    private void Awake()
    {
        responseButtonText1 = responseButton1.transform.GetComponentInChildren<TextMeshProUGUI>();
        responseButtonText2 = responseButton2.transform.GetComponentInChildren<TextMeshProUGUI>();
        responseButtonText3 = responseButton3.transform.GetComponentInChildren<TextMeshProUGUI>();
        ButtonsSetInteractable(false);
    }

    public void runNextLine(int number)
    {
        nextLine = true;
        replyNumber = number;
    }

    public void InteractWithClient()
    {
        //currentCharacter = GetRandom(predefinedCharacters);
        currentCharacter = new Character("Goob", "elder man", Effect.PainReduction, 3, true);
        StartCoroutine(clientInteractionCR());
    }

    // Client interaction singular loop
    IEnumerator clientInteractionCR()
    {
        SetSameReply("...");

        // Do the start of the interaction
        playerText.text = "Welcome in, how may I help you?";
        clientText.text = "Client says...";

        string botInstructions = $"You are a new client, a {currentCharacter.type} named {currentCharacter.name} visiting gem smith creating " +
            $"magical amulets in their workshop. You want the gem smith to create you an amulet that has " +
            $"{getStr(currentCharacter.wantedEffect)} as its magical property. ";

        // Wait for the connection attempt to end
        yield return SendChatCR(botInstructions);

        print("Connection attempt finished");

        // If couldn't connect to Gemini
        if (gemini.connectionFailure)
        {
            clientText.text = "<color=red><size=30>Gemini unavailable, running default dialog.</color></size>\n\n" +
                $"Hi! My name is {currentCharacter.name}, I'm a(n) {currentCharacter.type}.\n" +
                $"Could you craft me a gem that has an effect of {getStr(currentCharacter.wantedEffect)}?";
        }

        SetSameReply("Continue");

        // Wait for the player to press one of the given buttons
        yield return WaitForButtonPress();
        clientText.text = "Client says...";
        bool end = false;

        playerText.text = $"Sure thing! That will cost {price} coins.";

        // If client is likely to argue on the pricing, give Gemini the instruction to do so
        if (currentCharacter.bargainingTimes > 0)
        {
            currentCharacter.bargainingTimes--;

            // Send chat
            yield return SendChatCR($"Try to bargain the price down to {(bargainedPercentage * price).ToString()}.");

            if (gemini.connectionFailure)
            {
                clientText.text = $"I would like to pay {bargainedPercentage * price} instead.";
            }

            SetReplies(bargainReplies);
        }

        int counter = 0;

        while (currentCharacter.bargainingTimes > 0)
        {
            print("===================== NEW LOOP =====================");
            currentCharacter.bargainingTimes--;
            counter++;

            // Wait for the player to press one of the given buttons
            yield return WaitForButtonPress();
            clientText.text = "Client says...";


            // Depending on what the player chose
            switch (replyNumber)
            {
                case 0: // DISAGREEING
                    // If the character still has bargainingTimes > 0 after the -1 decrease
                    playerText.text = "I cannot agree on that price.";
                    if (currentCharacter.bargainingTimes > 0)
                    {
                        yield return SendChatCR("Insist on your proposition. ");

                        if (gemini.connectionFailure)
                        {
                            // Depending on which time it is that the character is insisting on lowering the price
                            clientText.text = counter == 1 ? "Please?" : "Pretty please?";
                        }
                    }
                    else
                    {
                        yield return SendChatCR("Agree on the given price. Finish the interaction.");

                        if (gemini.connectionFailure)
                        {
                            clientText.text = $"Okay, fine :<\n";
                        }
                        end = true;
                    }
                    break;

                case 1: // COMPROMISING
                    // Instruct Gemini according to the character's likelyToCompromise value
                    playerText.text = $"How about we meet halfway and set the price to {((bargainedPercentage * price + price) / 2)}?";

                    if (currentCharacter.likelyToCompromise)
                    {
                        yield return SendChatCR("Agree on the compromise. Finish the interaction.");

                        if (gemini.connectionFailure)
                        {
                            clientText.text = "Sure! :)\n Thank you!";
                        }
                        end = true;
                    }
                    else
                    {
                        yield return SendChatCR("Insist on your proposition. ");

                        if (gemini.connectionFailure)
                        {
                            clientText.text = "Pretty please?";
                        }
                    }
                    break;

                case 2: // AGREEING
                    // End interaction
                    playerText.text = "Alright then.";
                    yield return SendChatCR("Finish the interaction.");

                    if (gemini.connectionFailure)
                    {
                        clientText.text = "Yay!\nThank you!";
                    }
                    end = true;
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
        GameManager.Instance.UnlockPlayer();
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
    (string reply1, string reply2, string reply3) bargainReplies = ("Disagree", "Compromise", "Agree");

    // Do losowania czegoœ randomowego z listy
    T GetRandom<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    string getStr(Effect effect)
    {
        switch (effect)
        {
            case Effect.PainReduction:
                return "pain reduction";
            case Effect.Metamorphosis:
                return "metamorphosis";
            default:
                return "healing";
        }
    }

    IEnumerator WaitingTextAnimation()
    {
        while (true)
        {
            if (!gemini.connectionAttemptFin)
            {
                clientText.text = "Client says";
                for (int i = 0; i < 4; i++)
                {
                    yield return new WaitForSeconds(0.3f);
                    if (!gemini.connectionAttemptFin)
                    {
                        clientText.text += ".";
                    }
                }
            }
        }

    }

}
