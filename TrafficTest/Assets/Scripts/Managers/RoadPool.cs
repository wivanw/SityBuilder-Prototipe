﻿using UnityEngine;
using System.Collections;

public class RoadPool : MonoBehaviour
{
    public static RoadPool Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}