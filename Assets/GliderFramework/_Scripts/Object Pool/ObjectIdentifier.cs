using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add as component to GameObjects that you want ObjectPool to manage.
public class ObjectIdentifier : MonoBehaviour
{
    [HideInInspector] public int poolID;
    [HideInInspector] public string poolName;
}
