﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unimplemented
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
