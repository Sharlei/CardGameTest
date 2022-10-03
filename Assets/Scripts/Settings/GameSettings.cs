using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Settings/Game Settings")]
public class GameSettings : ScriptableObject
{
    public int minRandomValue;
    public int maxRandomValue;
}
