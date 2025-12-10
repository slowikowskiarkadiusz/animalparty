using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "A", menuName = "ScriptableObjects/Create Loadable Set", order = 1)]
public class LoadableSet : ScriptableObject
{
    public List<Object> Collection;
}
