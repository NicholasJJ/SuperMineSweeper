using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    private bool rotating = false;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            rotating = !rotating;
        }
        if (rotating)
            transform.localEulerAngles += new Vector3(0, 0, Time.deltaTime * speed);
    }
}
