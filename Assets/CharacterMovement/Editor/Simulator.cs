using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// <summary>
/// Simulates the sprite using a list of vector 3 coordinates
/// </summary>
public class Simulator : Editor
{
    private IEnumerator coroutine;
    private bool isRunning;

    private GameObject simulatedObj;
    private List<Vector3> path;

    //begins simulating
    public void SimulatePath(List<Vector3> _path, GameObject _simulatedobj)
    {
        if (isRunning)
        {
            CloseSimulation();
        }
        path = _path;
        simulatedObj = Instantiate(_simulatedobj);
        simulatedObj.name = "simulated object";
        DestroyImmediate(simulatedObj.GetComponent<MovementClass>());

        if (simulatedObj.GetComponent<SpriteRenderer>())
        {
            Color color = simulatedObj.GetComponent<SpriteRenderer>().color;
            color.a = 0.5f;
            simulatedObj.GetComponent<SpriteRenderer>().color = color;
        }

        EditorApplication.update += EditorUpdate;
        coroutine = SimulatingPath(path);
    }

    //this is the editor update call only called when the ienumerator is running
    private void EditorUpdate()
    {
        coroutine.MoveNext();
    }
    //the coroutine itself, note: it cant do a second delay.
    private IEnumerator SimulatingPath(List<Vector3> path)
    {
        isRunning = true;
        int index = 0;
        while (index < path.Count - 1)
        {
            index++;
            simulatedObj.transform.position = path[index];
            yield return new WaitForFixedUpdate();
        }
        CloseSimulation();
    }

    //closes the simulation, disabling the editorupdate and destroys the simulated object
    private void CloseSimulation()
    {
        DestroyImmediate(simulatedObj);
        EditorApplication.update -= EditorUpdate;
        isRunning = false;
    }

}
