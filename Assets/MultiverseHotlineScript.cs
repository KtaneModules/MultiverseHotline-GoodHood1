using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;

public class MultiverseHotlineScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;
    public KMSelectable Phone;
    public GameObject PhoneObj;
    public KMSelectable[] Buttonage;
    public MeshRenderer Galaxy;
    public AudioClip[] UniClips;
    public TextMesh[] DigitDisplays;
    public GameObject Underscores;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;
    private int mainVoice;
    private int secondVoice;
    private string[] voices = new string[] { "G", "P" };
    private int phoneCounter = 0;
    private bool phoneInteractable = true;
    private bool allowButtonPress = false;
    private bool strikeModule = false;
    private string originNumber;
    private int originUniverse;
    private int targetUniverse;
    private string modifiedNum;
    private string finalNum;
    private int inputStage = 0;
    private string inputtedDigits = "";

    private string[,] uniData = new string[,]
        {
            {"SACRAMENTO COUNTY GAZETTE", "256", "8", "213"},
            {"SACRED CHURCH OF GARY", "448", "5", "231"},
            {"SCHOOL OF CAT GYMNASTICS", "517", "8", "321"},
            {"SEATTLE COAST GUARD", "156", "9", "312"},
            {"SECRET CARPENTERS’ GUILD", "393", "9", "123"},
            {"SENATE OF CLASSY GENTLEMEN", "573", "10", "213"},
            {"SHANGHAI COLLEGE OF GEOLOGY", "285", "6", "123"},
            {"SILK CLOAKS & GARMENTS", "637", "8", "132"},
            {"SOCIETY OF CHEWING GUM", "428", "7", "312"},
            {"STAINED CRYSTAL GLASSMAKERS", "719", "10", "231"},
            {"STAMP COLLECTORS OF GEORGIA", "391", "6", "132"},
            {"STEEL CRATE GAMES", "241", "10", "123"},
            {"STUPID CHEAP GROCERIES", "962", "7", "321"},
            {"SUPER COMBUSTIBLE GADGETS", "627", "9", "132"},
            {"SUPREME COURT OF THE GEESE", "861", "5", "321"},
            {"SYNDICATE OF CRIMINAL GUYS", "184", "7", "312"}
        };

    private void Awake()
    {
        _moduleId = _moduleIdCounter++;

        foreach (KMSelectable button in Buttonage)
        {
            button.OnInteract += delegate () { InputPress(button); return false; };
        }
        Phone.OnInteract += delegate () { PhonePress(); return false; };
    }

    private void Start()
    {
        mainVoice = Rnd.Range(0, voices.Length);
        secondVoice = Rnd.Range(0, voices.Length);
        while (secondVoice == mainVoice)
            secondVoice = Rnd.Range(0, voices.Length);
        originUniverse = Rnd.Range(0, 16);
        GenerateOriginNumber();
        Debug.LogFormat("[The Multiverse Hotline #{0}] The caller's universe is {1} and their number is {2}.", _moduleId, uniData[originUniverse, 0], originNumber);
        FindTargetUniverse();
        modifiedNum = ConvertNumeralSystem(originNumber, originUniverse, targetUniverse);
        finalNum = "";
        for (int i = 0; i < 3; i++)
            finalNum += Rnd.Range(0, 10).ToString();
        Debug.LogFormat("[The Multiverse Hotline #{0}] {1} converted to {2}'s numeral system is {3} which is your modified number.", _moduleId, originNumber, uniData[targetUniverse, 0], modifiedNum);
        Debug.LogFormat("[The Multiverse Hotline #{0}] The final number to enter is {1}.", _moduleId, finalNum);


    }
    void FixedUpdate()
    {
        Galaxy.material.SetTextureOffset("_MainTex", new Vector2(Galaxy.material.GetTextureOffset("_MainTex").x - 0.0002f, Galaxy.material.GetTextureOffset("_MainTex").y + 0.0001f));
    }

    private void InputPress(KMSelectable btn)
    {
        btn.AddInteractionPunch(.5f);
        if (_moduleSolved) return;
        if (!allowButtonPress) return;
        Audio.PlaySoundAtTransform("beep" + Rnd.Range(1, 4), btn.transform);

        string pressedDigit = btn.GetComponentInChildren<TextMesh>().text;
        if (inputtedDigits.Length < 3)
        {
            DigitDisplays[inputtedDigits.Length].text = pressedDigit;
            inputtedDigits += pressedDigit;
        }
        if (inputtedDigits.Length == 3)
        {
            if (inputStage == 0)
            {
                phoneCounter = 1;
                allowButtonPress = false;
                if (inputtedDigits == uniData[targetUniverse, 1])
                {
                    inputStage++;
                    inputtedDigits = "";
                }
                else
                {
                    strikeModule = true;
                }
            }
            else if (inputStage == 1)
            {
                phoneCounter = 2;
                if (inputtedDigits == modifiedNum)
                {
                    inputStage++;
                    inputtedDigits = "";
                    StartCoroutine(ReadNumber(finalNum, secondVoice, true, true));
                }
                else
                {
                    Module.HandleStrike();
                    Debug.LogFormat("[The Multiverse Hotline #{0}]✕ You inputted {1} for the modified number when it expected {2}.", _moduleId, inputtedDigits, modifiedNum);
                    ResetModule();
                }
            }
            else if (inputStage == 2)
            {
                phoneCounter = 3;
                if (inputtedDigits == finalNum)
                {
                    Module.HandlePass();
                    _moduleSolved = true;
                    Audio.PlaySoundAtTransform("solve", Module.transform);
                }
                else
                {
                    Module.HandleStrike();
                    Debug.LogFormat("[The Multiverse Hotline #{0}]✕ You inputted {1} for the final number when it expected {2}.", _moduleId, inputtedDigits, finalNum);
                    ResetModule();
                }
            }
        }
        
    }

    private IEnumerator ConnectToTarget(bool strike)
    {
        yield return new WaitForSeconds(0.5f);
        Audio.PlaySoundAtTransform("phonering", PhoneObj.transform);
        allowButtonPress = false;
        yield return new WaitForSeconds(5f);
        if (strike)
        {
            Module.HandleStrike();
            Debug.LogFormat("[The Multiverse Hotline #{0}]✕ You inputted {1} for the univserse ID when it expected {2}.", _moduleId, inputtedDigits, uniData[targetUniverse, 1]);
            ResetModule();
        }
        else
        {
            allowButtonPress = true;
            Underscores.SetActive(true);
            foreach (TextMesh tm in DigitDisplays)
                tm.text = "";
            Audio.PlaySoundAtTransform("connected", PhoneObj.transform);
        }
    }

    private void ResetModule()
    {
        DropPhone();
        phoneInteractable = true;
        allowButtonPress = false;
        phoneCounter = 0;
        inputStage = 0;
        inputtedDigits = "";
        strikeModule = false;
        foreach (TextMesh tm in DigitDisplays)
            tm.text = "?";
        Start();
    }

    private void GenerateOriginNumber()
    {
        string final = "";
        for (int i = 0; i < 3; i++)
        {
            final += Rnd.Range(0, Int32.Parse(uniData[originUniverse, 2]));
        }
        originNumber = final;
    }

    private void FindTargetUniverse()
    {
        string[,] universeTable = new string[,]
        {
            {"STEEL CRATE GAMES", "STAINED CRYSTAL GLASSMAKERS", "SOCIETY OF CHEWING GUM", "SACRAMENTO COUNTY GAZETTE"},
            {"STUPID CHEAP GROCERIES", "SEATTLE COAST GUARD", "SILK CLOAKS & GARMENTS", "SHANGHAI COLLEGE OF GEOLOGY"},
            {"SUPREME COURT OF THE GEESE", "SECRET CARPENTERS’ GUILD", "SYNDICATE OF CRIMINAL GUYS", "SACRED CHURCH OF GARY"},
            {"SUPER COMBUSTIBLE GADGETS", "SCHOOL OF CAT GYMNASTICS", "STAMP COLLECTORS OF GEORGIA", "SENATE OF CLASSY GENTLEMEN"}
        };
        // A single line bugs if the 2D array is used and got no clue why
        string[] tableMadness = new string[]
        {
            "STEEL CRATE GAMES", "STAINED CRYSTAL GLASSMAKERS", "SOCIETY OF CHEWING GUM", "SACRAMENTO COUNTY GAZETTE",
            "STUPID CHEAP GROCERIES", "SEATTLE COAST GUARD", "SILK CLOAKS & GARMENTS", "SHANGHAI COLLEGE OF GEOLOGY",
            "SUPREME COURT OF THE GEESE", "SECRET CARPENTERS’ GUILD", "SYNDICATE OF CRIMINAL GUYS", "SACRED CHURCH OF GARY",
            "SUPER COMBUSTIBLE GADGETS", "SCHOOL OF CAT GYMNASTICS", "STAMP COLLECTORS OF GEORGIA", "SENATE OF CLASSY GENTLEMEN"
        };

        int[] origCoords = BaseHelper.FindIndexIn2D(universeTable, uniData[originUniverse, 0]);
        int curRow = origCoords[0];
        int curCol = origCoords[1];

        string[] digitDirs = new string[] { "U", "R", "D", ".", "L", "U", "R", "L", ".", "D" };
        string targetUniNum = ConvertNumeralSystem(originNumber, originUniverse, 11);
        Debug.LogFormat("[The Multiverse Hotline #{0}] {1} in our universe's numeral system is {2}.", _moduleId, originNumber, targetUniNum);
        List<string> directions = new List<string>();

        for (int i = 0; i < 3; i++)
        {
            if (digitDirs[targetUniNum[i] - '0'] == ".") continue;
            directions.Add(digitDirs[targetUniNum[i] - '0']);
        }

        string dirPrintLine = "";
        foreach (var dir in directions)
            dirPrintLine += dir + ", ";
        if (dirPrintLine.Length == 0)
        {
            dirPrintLine = "NONE, ";
        }
        Debug.LogFormat("[The Multiverse Hotline #{0}] This corresponds to the direction(s) {1}.", _moduleId, dirPrintLine.Remove(dirPrintLine.Length - 2, 2));

        List<int> visitedCells = new List<int>(); //Row * 4 + col
        visitedCells.Add(curRow * 4 + curCol);
        int curDirIndex = 0;

        int nonUniqueMoves = 0;
        while (nonUniqueMoves != 4)
        {
            switch (directions[curDirIndex])
            {
                case "R":
                    curCol++;
                    curCol %= 4;
                    break;
                case "D":
                    curRow++;
                    curRow %= 4;
                    break;
                case "L":
                    curCol--;
                    if (curCol < 0) curCol += 4;
                    break;
                case "U":
                    curRow--;
                    if (curRow < 0) curRow += 4;
                    break;
            }
            if (visitedCells.Contains(curRow * 4 + curCol))
            {
                nonUniqueMoves++;
                continue;
            }
            curDirIndex++;
            nonUniqueMoves = 0;
            visitedCells.Add(curRow * 4 + curCol);
            curDirIndex %= directions.Count();
        }

        string targetUniName = tableMadness[curRow * 4 + curCol];
        for (int i = 0; i < uniData.GetLength(0); i++)
            if (targetUniName == uniData[i, 0]) targetUniverse = i;
        Debug.LogFormat("[The Multiverse Hotline #{0}] After moving in the grid, the target universe is {1} and so the ID to dial is {2}.", _moduleId, targetUniName, uniData[targetUniverse, 1]);


    }

    private string ConvertNumeralSystem(string num, int fromUni, int toUni)
    {
        string fromDigitOrder = uniData[fromUni, 3];
        string newNum = "";
        for (int i = 1; i < 4; i++)
        {
            newNum += num[fromDigitOrder.IndexOf(i.ToString())];
        }
        int numBase10 = BaseHelper.BaseToDec(Int32.Parse(uniData[fromUni, 2]), Int32.Parse(newNum));
        int newUniNum = BaseHelper.DecToBase(Int32.Parse(uniData[toUni, 2]), numBase10);
        newUniNum %= 1000;

        string toDigitOrder = uniData[toUni, 3];
        newNum = Pad0sLeft(newUniNum.ToString());
        string finalNum = "";
        for (int i = 0; i < 3; i++)
        {
            finalNum += newNum[Int32.Parse(toDigitOrder[i].ToString()) - 1];
        }

        return finalNum;
    }

    private string Pad0sLeft(string input)
    {
        if (input.Length >= 3) return input;
        else if (input.Length == 2) return "0" + input;
        else if (input.Length == 1) return "00" + input;
        else return null;
    }

    private IEnumerator ReadNumber(string num, int voice, bool dropPhone, bool resetScreen)
    {
        allowButtonPress = false;
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < 3; i++)
        {
            string audioName = voices[voice] + "-" + num[i];
            Audio.PlaySoundAtTransform(audioName, Module.gameObject.transform);
            yield return new WaitForSeconds(1.1f);
        }
        if (dropPhone) DropPhone();
        if (resetScreen)
        {
            foreach (TextMesh tm in DigitDisplays)
                tm.text = "";
        }
        allowButtonPress = true;
    }
    private IEnumerator ReadUniverseAndNumber(int num, int voice, string nums)
    {
        allowButtonPress = false;
        yield return new WaitForSeconds(0.6f);
        string audioName = voices[voice] + "Uni-" + num;
        AudioClip clip;
        clip = UniClips[voice * 16 + num];
        Audio.PlaySoundAtTransform(audioName, Module.gameObject.transform);
        yield return new WaitForSeconds(clip.length);
        StartCoroutine(ReadNumber(nums, voice, true, false));
    }

    private void PhonePress()
    {
        if (!phoneInteractable) return;
        Phone.AddInteractionPunch(1.2f);
        if (PhoneObj.activeSelf)
        {
            Audio.PlaySoundAtTransform("phonepickup", Phone.transform);
            PhoneObj.SetActive(false);
        }
        if (phoneCounter == 0)
            StartCoroutine(ReadUniverseAndNumber(originUniverse, mainVoice, originNumber));
        else if (phoneCounter == 1)
            StartCoroutine(ConnectToTarget(strikeModule));
        else if (phoneCounter == 2)
            StartCoroutine(ReadNumber(finalNum, secondVoice, true, false));
        else
            StartCoroutine(PhoneBusy());
    }

    private IEnumerator PhoneBusy()
    {
        yield return new WaitForSeconds(0.5f);
        Audio.PlaySoundAtTransform("busy", transform);
        yield return new WaitForSeconds(4.9f);
        DropPhone();
    }

    private IEnumerator RingingAnimation()
    {
        for (int i = 0; i < 2; i++)
        {
            float elapsed = 0f;
            float duration = 2.0f;
            while(elapsed < duration)
            {
                var rotation = Quaternion.Euler(0f,(float)(Math.Sin(elapsed * 20)),0f);
                PhoneObj.transform.localRotation = rotation;
                yield return null;
                elapsed += Time.deltaTime;        
            }
            PhoneObj.transform.localRotation = Quaternion.Euler(0f,0f,0f);
            yield return new WaitForSeconds((i == 0) ? 2.0f : 0f);
        }
    }
    private void DropPhone()
    {
        if (PhoneObj.activeSelf == false)
        {
            Audio.PlaySoundAtTransform("phonedrop", Phone.transform);
            PhoneObj.SetActive(true);
            Phone.AddInteractionPunch(2f);
        }
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use '!{0} phone' to press the phone | "
                                                + "'!{0} press <buttons>' to press that numbered button;"
                                                + "chain button presses without using spaces; the input must be 3 digits long. "
                                                + "eg. '!{0} press 123'.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToUpper();

        if (command == "PHONE")
        {
            if (PhoneObj.activeSelf == true)
            {
                yield return null;
                Phone.OnInteract();
                yield break;
            } else
            {
                yield return "sendtochaterror You are not able to select the phone now!";
            }
            
        }

        string[] commands = command.Split(' ');

        if (!allowButtonPress) yield return "sendtochaterror You are not able to select the buttons now!";
        if (commands.Length != 2) yield return "sendtochaterror That is an invalid command!";
        if (commands[0] == "PRESS")
        {
            if (commands[1].Length != 3) yield return "sendtochaterror Invalid command!"; // Probably Mar typed this command
            if (!commands[1].All(Char.IsDigit)) yield return "sendtochaterror Invalid command!"; // Probably Mar typed this command
            yield return null;
            for (int i = 0; i < 3; i++)
            {
                int press = Int32.Parse(commands[1][i].ToString()) - 1;
                if (press < 0) press += 10;
                Buttonage[press].OnInteract();
            }
        }
        else
        {
            yield return "sendtochaterror Invalid command!"; // Probably Mar typed this command
        }


    }

    IEnumerator TwitchHandleForcedSolve()
    {
        if (inputStage == 0)
        {
            if (PhoneObj.activeSelf) Phone.OnInteract();
            while (!allowButtonPress) yield return null;
            for (int i = 0; i < 3; i++)
            {
                int press = Int32.Parse(uniData[targetUniverse, 1][i].ToString()) - 1;
                if (press < 0) press += 10;
                Buttonage[press].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
        if (inputStage <= 1)
        {
            if (PhoneObj.activeSelf) Phone.OnInteract();
            while (!allowButtonPress) yield return null;
            for (int i = 0; i < 3; i++)
            {
                int press = Int32.Parse(modifiedNum[i].ToString()) - 1;
                if (press < 0) press += 10;
                Buttonage[press].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
        while (!allowButtonPress) yield return null;
        for (int i = 0; i < 3; i++)
        {
            int press = Int32.Parse(finalNum[i].ToString()) - 1;
            if (press < 0) press += 10;
            Buttonage[press].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }


    }
}

