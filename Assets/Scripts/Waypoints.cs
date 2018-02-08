using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Waypoints : MonoBehaviour {
    StreamWriter output;    // the output file

    // Use this for initialization
    void Start () {
        string path;
#if UNITY_EDITOR
        path = "Output\\";
#else
         path = Application.dataPath + "\\Output\\";
#endif
        output = File.CreateText(path + "Waypoints_" + SceneManager.GetActiveScene().name + Random.value + ".txt");

        output.Write(SceneManager.GetActiveScene().name
        + ":\tRecord camera position and orientation\nposition in world space,orientation in world space\n");
    }
    void OnDestroy()
    {
        output.Close();
    }
    void FixedUpdate()
    {
        output.WriteLine(transform.position.x.ToString("F") + "\t" + transform.position.y.ToString("F") + "\t" + transform.position.z.ToString("F") + "\t"
            + transform.forward.x.ToString("F") + "\t" + transform.forward.y.ToString("F") + "\t" + transform.forward.z.ToString("F"));
    }
}
