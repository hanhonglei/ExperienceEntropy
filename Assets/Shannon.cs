using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Shannon : MonoBehaviour
{
    List<float> allObjects; // record objects' perception information
    int frames;             // total frames of this scene
    StreamWriter output;    // the output file

    public GameObject answerUI;         // the answer UI when user study is done
    public GameObject mathObjects;       // the objects' parent object including all potential perceptive objects
    bool done = false;

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
        string path;
#if UNITY_EDITOR
        path = "Output\\";
#else
         path = Application.dataPath + "\\Output\\";
#endif
        output = File.CreateText(path + "Entropy_" + SceneManager.GetActiveScene().name + Random.value + ".txt");

        if (!answerUI)
        {
            answerUI = GameObject.FindGameObjectWithTag("AnswerUI");
            answerUI.SetActive(false);
        }
        if (!mathObjects)
        {
            mathObjects = GameObject.FindGameObjectWithTag("MathObjects");
        }
    }

    public StreamWriter GetOutput()
    {
        return output;
    }
    void Update()
    {
        if (!answerUI.activeSelf)
        {
            if (done)        // when user study scene is done, or user presses space bar
            {
                answerUI.SetActive(true);
            }
        }
        else if (mathObjects.activeSelf)
        {
            // make sure answerUI is efficient after set active
            answerUI.GetComponent<UserInputs>().BeginAnswer();

            mathObjects.SetActive(false);
            GetComponent<Camera>().enabled = false; // disable all perceptive objects
                                // make mouse cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
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
            done = true;
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
        output.Close();
    }
    void OnDestroy()
    {
    }

}