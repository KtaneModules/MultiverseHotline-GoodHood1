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

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

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
    }
}
