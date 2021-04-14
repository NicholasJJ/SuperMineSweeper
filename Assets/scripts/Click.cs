using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    private bool done;
    // Update is called once per frame
    void Update()
    {
        done = gameObject.GetComponent<Generate>().done;
        if (Input.GetMouseButtonDown(0) && !done) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray,out RaycastHit hit)) {
                MineItem m = hit.collider.gameObject.GetComponent<MineItem>();
                if (m) {
                    m.dig();
                }
            }
        }
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) && !done)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                MineItem m = hit.collider.gameObject.GetComponent<MineItem>();
                if (m) {
                    m.flag();
                }
            }
        }
    }
}
