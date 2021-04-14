using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generate : MonoBehaviour
{
    public GameObject square;
    public GameObject hexagon;
    public GameObject triangle;
    public GameObject cube;
    [SerializeField] public GameObject[][] gridBoard;
    public GameObject[] allItems;

    [SerializeField] private int rowCount;
    [SerializeField] private int colCount;
    [SerializeField] private int bombCount;

    public Sprite clock;
    public Sprite skull;
    public Sprite star;

    public bool activated = false;
    public bool done = false;
    public int totalDug;

    private bool moving;
    private GameObject camera;
    [SerializeField] private float moveSpeed;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        setUp(35, 15, 10, "C");
    }

    private void Update()
    {
        if (moving) {
            if (Input.GetKey(KeyCode.A))
                transform.eulerAngles += new Vector3(0, moveSpeed * Time.deltaTime, 0);
            if (Input.GetKey(KeyCode.D))
                transform.eulerAngles -= new Vector3(0, moveSpeed * Time.deltaTime, 0);
        }
    }

    void begin() {
        string s = GameObject.Find("Label").GetComponent<Text>().text;
        Debug.Log(s);
        if (s.Equals("Medium")) {
            setUp(10, 10, 20, "S");
        } else if (s.Equals("Large")) {
            setUp(18, 18, 40, "S");
        } else if (s.Equals("Viking Size")) {
            setUp(24, 20, 99, "S");
        } else if (s.Equals("HexMedium")) {
            setUp(18, 18, 60, "H");
        } else if (s.Equals("BigHex")) {
            setUp(100, 100, 800, "H");
        } else if (s.Equals("Cylinder Medium")) {
            setUp(25, 15, 40, "C");
        } else if (s.Equals("Cylinder Large")) {
            setUp(35, 18, 140, "C");
        }
    }

    public void lose() {
        done = true;
        foreach (GameObject s in allItems) {
            if (s.GetComponent<MineItem>().isBomb)
                s.GetComponent<MineItem>().dig();
        }
        GameObject.Find("stateImage").GetComponent<Image>().sprite = skull;
    }

    public void successfulDig() {
        totalDug++;
        if (totalDug + bombCount == allItems.Length) {
            done = true;
            GameObject.Find("stateImage").GetComponent<Image>().sprite = star;
        }
    }

    void placeSq(int i, int j) {
        gridBoard[i][j] = Instantiate(square, gameObject.transform);
        this.allItems[i * colCount + j] = gridBoard[i][j];
        gridBoard[i][j].GetComponent<MineItem>().loc = new Vector2(i, j);
        gridBoard[i][j].gameObject.name = "Square " + i + "," + j;
        gridBoard[i][j].transform.localPosition = new Vector2(i, j);
    }

    ArrayList sqNeighbors(GameObject s) {
        ArrayList nbores = new ArrayList();
        Vector2 loc = s.GetComponent<MineItem>().loc;
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (!(i == 0 && j == 0) && loc.x + i >= 0 && loc.y + j >= 0
                    && loc.x + i < rowCount && loc.y + j < colCount) {
                    nbores.Add(gridBoard[(int)loc.x + i][(int)loc.y + j]);
                }
            }
        }
        return nbores;
    }

    void placeCyl(int i, int j) {
        float r = rowCount / (2 * Mathf.PI);
        float t = (float)i / rowCount * (2 * Mathf.PI);
        gridBoard[i][j] = Instantiate(cube, gameObject.transform);
        this.allItems[i * colCount + j] = gridBoard[i][j];
        gridBoard[i][j].GetComponent<MineItem>().loc = new Vector2(i, j);
        gridBoard[i][j].gameObject.name = "Square " + i + "," + j;
        gridBoard[i][j].transform.localPosition = new Vector3(r*Mathf.Sin(t),j,r*Mathf.Cos(t));
        gridBoard[i][j].transform.eulerAngles = new Vector3(0, 180 + (Mathf.Rad2Deg * t), 0);
    }

    ArrayList CylNeighbores(GameObject s) {
        ArrayList nbores = new ArrayList();
        Vector2 loc = s.GetComponent<MineItem>().loc;
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (!(i == 0 && j == 0) && loc.y + j >= 0 && loc.y + j < colCount) {
                    int x;
                    if (loc.x + i < 0)
                        x = rowCount + ((int)loc.x + i);
                    else
                        x = ((int)loc.x + i) % rowCount;
                    nbores.Add(gridBoard[x][(int)loc.y + j]);
                }
            }
        }
        return nbores;
    }

    void placeTri(int i, int j) {
        gridBoard[i][j] = Instantiate(triangle, gameObject.transform);
        this.allItems[i * colCount + j] = gridBoard[i][j];
        gridBoard[i][j].GetComponent<MineItem>().loc = new Vector2(i, j);
        gridBoard[i][j].gameObject.name = "Triangle " + i + "," + j;
        gridBoard[i][j].transform.localPosition = new Vector2(i * 0.60f, j);
        if ((i + j) % 2 == 0) {
            foreach (Transform child in gridBoard[i][j].GetComponentsInChildren<Transform>())
                child.eulerAngles += new Vector3(0, 0, 180);
        }
            
    }

    ArrayList triNeighbores(GameObject s) {
        ArrayList nbores = new ArrayList();
        Vector2 loc = s.GetComponent<MineItem>().loc;
        bool upfacing = (((int)loc.x + (int)loc.y) % 2 == 0);
        int imax = 1;
        for (int j = -1; j <= 1; j++) {
            if (j == 0 || (j == -1 && upfacing) || (j == 1 && !upfacing))
                imax = 2;
            else
                imax = 1;
            for (int i = -imax; i <= imax; i++) {
                if (!(i == 0 && j == 0) && loc.x + i >= 0 && loc.y + j >= 0
                    && loc.x + i < rowCount && loc.y + j < colCount) {
                    nbores.Add(gridBoard[(int)loc.x + i][(int)loc.y + j]);
                }
            }
        }
        return nbores;
    }

    void placeHex(int i, int j) {
        gridBoard[i][j] = Instantiate(hexagon, gameObject.transform);
        this.allItems[i * colCount + j] = gridBoard[i][j];
        gridBoard[i][j].GetComponent<MineItem>().loc = new Vector2(i, j);
        gridBoard[i][j].gameObject.name = "Hexagon " + i + "," + j;
        if (i % 2 == 1)
            gridBoard[i][j].transform.localPosition = new Vector2(i, j + .5f);
        else
            gridBoard[i][j].transform.localPosition = new Vector2(i, j);
    }

    ArrayList hexNeighbors(GameObject s) {
        ArrayList nbores = new ArrayList();
        Vector2 loc = s.GetComponent<MineItem>().loc;
        for (int j = -1; j <= 1; j += 2) {
            if (loc.y + j >= 0 && loc.y + j < colCount)
                nbores.Add(gridBoard[(int)loc.x][(int)loc.y + j]);
        }
        if (Mathf.FloorToInt(loc.x % 2) == 0) {
            for (int i = -1; i <= 1; i += 2) {
                for (int j = -1; j <= 0; j++) {
                    if (loc.x + i >= 0 && loc.x + i < rowCount
                        && loc.y + j >= 0 && loc.y + j < colCount)
                        nbores.Add(gridBoard[(int)loc.x + i][(int)loc.y + j]);
                }
            }
        } else {
            for(int i = -1; i <= 1; i += 2) {
                for (int j = 0; j <= 1; j++) {
                    if (loc.x + i >= 0 && loc.x + i < rowCount
                        && loc.y + j >= 0 && loc.y + j < colCount)
                        nbores.Add(gridBoard[(int)loc.x + i][(int)loc.y + j]);
                }
            }
        }
        return nbores;
    }

    void setUp(int rows, int columns, int bombs, string style) {
        transform.eulerAngles = Vector3.zero;
        moving = false;
        done = false;
        activated = false;
        totalDug = 0;
        GameObject.Find("Timer").GetComponent<timer>().clockReset();
        GameObject.Find("stateImage").GetComponent<Image>().sprite = clock;
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
        rowCount = rows;
        colCount = columns;
        bombCount = bombs;
        gridBoard = new GameObject[rows][];
        allItems = new GameObject[rows * columns];
        for (int i = 0; i < rows; i++) {
            gridBoard[i] = new GameObject[columns];
            for (int j = 0; j < columns; j++) {
                if (style.Equals("S"))
                    placeSq(i, j);
                else if (style.Equals("H"))
                    placeHex(i, j);
                else if (style.Equals("T"))
                    placeTri(i, j);
                else if (style.Equals("C"))
                    placeCyl(i, j);
            }
        }
        foreach (GameObject s in allItems) {
            ArrayList ns = new ArrayList();
            if (style.Equals("S"))
                ns = sqNeighbors(s);
            else if (style.Equals("H"))
                ns = hexNeighbors(s);
            else if (style.Equals("T"))
                ns = triNeighbores(s);
            else if (style.Equals("C"))
                ns = CylNeighbores(s);
            s.GetComponent<MineItem>().SetNeighbors(ns);
        }
        if (style.Equals("C")) {
            camera.transform.localPosition = new Vector3(0, columns / 2, -(columns + rows)/2);
            camera.GetComponent<Camera>().orthographic = false;
            moving = true;
        } else {
            camera.transform.localPosition = new Vector3(rows / 2, columns / 2, -10);
            camera.GetComponent<Camera>().orthographic = true;
            camera.GetComponent<Camera>().orthographicSize = Mathf.Max(rows, columns) / 2 + 1;
        }
        GameObject.Find("FlagCount").GetComponent<UnityEngine.UI.Text>().text = bombs.ToString();
    }

    public void activate(Vector2 start) {
        activated = true;
        for (int k = 0; k < bombCount;)
        {
            int r = Random.Range(0, colCount * rowCount);
            MineItem rMI = allItems[r].GetComponent<MineItem>();
            Vector2 diff = rMI.loc - start;
            if (!rMI.isBomb && !(Mathf.Abs(diff.x) < 3 && Mathf.Abs(diff.y) < 3)) {
                rMI.isBomb = true;
                allItems[r].name += " B";
                k++;
            }
        }
        foreach (GameObject s in allItems)
        {
            s.GetComponent<MineItem>().SetNumber();
        }
    }
}
