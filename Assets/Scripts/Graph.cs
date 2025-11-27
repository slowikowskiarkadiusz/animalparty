using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    private readonly Dictionary<string, string[]> graph = new()
    {
        {"START", new []{"AA"}},
        {"AA", new []{"AB"}},
        {"AB", new []{"AC"}},
        {"AC", new []{"AD"}},
        {"AD", new []{"AE"}},
        {"AE", new []{"AF"}},
        {"AF", new []{"AG"}},
        {"AG", new []{"AH"}},
        {"AH", new []{"AI", "AN"}},
        {"AI", new []{"AJ"}},
        {"AJ", new []{"AK"}},
        {"AK", new []{"AL"}},
        {"AL", new []{"AM"}},
        {"AM", new []{"AN"}},
        {"AN", new []{"AO"}},
        {"AO", new []{"AP"}},
        {"AP", new []{"AR"}},
        {"AR", new []{"AS"}},
        {"AS", new []{"AT"}},
        {"AT", new []{"AU"}},
        {"AU", new []{"AW"}},
        {"AW", new []{"AX"}},
        {"AX", new []{"AY"}},
        {"AY", new []{"AZ"}},
        {"AZ", new []{"BA"}},
        {"BA", new []{"AA"}},
    };

    private readonly Dictionary<string, MeshCollider> nodeDictionary = new();

    [SerializeField] private Transform fields;

    private void Start()
    {
        for (int i = 0; i < fields.childCount; i++)
        {
            var child = fields.GetChild(i);
            nodeDictionary[child.name] = child.GetComponent<MeshCollider>();
        }
    }
}
