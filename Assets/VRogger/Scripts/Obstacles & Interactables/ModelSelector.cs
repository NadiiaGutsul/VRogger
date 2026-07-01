using System.Collections.Generic;
using UnityEngine;

public class ModelSelector : MonoBehaviour
{
    private List<GameObject> modelList;
    private int selectedModel;

    void Start()
    {
        // Choose a random model and activate it on startup
        modelList = new List<GameObject>();

        foreach (Transform child in transform)
        {
            modelList.Add(child.gameObject);
        }

        selectedModel = Random.Range(0, modelList.Count);

        modelList[selectedModel].SetActive(true);
    }
}
