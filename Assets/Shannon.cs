﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Shannon : MonoBehaviour
{
    List<float> allObjects; // record objects' perception information
    int frames;             // total frames of this scene
    StreamWriter output;    // the output file

    static float logtwo(float num)
    {
        return Mathf.Log(num) / Mathf.Log(2);
    }
    // calculate the entropy based on the equation -pi*log(pi)/log(2.0f);
    float RunEntropy(List<float> table)
    {
        float infoC = 0.0f;

        foreach (float freq in table)
        {
            infoC += freq * logtwo(freq);
            Debug.Log(freq);
        }
        infoC *= -1;
        Debug.Log("The Entropy of is  " + infoC);
        return infoC;
    }
    // Use this for initialization
    void Start()
    {
        allObjects = new List<float>();
        //RunEntropy("1234");
        frames = 0;
        output = File.CreateText("Output/Entropy_" + SceneManager.GetActiveScene().name + Random.value+".txt");
    }

    public StreamWriter GetOutput()
    {
        return output;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            float entropy = RunEntropy(allObjects);
            Debug.Log("Total frames: " + frames);
            output.Write(SceneManager.GetActiveScene().name 
                + "\tShannon entropy,Total frames\t");
            output.WriteLine(entropy + "\t" + frames);
            output.Close();
            Application.Quit();
        }
    }
    void FixedUpdate()
    {
        frames++;
    }

    public void TellPerception(GameObject g, float f)
    {
        allObjects.Add(f / frames);
    }
    void OnApplicationQuit()
    {
        Debug.Log("Application quit");
    }
    void OnDestroy()
    {
    }
    void OnApplicationFocus(bool hasFocus)
    {
    }
}