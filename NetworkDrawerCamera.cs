using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDrawerCamera : MonoBehaviour
{
    NetworkDrawer networkDrawer;
    void Start()
    {
        networkDrawer = FindObjectOfType<NetworkDrawer>();
    }

    private void OnPostRender()
    {
        GL.PushMatrix();
        networkDrawer.DrawNetwork();
        GL.PopMatrix();
    }
}
