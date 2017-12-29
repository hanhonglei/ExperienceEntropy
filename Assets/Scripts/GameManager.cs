using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public enum GameLevel { Intro, UserStudy, Inbetween };

public class GameManager : MonoBehaviour
{
    private string sceneName;           // current scene name
    public GameLevel gameLevel;         // current level type
    public GameObject answerUI;         // the answer UI when user study is done
    public GameObject allObjects;       // the objects' parent object including all potential perceptive objects
    public float levelTime = 18.0f;     // how long the user study scene will last

    private float currentTime = 0.0f;   // current time
    // Use this for initialization
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        sceneName = scene.name;

        // Get some particular parameters in particular levels [10/31/2016 Han]
        if (gameLevel == GameLevel.Intro
            || gameLevel == GameLevel.Inbetween)
            return;

        if (!answerUI)
        {
            answerUI = GameObject.FindGameObjectWithTag("AnswerUI");
            answerUI.SetActive(false);
        }
        if (!allObjects)
        {
            allObjects = GameObject.FindGameObjectWithTag("AllObjects");
        }
        Debug.Assert(answerUI && allObjects);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        switch (gameLevel)
        {
            case GameLevel.Intro:
            case GameLevel.Inbetween:
                if (Input.anyKeyDown)   // load next scene 
                {
                    PlayNextScene();
                    Debug.Log("Pressed, Load scene!" + SceneManager.sceneCountInBuildSettings);
                    Debug.Log(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            default:
                if (Input.GetKeyDown(KeyCode.C))    // capture screen image
                {
                    ScreenCapture.CaptureScreenshot("Output\\" + sceneName + "_GazeScreen_Frame_" + Time.frameCount + ".png", 2);
                }
                if (!answerUI.activeSelf)
                {
                    if (currentTime > levelTime || Input.GetKeyDown(KeyCode.Space))        // when user study scene is done, or user presses space bar
                    {
                        answerUI.SetActive(true);
                    }
                }
                else if (allObjects.activeSelf)
                {
                    // make sure answerUI is efficient after set active
                    answerUI.GetComponent<UserInputs>().BeginAnswer();

                    allObjects.SetActive(false);
                    GameObject p = GameObject.FindGameObjectWithTag("Player");
                    p.SetActive(false); // disable all perceptive objects
                    // make mouse cursor visible
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                break;
        }
    }

    public void PlayNextScene()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
