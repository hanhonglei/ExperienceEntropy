﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // required when using UI elements in scripts
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;
using System;// Required when using Event data.

public class UserInputs : MonoBehaviour
{
    private InputField myInputField;        // input field for users
    private StreamWriter sr;                // record users' answer into this file
    private List<Vector2> allMath;
    public Camera cam;

    // Use this for initialization
    void Start()
    {
        allMath = new List<Vector2>();
        if (!cam)
            cam = Camera.main;
    }
    void Awake()
    {
        myInputField = gameObject.GetComponentInChildren<InputField>();
        myInputField.Select();
        //Debug.Log(myInputField);
    }
    // call this function automatically when the user answer UI becomes enable
    void OnEnable()
    {
        myInputField.text = "Your answer";
        EventSystem.current.SetSelectedGameObject(myInputField.gameObject);
        myInputField.ActivateInputField();
    }
    // called by GameManager when the answer UI is enable, to record the math problems in this scene
    public void BeginAnswer()
    {
        if (!cam)
            cam = Camera.main;
        if(sr == null)
            sr = cam.gameObject.GetComponent<Shannon>().GetOutput();
        GameObject g = GameObject.FindGameObjectWithTag("MathObjects");
        if (!g)
        {
            return;
        }
        RecordPerception[] objs = g.GetComponentsInChildren<RecordPerception>();
        foreach (RecordPerception o in objs)
        {
            if (!o.GetComponentInChildren<Text>())
                continue;
            foreach (Vector2 v in o.MathProblems)
            {
                allMath.Add(v);
            }
        }
    }
    // record user's answer into the file
    // and change to the next scene
    public void InputDone(InputField input)
    {
        AnswerCorrect(input.text);

        //GameObject lm = GameObject.FindGameObjectsWithTag("GameController")[0];
        //GameManager g = lm.GetComponent<GameManager>();
        //g.PlayNextScene();
        Application.Quit();
    }
    // calculate user answer's correct percentage
    void AnswerCorrect(string answer)
    {
        string[] a = answer.Split();
        int c = 0;
        int n = allMath.Count;
        sr.Write("Participant's answer: ");
        foreach (string aa in a)
        {
            foreach (Vector2 v in allMath)
            {
                int aai = -1;
                Int32.TryParse(aa, out aai);
                if (aai == (int)(v.x + v.y))
                {
                    c++;
                    allMath.Remove(v);
                    break;
                }
            }
            sr.Write("\t" + aa);
        }
        sr.WriteLine();

        float f = (float)c * 100.0f / n;

        StreamWriter file = cam.gameObject.GetComponent<Shannon>().GetOutput();
        file.WriteLine("User answer accuracy is:\t" + f.ToString("F") + "%");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
