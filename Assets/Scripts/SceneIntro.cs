using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneIntro : MonoBehaviour
{
    GameObject allObjects = null;
    public float _introTime = 3.0f;
    private float _startTime = 0.0f;
    // Use this for initialization
    void Start()
    {
        _startTime = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        if (!allObjects)
        {
            allObjects = GameObject.FindGameObjectWithTag("AllObjects");
            Debug.Assert(allObjects);
            Canvas[] c = allObjects.GetComponentsInChildren<Canvas>();
            Text t = gameObject.GetComponentInChildren<Text>();
            RecordPerception r = c[0].GetComponentInParent<RecordPerception>();

            t.text = c.Length + " Objects\n" + r.mathNum + " Math each";
        }
        if ((Time.timeSinceLevelLoad - _startTime) > _introTime)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
