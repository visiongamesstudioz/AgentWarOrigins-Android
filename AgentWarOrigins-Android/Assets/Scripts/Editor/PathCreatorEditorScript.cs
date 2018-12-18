using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathCreator))]
public class PathCreatorEditorScript : Editor
{
    private int i = 0;
    private void OnSceneGUI()
    {
        var pathcreator = (PathCreator)target;
        var e = Event.current;

        if (e != null && e.type == EventType.MouseDown && e.button == 0)
        {
            
            var mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity,pathcreator.Mask))
            {
                //create empty game object at hitpoint position
                GameObject wayPoint = new GameObject("waypoint" + i);
                wayPoint.transform.position = hitInfo.point;
                wayPoint.transform.SetParent(pathcreator.transform);
                i++;
            }
        }
    }



}
