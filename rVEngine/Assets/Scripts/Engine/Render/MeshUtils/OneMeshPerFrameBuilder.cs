using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneMeshPerFrameBuilder : MonoBehaviour
{
    private bool _isRunning = false;
    List<IEnumerator> _buildFunctionsList = new List<IEnumerator>();

    private void Update()
    {
        if (!_isRunning)
        {
            List<IEnumerator> cloneBuildList = CloneList(_buildFunctionsList);
            StartCoroutine(TaskBuildMeshes(cloneBuildList));
        }
    }

    private List<IEnumerator> CloneList(List<IEnumerator> originalList)
    {
        List<IEnumerator> cloneBuildList = new List<IEnumerator>();

        foreach (IEnumerator function in originalList)
        {
            cloneBuildList.Add(function);
        }

        return cloneBuildList;
    }

    private IEnumerator TaskBuildMeshes(List<IEnumerator> funcList)
    {
        foreach (IEnumerator task in funcList)
        {
            //task();
            yield return null;
        }
    }

}
