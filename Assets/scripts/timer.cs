using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    public float time;
    private Generate b;
    // Start is called before the first frame update
    void Start()
    {
        b = GameObject.Find("Board").GetComponent<Generate>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!b.done && b.activated) {
            time += Time.deltaTime;
            gameObject.GetComponent<Text>().text = (Mathf.FloorToInt(time).ToString());
        }
    }

    public void clockReset() {
        time = 0;
        gameObject.GetComponent<Text>().text = "00";
    }
}
