using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Appear : MonoBehaviour
{
    private int times;  // total frames this object can be seen
    public Camera cam;  // the main camera, the main script is assigned to this camera 
    public int cullDist = 100;  // the furthest clipping plane
    float centerness, blockness, area;  // 3 factors used to evalute the perception, details can find here https://github.com/hanhonglei/UserAttentionUserStudy
    float maxside, unprojectedsize; // used to calculate the factor area
    bool toldPerception = false;
    // Use this for initialization
    void Start()
    {
        times = 0;
        centerness = 0.0f;
        blockness = 0.0f;
        area = 0.0f;
        if (!cam)
            cam = Camera.main;
        maxside = (GetComponent<Collider>().bounds.extents.x > GetComponent<Collider>().bounds.extents.y) ? GetComponent<Collider>().bounds.extents.x : GetComponent<Collider>().bounds.extents.y;// -xcc 12.22
        maxside = (maxside > GetComponent<Collider>().bounds.extents.z) ? maxside : GetComponent<Collider>().bounds.extents.z;// -xcc 12.22
        unprojectedsize = Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView);
    }
    // to evaluate the perception, https://github.com/hanhonglei/UserAttentionUserStudy
    float Perception()
    {
        return (area + centerness + blockness) / 3.0f;
    }
    // Calculate the perception factors each frame, if the object cannot be seen the factors will not be calculated
    // 由于有些层次对象不存在renderer成员，故而使用几何方式判断是否在视见体内
    bool PerceptionByFrame()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (!GeometryUtility.TestPlanesAABB(planes, gameObject.GetComponent<Collider>().bounds))
            return false;
        // 后续是利用射线判断当前物体是否被其他物体遮挡
        Vector3 bound = GetComponent<Collider>().bounds.extents;
        var dist = Vector3.Distance(GetComponent<Collider>().bounds.center/*使用collider中心位置transform.position*/
            , cam.transform.position) - bound.y;
        if (dist > cullDist) // 如果超过最远距离，则不进行计算
            return false;

        // 自己进行遮挡率估算.将模型中心,8个角分别投射射线给摄像机,最后计算比例,记为遮挡率
        var ray = new Ray[9];
        var org = cam.transform.position;
        var targ = GetComponent<Collider>().bounds.center;
        // 中心
        ray[0] = new Ray(org, targ - org);
        Debug.DrawLine(org, targ);
        ray[1] = new Ray(org, targ + bound - org);
        Debug.DrawLine(org, targ + bound);
        bound.x = -bound.x;
        ray[2] = new Ray(org, targ + bound - org);
        Debug.DrawLine(org, targ + bound);
        bound.y = -bound.y;
        ray[3] = new Ray(org, targ + bound - org);
        Debug.DrawLine(org, targ + bound);
        bound.z = -bound.z;
        ray[4] = new Ray(org, targ + bound - org);
        Debug.DrawLine(org, targ + bound);
        bound.x = -bound.x;
        ray[5] = new Ray(org, targ + bound - org);
        Debug.DrawLine(org, targ + bound);
        bound.y = -bound.y;
        ray[6] = new Ray(org, targ + bound - org);
        Debug.DrawLine(org, targ + bound);
        bound.y = -bound.y;
        bound.z = -bound.z;
        ray[7] = new Ray(org, targ + bound - org);
        Debug.DrawLine(org, targ + bound);
        bound.x = -bound.x;
        bound.z = -bound.z;
        bound.y = -bound.y;
        ray[8] = new Ray(org, targ + bound - org);
        Debug.DrawLine(org, targ + bound);
        // ！！ 完全使用射线方式进行遮挡计算，需要进一步考虑如何实现！！
        var hitNum = 0;
        if (dist > 0)
        {
            for (var i = 0; i < 9; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(ray[i].origin, ray[i].direction, out hit, dist))
                    if (hit.collider.gameObject != gameObject)
                    {
                        //						Debug.DrawLine (org, hit.point);
                        hitNum++;
                    }
            }
        }
        if (hitNum == 9)
        {
            //Debug.Log(name + " blocked");                
            return false;
        }
        // 后续是按照“内隐测量”计算物体的关注度
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);

        // 和摄像机距离，依靠物体大小，计算投影面积
        //float distFactor = (1 - (screenPos.z - cam.nearClipPlane) / (cullDist - cam.nearClipPlane));
        //distFactor = distFactor < 0 ? 0 : distFactor;
        //distFactor = distFactor > 1 ? 1 : distFactor;
        float us = maxside / (unprojectedsize * screenPos.z);
        //us *= distFactor;
        us = us < 0 ? 0 : us;
        us = us > 1 ? 1 : us;
        area += us;
        //Debug.Log("Area: " + us);

        // 物体中心在屏幕空间的坐标,-0.5到0.5之间
        float x = screenPos.x < 0 ? 0 : screenPos.x;
        x = x > cam.pixelWidth ? cam.pixelWidth : x;
        float y = screenPos.y < 0 ? 0 : screenPos.y;
        y = y > cam.pixelHeight ? cam.pixelHeight : y;
        float xx = 0.5f - x / cam.pixelWidth;
        float yy = 0.5f - y / cam.pixelHeight;
        float disToCenter = Mathf.Sqrt(Mathf.Abs(xx) * Mathf.Abs(xx) + Mathf.Abs(yy) * Mathf.Abs(yy));
        disToCenter = disToCenter > 0.5f ? 0.5f : disToCenter;
        centerness += 1 - (disToCenter / 0.5f);

        blockness += 1 - hitNum / 9.0f;

        return true;
    }
    // output the info into the file
    public void OutputInfo()
    {
        if (times <= 0 || toldPerception)
            return;
        StreamWriter file = cam.gameObject.GetComponent<Shannon>().GetOutput();
        file.Write(name + transform.position
            + ":\tPerception,Area,Centerness,Blockness,Stayed frames,Interaction type and times\t");
        InteractiveItem ii = GetComponent<InteractiveItem>();
        int interactionTimes = ii == null ? 0 : ii.InteractionTimes();
        Type it = ii == null ? Type.Default : ii.itemType;
        file.WriteLine(Perception() + "\t" + area + "\t" + centerness + "\t" + blockness + "\t" + times 
            + "\t" + it + "\t" + interactionTimes);
        cam.gameObject.GetComponent<Shannon>().TellPerception(gameObject, Perception());
        toldPerception = true;
    }
    // Update is called once per frame
    void Update()
    {
        // if the esc key is pressed, then record all info then quit
        if (cam.gameObject.GetComponent<Shannon>().IsDone()
            && !cam.gameObject.GetComponent<Shannon>().IsCalcShannonDone())
        {
            OutputInfo();
            //Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        // calculate the perception factors
        if (PerceptionByFrame())
        {
            times++;
            //Debug.Log("In");
        }
        //else
        //Debug.Log("Out");
    }

    public int Times()
    {
        Debug.Log(name + " time is: " + times);
        return times;
    }
    // applicationQuit先被调用，OnDestroy函数后被调用
    void OnApplicationQuit()
    {
        OutputInfo();
    }
    void OnDestroy()
    {
        OutputInfo();
        Debug.Log(name + "destroy");
    }

}
