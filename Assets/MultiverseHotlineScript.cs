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
    public GameObject Phoneage;
    public KMSelectable[] Buttonage;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;
    private bool wantRinging = true;
    private float elapsed = 0f;
    private float duration;

    private static string[] universes = new string[]
    {
        "Steel Crate Games", "Stained Crystal Glassmakers", "Society of Chewing Gum", "Sacremento County Gazette",
        "Stupid Cheap Groceries", "Seattle Coast Guard", "Silk Cloaks & Garments", "Shanghai College of Geology",
        "Supreme Court of the Geese", "Secret Carpenters' Guild", "Syndicate of Criminal Guys", "Sacred Church of Gary",
        "Super Combustible Gadgets", "School of Cat Gymnastics", "Stamp Collectors of Georgia", "Senate of Classy Gentlemen"
    };

    private void Start()
    {
        _moduleId = _moduleIdCounter++;

        foreach (KMSelectable button in Buttonage)
        {
            button.OnInteract += delegate () { /*InputPress(button);*/ StartCoroutine(RingingAnimation()); return false; };
        }

    }
    
    private IEnumerator RingingAnimation()
    {
        for (int i = 0; i < 2; i++)
        {
            elapsed = 0f;
            duration = 2.0f;
            while(elapsed < duration)
            {
                var rotation = Quaternion.Euler(0f,(float)(Math.Sin(elapsed * 20)),0f);
                Phoneage.transform.localRotation = rotation;
                yield return null;
                elapsed += Time.deltaTime;        
            }
            Phoneage.transform.localRotation = Quaternion.Euler(0f,0f,0f);
            yield return new WaitForSeconds((i == 0) ? 2.0f : 0f);
        }


    }

}
