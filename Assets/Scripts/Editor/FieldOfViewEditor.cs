﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        if (fow.bw == null)
        {
            return;
        }
        foreach (Transform visibleTarget in fow.bw.visibleTargets)
        {
            if (visibleTarget == null)
            {
                //TODO: esto se tendría que mejorar, es un poco sucio
                continue;
            }
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
    }
}
