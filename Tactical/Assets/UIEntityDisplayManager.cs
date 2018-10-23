using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEntityDisplayManager : MonoBehaviour {
    private void Awake()
    {
        Global.myUIEntityDisplayManager = this;
    }
}
