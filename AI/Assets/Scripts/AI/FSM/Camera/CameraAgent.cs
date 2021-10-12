using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    StateManager sm = new StateManager();

    // Start is called before the first frame update
    void Start()
    {
        sm.ChangeState(new CameraSearchState(this));
    }

    // Update is called once per frame
    void Update()
    {
        sm.Update();
    }
}
