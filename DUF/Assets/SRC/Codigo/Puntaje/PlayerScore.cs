using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int currentScore { get; private set; } = 0;

    public void AddPoints(int points)
    {
        currentScore += points;
    }
}

