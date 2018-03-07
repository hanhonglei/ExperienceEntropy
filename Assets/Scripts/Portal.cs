using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : InteractiveItem
{
    private GameObject ui;
    private float savedTimeScale;

    // Use this for initialization
    void Start()
    {
        ui = transform.Find("Canvas").gameObject;
        Debug.Assert(ui);
        ui.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    // give the player options: next level, or stay in this level?
    override public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;


        ui.SetActive(true);

        //other.gameObject.SetActive(false);
        savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        base.OnTriggerEnter(other);
    }

    public void OnNextLevel()
    {
        //Camera.main.gameObject.GetComponent<Shannon>().Done();

        GameObject mathObjects = GameObject.FindGameObjectWithTag("MathObjects");
        Component[] Items = mathObjects.GetComponentsInChildren<Appear>();
        foreach (Appear appear in Items)
            appear.OutputInfo();

        Time.timeScale = savedTimeScale;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        //Debug.Log(next + "\t" + SceneManager.sceneCountInBuildSettings);
        if (next >= SceneManager.sceneCountInBuildSettings)
        {
            Application.Quit();
        }
        else
            SceneManager.LoadScene(next);
    }
    public void OnStayHere()
    {
        ui.SetActive(false);
        Time.timeScale = savedTimeScale;
    }
}
