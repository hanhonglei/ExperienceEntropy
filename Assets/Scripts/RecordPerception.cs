using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecordPerception : Recorder
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
    public int mathNum = 3;                 // how many math problems this object will appear
    public float changeMathTime = 3.0f;     // how often to change to the next math problem

    private List<Vector2> mathProblems;     // math problems list

    private float mathTime = 0.0f;          // indicate when to change to the next math problem 
    private int currentMath = 0;            // current math index in the math problems list
    private Component[] texts;              // save text components in the child objects of this object


    public List<Vector2> MathProblems
    {
        get { return mathProblems; }
    }
    // Use this for initialization
    void Start()
    {
        base.Start();

        // Generate question randomly [10/27/2016 Han]
        texts = gameObject.GetComponentsInChildren<Text>();
        GenerateMathProblems();
        DisplayCurrentMath();
    }
    // generate math problems randomly
    void GenerateMathProblems()
    {
        mathProblems = new List<Vector2>();
        for (int i = 0; i < mathNum; i++)
        {
            Vector2 n = new Vector2(Random.Range(0, 9), Random.Range(0, 9));
            mathProblems.Add(n);
        }
    }

    // if this object has text component in child object, it should appear math problem for the player to solve
    private void DisplayCurrentMath()
    {
        foreach (Text text in texts)
        {
            int i = (int)(mathProblems[currentMath].x);
            int j = (int)(mathProblems[currentMath].y);

            text.text = i + "+" + j + "=?";
        }
    }
    // Update is called once per frame
    void Update()
    {
        // time to change to the next math problem
        mathTime += Time.deltaTime;
        if (mathTime > changeMathTime)
        {
            mathTime = 0.0f;
            currentMath = (currentMath + 1) % mathProblems.Count;
            DisplayCurrentMath();
        }
    }
}