using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Simulator : Editor
{

    private IEnumerator coroutine;
    private bool isRunning;
    private GameObject simulatedObj;
    private List<Vector3> path;
    public void SimulatePath(List<Vector3> _path, GameObject _simulatedobj)
    {
        if (isRunning)
        {
            closeSimulation();
        }
        path = _path;
        simulatedObj = Instantiate(_simulatedobj); //TODO change obj to obj,transform!
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
    private void EditorUpdate()
    {
        coroutine.MoveNext();
    }
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
        closeSimulation();
    }
    private void closeSimulation()
    {
        DestroyImmediate(simulatedObj);
        EditorApplication.update -= EditorUpdate;
        isRunning = false;
    }

}
