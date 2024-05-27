using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class BengoTalk : MonoBehaviour
{
    [SerializeField] private KMBombInfo Bomb;
    [SerializeField] private KMAudio Audio;

    [SerializeField] private List<KMSelectable> BengoButtons;
    [SerializeField] private List<MeshRenderer> BengoRenderers;
    [SerializeField] private List<Material> BengoMaterials;
    [SerializeField] private TextMesh DisplayText;
    [SerializeField] private GameObject JongoJongo;

    string alpha = "0ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    List<string> bengoPeople = new List<string>() { "_Play_", "GoodHood", "Sierra", "Kuro" };
    List<string> SierraNames = new List<string>() { "Acer", "Blaise", "Camia", "Ciel", "Hazel", "Kit", "Mar", "Monika", "Piccolo", "Sage" };
    List<string> bengoPhrases = new List<string>()
    {
        "Can I get a\nclock?",
        "Simon Sings (an\nE flat)",
        "When the opps\ngive you a MEALY\napple",
        "QUOTE I WANT TO\nBE REMOVED FROM\nYOUR SOUNDPAD\nENDQUOTE",
        "Oh hey, a vent!\nThat's not going\nto be\nproblematic.",
        "Falling, Feeling,\nSwimming,\nSwinging,\nSinging, Sinking,\nDying, Diving",
        "the grade has\nchanged",
        "Singing,\nSinking, Dying,\nDiving, Loving,\nLeaving, Pulling,\nPushing",
        "Ohhhhh, it's ten\ntimes",
        "Ohh, it's a\nmicrophone",
        "Oh hey, a vent!\nThat's not going\nto be a\nproblematic.",
        "Its actually\nvery clampicated",
        "Ohhhh, is you\nman like bare\nTuesday?",
        "We should really\ntell nick about\nthat. Nah, it's\nfunny.",
        "Do you remember\nwhen Starmap\nReconstruction\nhad SIX stars?",
        "hollup... Let\nhim cook",
        "It's as shrimple\nas that",
        "Monsplode\nTrading Cards,\ntrade Lanaluff.",
        "Duke, do you\nwant the ball?",
        "Oh the hamichok",
        "meow meow\nmeowmeow meow,\nmeow meowmeow\nmeowmeow, meow\nmeow meowmeow,\nmeow meow\nmeowmeow",
        "both of you are\nstupid, okay,\nright jongo jongo\nif you want to\nwatch you can\nhave that",
        "Talba Classic,\nTalba LOD,\nTalba Classic,\nTalba LOD",
        "Look at the left\narrow, and then\nlook at FMN,\nthen press up.",
        "The Festive KTaNE\nfestive the KTaNE\njukebox festive\nfestive jukebox\nKTaNE the jukebox\nfestive playing the\nfestive KTaNE\njukebox music",
        "(I am the\nsmallest\ndefinitely)"
    };

    int phraseIndex;
    string chosenPhrase, targetPhrase;
    List<string> bengoNames = new List<string>() { "_Play_", "GoodHood", "Sierra", "Kuro" };
    string targetWord;
    int targetPos;
    string sierraFronter;
    List<string> assignedLetters = new List<string>() { "", "", "", "" };

    int submissionIdx = 0;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    void Awake()
    {
        ModuleId = ModuleIdCounter++;
        foreach (KMSelectable bengo in BengoButtons) {
            bengo.OnInteract += delegate () { BengoPressed(bengo); return false; };
            }
    }

    void BengoPressed(KMSelectable person)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, person.gameObject.transform);
        if (ModuleSolved)
        {
            return;
        }
        person.AddInteractionPunch();
        bool submissionCorrect = assignedLetters[BengoButtons.IndexOf(person)].Contains(targetWord[submissionIdx].ToString().ToUpperInvariant());
        Log($"The {bengoNames[BengoButtons.IndexOf(person)]} button was used to submit the letter {targetWord[submissionIdx].ToString().ToUpperInvariant()}, {(submissionCorrect ? "correct." : "Strike!" )}");
        if (submissionCorrect)
        {
            submissionIdx++;
            if (submissionIdx == targetWord.Length)
            {
                Log("The target word was fully submitted, Module Solved!");
                GetComponent<KMBombModule>().HandlePass();
                ModuleSolved = true;
            }
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
        }
}

    void Start()
    {
        PhraseWordGeneration();
        BengoGeneration();
        LetterAssigning();
    }

    void PhraseWordGeneration()
    {
        phraseIndex = Rnd.Range(0, bengoPhrases.Count);
        JongoJongo.gameObject.SetActive(phraseIndex == 21);
        chosenPhrase = bengoPhrases[phraseIndex];
        DisplayText.text = chosenPhrase.Replace(" jongo jongo", "");
        DisplayText.fontSize = phraseIndex == 24 ? 250 : 300;
        targetPhrase = bengoPhrases[phraseIndex + (phraseIndex % 2 == 0 ? 1 : -1)];
        Log($"The generated phrase is \"{chosenPhrase.Replace("\n", " ")}\".{(phraseIndex == 21 ? " (DLC Unlocked, each \"jongo\" is replaced by Juliett's pfp)" : "" )}");
        Log($"The target phrase is \"{targetPhrase.Replace("\n", " ")}\".");

        targetPos = alpha.IndexOf(chosenPhrase[0].ToString().ToUpperInvariant()) + Bomb.GetBatteryCount() + Bomb.GetPortCount() + 1;
        string[] targetPhraseWords = targetPhrase.Replace("\n", " ").Split(' ');
        targetWord = RemoveSpecialChars(targetPhraseWords[(targetPos - 1) % targetPhraseWords.Length]);
        Log($"The target word is at position {targetPos} and it is \"{targetWord}\"");
    }

    void BengoGeneration()
    {
        bengoNames = bengoNames.Shuffle();
        string logNames = "";
        for (int i = 0; i < 4; i++)
        {
            BengoRenderers[i].material = BengoMaterials[bengoPeople.IndexOf(bengoNames[i])];
            logNames += bengoNames[i];
            if (i < 3)
            {
                logNames += ", ";
            }
        }
        Log($"The people in clockwise order starting from the top are: {logNames}.");
    }

    void LetterAssigning()
    {
        int sierraFronterPos = Bomb.GetIndicators().ToList().Count + Bomb.GetSerialNumberNumbers().Last() + 1;
        sierraFronter = SierraNames[(sierraFronterPos - 1) % 10];
        Log($"The Sierra's Fronter is at position {sierraFronterPos} and their name is {sierraFronter}");

        int currentIdx = bengoNames.IndexOf("Sierra");
        string currentLetter = sierraFronter[0].ToString();
        assignedLetters[currentIdx] += currentLetter;
        for (int i = 0; i < 25; i++)
        {
            currentIdx = (currentIdx + 1) % 4;
            currentLetter = alpha[(alpha.IndexOf(currentLetter)) % 26 + 1].ToString();
            assignedLetters[currentIdx] += currentLetter;
        }

        Log("People's assigned letters are:");
        for (int i = 0; i < 4; i++)
        {
            Log($"{bengoNames[i]}: {assignedLetters[i]}.");
        }
    }

    string RemoveSpecialChars(string s)
    {
        string specialChars = "',.?!()";
        for (int i = 0; i < specialChars.Length; i++)
        {
            s = s.Replace(specialChars[i].ToString(), null);
        }
        return s;
    }

    void Log(string arg)
    {
        Debug.Log($"[Bengo Talk #{ModuleId}] {arg}");
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} t/r/b/l> to press the top/right/bottom/left button. Chain commands without spaces, e.g. <!{0} trlbrrl>.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        bool commandValid = true;
        for (int i = 0; i < Command.Length; i++)
        {
            if (!"TRBL".Contains(Command.ToUpperInvariant()[i]))
            {
                commandValid = false;
                yield return "sendtochatmessage Invalid button names!";
                break;
            }
        }
        if (commandValid)
        {
            for (int i = 0; i < Command.Length; i++)
            {
                yield return null;
                BengoButtons["TRBL".IndexOf(Command.ToUpperInvariant()[i])].OnInteract();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = submissionIdx; i < targetWord.Length; i++)
        {
            yield return null;
            for (int j = 0; j < 4; j++)
            {
                if (assignedLetters[j].Contains(targetWord[i].ToString().ToUpperInvariant()))
                {
                    BengoButtons[j].OnInteract();
                    break;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
