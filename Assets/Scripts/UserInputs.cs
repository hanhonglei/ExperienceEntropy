using System.Collections;
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
    //private List<Vector2> allMath;          // all math problems in the objects
    private List<int> allNumbers;           // all two digit numbers in the objects
    public Camera cam;
    // Use this for initialization
    void Start()
    {
        //allMath = new List<Vector2>();
        allNumbers = new List<int>();
        if (!cam)
            cam = Camera.main;
    }
    void Awake()
    {
        myInputField = gameObject.GetComponentInChildren<InputField>();
        myInputField.Select();
        //Debug.Log(myInputField);
        if (sr == null && cam != null)
            sr = cam.gameObject.GetComponent<Shannon>().GetOutput();

    }
    // call this function automatically when the user answer UI becomes enable
    void OnEnable()
    {
        myInputField.text = "Your answers here";
        EventSystem.current.SetSelectedGameObject(myInputField.gameObject);
        myInputField.ActivateInputField();
    }
    // called by GameManager when the answer UI is enable, to record the math problems in this scene
    public void BeginAnswer()
    {
        if (!cam)
            cam = Camera.main;
        if (sr == null)
            sr = cam.gameObject.GetComponent<Shannon>().GetOutput();
        GameObject g = GameObject.FindGameObjectWithTag("MathObjects");
        if (!g)
        {
            return;
        }
        RecordPerception[] objs = g.GetComponentsInChildren<RecordPerception>();
        //sr.Write("All math problems: ");
        sr.Write("All two digit numbers: ");
        foreach (RecordPerception o in objs)
        {
            if (!o.GetComponentInChildren<Text>())
                continue;
            // use two digit number instead of math problem
            sr.Write(o.name + "\t" + o.TheNumber + "\t");
            allNumbers.Add(o.TheNumber);

            //foreach (Vector2 v in o.MathProblems)
            //{
            //    sr.Write(o.name + v + "\t");
            //    allMath.Add(v);
            //}
        }
        sr.WriteLine();
    }
    // record user's answer into the file
    // and change to the next scene
    //public void InputDone(InputField input)
    //{
    //    AnswerCorrect(input.text);

    //    //GameObject lm = GameObject.FindGameObjectsWithTag("GameController")[0];
    //    //GameManager g = lm.GetComponent<GameManager>();
    //    //g.PlayNextScene();
    //    Application.Quit();
    //}
    // calculate user answer's correct percentage
    void AnswerCorrect(string answer)
    {
        string[] a = answer.Split();
        int c = 0;
        //int n = allMath.Count;
        int n = allNumbers.Count;
        sr.Write("Participant's answer: ");
        // use two digit number
        foreach (string aa in a)
        {
            foreach (int v in allNumbers)
            {
                int aai = -1;
                Int32.TryParse(aa, out aai);
                if (aai == v)
                {
                    c++;
                    allNumbers.Remove(v);
                    break;
                }
            }
            sr.Write("\t" + aa);
        }
        //foreach (string aa in a)
        //{
        //    foreach (Vector2 v in allMath)
        //    {
        //        int aai = -1;
        //        Int32.TryParse(aa, out aai);
        //        if (aai == (int)(v.x + v.y))
        //        {
        //            c++;
        //            allMath.Remove(v);
        //            break;
        //        }
        //    }
        //    sr.Write("\t" + aa);
        //}
        sr.WriteLine();

        float f = (float)c * 100.0f / n;

        StreamWriter file = cam.gameObject.GetComponent<Shannon>().GetOutput();
        file.WriteLine("User answer accuracy is:\t" + f.ToString("F") + "%");
    }
    // Update is called once per frame
    void Update()
    {
        // when the participant press enter key, then record all information and quit the program
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Debug.Log("Got it" + myInputField.text);
            AnswerCorrect(myInputField.text);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
        }
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(myInputField.gameObject);
        }
    }
}
