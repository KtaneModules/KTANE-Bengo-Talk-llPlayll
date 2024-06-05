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
    [SerializeField] private List<Material> BengoHLMaterials;
    [SerializeField] private TextMesh DisplayText;
    [SerializeField] private GameObject JongoJongo;
    [SerializeField] private AudioClip KuroStrike;
    [SerializeField] private AudioClip KuroSolve;
    [SerializeField] private GameObject CatObject;
    [SerializeField] private List<Material> CatMaterials;
    [SerializeField] private GameObject HiddenCatObject;
    [SerializeField] private List<Material> HiddenCatMaterials;

    string alpha = "0ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    List<string> bengoPeople = new List<string>() { "_Play_", "GoodHood", "Sierra", "Kuro" };
    List<string> SierraNames = new List<string>() { "Acer", "Blaise", "Camia", "Ciel", "Hazel", "Kit", "Mar", "Monika", "Piccolo", "Sage" };
    List<string> bengoPhrases = new List<string>()
    {
        "Excuse me,\ncan I get a\nclock?",
        "Simon Sings\n(an E flat)",
        "When the opps\ngive you a\nMEALY apple",
        "QUOTE I WANT\nTO BE REMOVED\nFROM YOUR\nSOUNDPAD\nENDQUOTE",
        "Oh hey, a vent!\nThat's not\ngoing to be\nproblematic.",
        "Falling, Feeling,\nSwimming,\nSwinging,\nSinging, Sinking,\nDying, Diving",
        "the grade has\nchanged",
        "Singing, Sinking,\nDying, Diving,\nLoving, Leaving,\nPulling, Pushing",
        "Ohhhhh,\nit's ten times",
        "Ohh, it's a\nmicrophone",
        "Oh hey, a vent!\nThat's not\ngoing to be\na problematic.",
        "Its actually very\nclampicated",
        "Ohhhh, is you\nman like bare\nTuesday?",
        "We should\nreally tell Nick\nabout that.\nNah, it's funny.",
        "Do you remember\nwhen Starmap\nReconstruction\nhad SIX stars?",
        "hollup...\nLet him cook",
        "It's as shrimple\nas that",
        "Monsplode\nTrading Cards,\ntrade Lanaluff.",
        "Duke, do you\nwant the ball?",
        "Oh the\nhamichok",
        "meow meow meowmeow\nmeow, meow meowmeow\nmeowmeow, meow meow\nmeowmeow, meow meow\nmeowmeow",
        "both of you are\nstupid, okay,\nright jongo jongo\nif you want to watch\nyou can have that",
        "Talba Classic,\nTalba LOD,\nTalba Classic,\nTalba LOD",
        "Look at the left\narrow, and then\nlook at FMN,\nthen press up.",
        "The Festive KTaNE festive\nthe KTaNE jukebox festive\nfestive jukebox KTaNE the\njukebox festive playing the\nfestive KTaNE jukebox music",
        "(I am the smallest definitely)",
        "Wear ever\nyou go their\nyour.",
        "YOU HAVE\nVIOLATED AN\nAREA PROTECTED\nBY... A SECURITY\nSYSTEM.",
        "Play make\nthe sound.",
        "Peanut or Done? Oh\nno, Done is the\nname of the dog. Oh\nno, I can actually\nname the dog!",
        "The classic\nwood stick\nstone.",
        "They made\nthis one\nimpossible!",
        "I foind fishe!\nI foind fishe!",
        "Ze blootooth dewice\nis cannected uhhh\nsuccssesfulleh",
        "Do you remember when you\nwanted to scream and then\nyou pressed the center\nbutton 3 times and then you\nstill wanted to scream?",
        "can you find cat",
        "I'm back at\nWhack a\nTotem.",
        "Dancing, walking,\nrearranging\nfurniture.",
        "You HAVE to know\nthis! Here's the\nmoves it starts with.\nThey don't know,\nthey just see this.",
        "one upon a time"
    };
    List<int> firstLetterPos = new List<int>() { 5, 19, 23, 17, 15, 6, 20, 19, 15, 15, 15, 9, 15, 23, 4, 8, 9, 13, 4, 15, 13, 2, 20, 12, 20, 9, 23, 25, 16, 16, 20, 20, 9, 26, 4, 3, 9, 4, 25, 15 };
    List<int> comfyTextSizes = new List<int>() { 120, 115, 90, 75, 90, 75, 100, 85, 110, 120, 90, 90, 100, 90, 75, 110, 95, 95, 100, 145, 54, 70, 95, 90, 50, 40, 120, 70, 140, 70, 125, 125, 115, 70, 50, 85, 130, 80, 70, 85 };

    int phraseIndex;
    string chosenPhrase, targetPhrase;
    List<string> bengoNames = new List<string>() { "_Play_", "GoodHood", "Sierra", "Kuro" };
    string targetWord;
    int targetPos;
    string sierraFronter;
    List<string> assignedLetters = new List<string>() { "", "", "", "" };
    int catIndex;

    int submissionIdx = 0;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    void Awake()
    {
        ModuleId = ModuleIdCounter++;
        foreach (KMSelectable bengo in BengoButtons) {
            bengo.OnInteract += delegate () { BengoPressed(bengo); return false; };
            bengo.OnHighlight += delegate () { BengoRenderers[BengoButtons.IndexOf(bengo)].material = BengoHLMaterials[bengoPeople.IndexOf(bengoNames[BengoButtons.IndexOf(bengo)])]; };
            bengo.OnHighlightEnded += delegate () { BengoRenderers[BengoButtons.IndexOf(bengo)].material = BengoMaterials[bengoPeople.IndexOf(bengoNames[BengoButtons.IndexOf(bengo)])]; };
        }
        HiddenCatObject.GetComponent<KMSelectable>().OnInteract += delegate () { CatFound(); return false; };
    }

    void CatFound()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, HiddenCatObject.gameObject.transform);
        HiddenCatObject.GetComponent<KMSelectable>().AddInteractionPunch();
        Log("You found cat, meow!");
        HiddenCatObject.SetActive(false);
    }

    void BengoPressed(KMSelectable person)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, person.gameObject.transform);
        person.AddInteractionPunch();
        if (ModuleSolved)
        {
            return;
        }
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
                StartCoroutine("SolveRoutine");
            }
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            if (Rnd.Range(1, 6) == 1)
            {
                Audio.PlaySoundAtTransform(KuroStrike.name, transform);
            }
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
        if (phraseIndex == 35)
        {
            catIndex = Rnd.Range(0, CatMaterials.Count);
            CatObject.gameObject.SetActive(true);
            CatObject.GetComponent<MeshRenderer>().material = CatMaterials[catIndex];
            HiddenCatObject.gameObject.SetActive(true);
            HiddenCatObject.GetComponent<MeshRenderer>().material = HiddenCatMaterials[catIndex];
            HiddenCatObject.gameObject.transform.localPosition += new Vector3(Rnd.Range(0, 0.1201f), 0, Rnd.Range(0, 0.1563f));
        }
        else
        {
            HiddenCatObject.gameObject.SetActive(false);
        }
        chosenPhrase = bengoPhrases[phraseIndex];
        DisplayText.text = chosenPhrase.Replace(" jongo jongo", "").Replace(" cat", "");
        DisplayText.fontSize = comfyTextSizes[phraseIndex];
        targetPhrase = bengoPhrases[phraseIndex + (phraseIndex % 2 == 0 ? 1 : -1)];
        Log($"The generated phrase is \"{chosenPhrase.Replace("\n", " ")}\".{(phraseIndex == 21 ? " (DLC Unlocked, each \"jongo\" is replaced by Juliett's pfp)" : "" )}");
        Log($"The target phrase is \"{targetPhrase.Replace("\n", " ")}\".");

        targetPos = firstLetterPos[phraseIndex] + Bomb.GetBatteryCount() + Bomb.GetPortCount() + 1;
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

    IEnumerator SolveRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (KMSelectable bengo in BengoButtons)
        {
            bengo.OnHighlightEnded();
        }
        if (Rnd.Range(1, 11) == 1)
        {
            yield return new WaitForSeconds(0.3f);
            Audio.PlaySoundAtTransform(KuroSolve.name, transform);
        }

    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} 1/2/3/4> to press the 1st/2nd/3rd/4th button top to bottom. Chain commands without spaces, e.g. <!{0} 1243224>.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        bool commandValid = true;
        for (int i = 0; i < Command.Length; i++)
        {
            if (!"1234".Contains(Command[i]))
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
                int btnIdx = "1234".IndexOf(Command[i]);
                BengoButtons[btnIdx].OnHighlight();
                BengoButtons[btnIdx].OnInteract();
                yield return new WaitForSeconds(0.2f);
                BengoButtons[btnIdx].OnHighlightEnded();
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
                    BengoButtons[j].OnHighlight();
                    BengoButtons[j].OnInteract();
                    yield return new WaitForSeconds(0.2f);
                    BengoButtons[j].OnHighlightEnded();
                    break;
                }
            }
            
        }
    }
}
