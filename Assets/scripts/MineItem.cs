using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineItem : MonoBehaviour
{
    public Material dirt;
    public GameObject bombPng;
    public GameObject flagPng;
    public GameObject numberPng;

    [SerializeField] private GameObject[] neighbors;
    [SerializeField] public bool isBomb;
    public Vector2 loc;
    [SerializeField] private int bombNeighbors;

    [SerializeField] private bool dug = false;
    [SerializeField] private bool flagged = false;

    public void dig() {
        if (!GetComponentInParent<Generate>().activated) {
            GetComponentInParent<Generate>().activate(loc);
        }
        if (!dug && !flagged) {
            dug = true;
            gameObject.GetComponent<MeshRenderer>().material = dirt;
            if (isBomb) {
                bombPng.GetComponent<SpriteRenderer>().enabled = true;
                GetComponentInParent<Generate>().lose();
            } else {
                GetComponentInParent<Generate>().successfulDig();
                numberPng.GetComponent<MeshRenderer>().enabled = true;
                if (bombNeighbors == 0) {
                    foreach (GameObject n in neighbors) {
                        n.GetComponent<MineItem>().dig();
                    }
                }
            }
        }
    }

    public void flag() { //TODO: implement saftey clicking
        if (!dug) {
            flagged = !flagged;
            flagPng.GetComponent<SpriteRenderer>().enabled = flagged;
            int currflags = int.Parse(GameObject.Find("FlagCount").GetComponent<UnityEngine.UI.Text>().text);
            if (flagged)
                currflags--;
            else
                currflags++;
            GameObject.Find("FlagCount").GetComponent<UnityEngine.UI.Text>().text = currflags.ToString();
        } else {
            int knownNeighborhood = 0;
            foreach (GameObject n in neighbors) {
                if (n.GetComponent<MineItem>().flagged)
                    knownNeighborhood++;
            }
            if (knownNeighborhood >= bombNeighbors) {
                foreach (GameObject n in neighbors) {
                    n.GetComponent<MineItem>().dig();
                }
            }
        }
    }

    public void SetNeighbors(ArrayList ns) {
        neighbors = new GameObject[ns.Count];
        ns.CopyTo(neighbors);
    }

    public void SetNumber() {
        int num = 0;
        if (isBomb)
        {
            num = -1;
        }
        else
        {
            foreach (GameObject n in neighbors)
            {
                if (n.GetComponent<MineItem>().isBomb)
                {
                    num++;
                }
            }
        }
        bombNeighbors = num;
        TextMesh T = GetComponentInChildren<TextMesh>();
        T.text = num.ToString();
        if (num == 1)
            T.color = Color.cyan;
        else if (num == 2)
            T.color = Color.green;
        else if (num == 3)
            T.color = Color.red;
        else if (num == 4)
            T.color = Color.magenta;
        else if (num == 5)
            T.color = Color.blue;
        else if (num == 6)
            T.color = Color.yellow;
        else if (num >= 7)
            T.color = Color.black;
        else
            T.color = Color.clear;
    }
}
