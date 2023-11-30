using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventDestroyOnLoad : MonoBehaviour
{
    static PreventDestroyOnLoad _singleton;

    private void Awake() 
    {
        if (!_singleton) 
        {
            _singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        if (_singleton)
        {
            if (_singleton.gameObject.GetInstanceID() != this.gameObject.GetInstanceID()) Destroy(gameObject);
        }
    }
}
