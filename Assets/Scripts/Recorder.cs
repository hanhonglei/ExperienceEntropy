using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class Recorder : MonoBehaviour
{
    /*
      How to use:
      Grab a 1st Person Controller to the scene
      * Set collider component to the object recorded
      * Give a meaningful name to this object
      * Grab this script to this object
      * Run the game, and let the player to navigate game scene freely
      * Quit the game, all recorded data files will be saved in the folder named Output
      */
    [SerializeField]
    protected Camera _mainCam;
 
    // Use this for initialization
    protected void Start()
    {
 
        if (_mainCam == null)       // get main camera
            _mainCam = Camera.main;

    } 
}
