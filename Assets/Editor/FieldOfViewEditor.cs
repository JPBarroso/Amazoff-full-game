using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(RoombaController))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        RoombaController rc = (RoombaController)target;
        Handles.color = Color.green;
        Handles.DrawWireArc(rc.transform.position, Vector3.up, Vector3.forward, 360, rc.viewRadius);
        Handles.color = Color.blue;
        Handles.DrawWireArc(rc.transform.position, Vector3.up, Vector3.forward, 360, rc.viewRadius2);
        Handles.color = Color.red;
        Handles.DrawWireArc(rc.transform.position, Vector3.up, Vector3.forward, 360, rc.killDst);
        Vector3 viewAngleA = rc.DirFromAngle(-rc.viewAngle / 2, false);
        Vector3 viewAngleB = rc.DirFromAngle(rc.viewAngle / 2, false);
        Handles.DrawLine(rc.transform.position, rc.transform.position + viewAngleA * rc.viewRadius);
        Handles.DrawLine(rc.transform.position, rc.transform.position + viewAngleB * rc.viewRadius);
    }
}
