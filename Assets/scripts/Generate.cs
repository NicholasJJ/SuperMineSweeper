using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dummiesman;
using System.IO;

public class Generate : MonoBehaviour
{
    public GameObject square;
    public GameObject hexagon;
    public GameObject triangle;
    public GameObject cube;
    public GameObject line;
    [SerializeField] public GameObject[][] gridBoard;
    public GameObject[] allItems;

    [SerializeField] private Text objName;
    [SerializeField] private Text buildDebug;

    public Mesh cubeMesh;
    public ArrayList listItems = new ArrayList();

    [SerializeField] private int rowCount;
    [SerializeField] private int colCount;
    [SerializeField] private int bombCount;

    public Sprite clock;
    public Sprite skull;
    public Sprite star;

    public bool activated = false;
    public bool done = false;
    public int totalDug;

    private bool movingLeftRight;
    private bool movingUpDown;
    private GameObject camera;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float translateSpeed;

    float timer;
    bool waiting = false;
    float pbombs;
    Mesh pmesh;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");

        float distSample = 2f / Vector3.Distance(cubeMesh.vertices[cubeMesh.triangles[0]], cubeMesh.vertices[cubeMesh.triangles[1]]);
        Debug.Log(cubeMesh.vertices[0] + " " + distSample + " " + cubeMesh.vertices[0] * distSample);
        Vector3[] vertices = new Vector3[cubeMesh.vertices.Length];
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = cubeMesh.vertices[i] * distSample;
        }
        cubeMesh.vertices = vertices;
        


        Debug.Log(cubeMesh.vertices[0]);
        

        setUp(35, 15, 10, "C");

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) {
            transform.Translate(0.5f * translateSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0.5f * translateSpeed * Input.GetAxis("Vertical") * Time.deltaTime, 0, Space.World);
        } else {
            if (movingLeftRight)
            {
                transform.Rotate(0, rotateSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0, Space.World);
            }
            if (movingUpDown)
            {
                transform.Rotate(rotateSpeed * Input.GetAxis("Vertical") * Time.deltaTime, 0, 0, Space.World);
            }
        }
        
        
        if (waiting && Time.time > timer + 0.1f) {
            waiting = false;
            setUpPoly(pbombs, pmesh);
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
        } else if (s.Equals("Poly")) {
            slowSetupPoly(.2f, cubeMesh);
        }
    }

    void beginFromObj() {
        string s = objName.text;
        string directory = System.IO.Directory.GetCurrentDirectory();
        Mesh importMesh = new Mesh();
        string filePath = directory + "/" + s + ".obj";
        buildDebug.text = filePath;
        if (File.Exists(filePath)) {
            importMesh = new OBJLoader().Load(filePath).GetComponentInChildren<MeshFilter>().mesh;
            Destroy(GameObject.Find(s));
            float distSample = 2f / Vector3.Distance(importMesh.vertices[importMesh.triangles[0]], importMesh.vertices[importMesh.triangles[1]]);
            Vector3[] vertices = new Vector3[importMesh.vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = importMesh.vertices[i] * distSample;
            }
            importMesh.vertices = vertices;
            slowSetupPoly(.2f, importMesh);
        } else {
            Debug.Log("File DNE!");
            buildDebug.text += " FileDNE!";
        }
    }

    public void lose() {
        done = true;
        if (allItems.Length > 2) {
            foreach (GameObject s in allItems) {
                if (s.GetComponent<MineItem>().isBomb)
                    s.GetComponent<MineItem>().dig();
            }
        } else {
            foreach (GameObject s in listItems) {
                if (s.GetComponent<MineItem>().isBomb)
                    s.GetComponent<MineItem>().dig();
            }
        }
        GameObject.Find("stateImage").GetComponent<Image>().sprite = skull;
    }

    public void successfulDig() {
        totalDug++;
        if ((allItems.Length > 2 && totalDug + bombCount == allItems.Length)
            || (allItems.Length == 1 && totalDug + bombCount == listItems.Count)) {
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

    void placePoly(Vector3 v, Vector3[] tri, Vector3 normal) {
        if (!GameObject.Find("point " + v)) {
            GameObject m = Instantiate(cube, gameObject.transform);
            listItems.Add(m);
            m.name = "point " + v;
            m.transform.localPosition = v;
            //m.transform.LookAt(v.normalized * (-2 * v.magnitude));
            m.transform.LookAt(v + -normal.normalized);
            m.GetComponent<MineItem>().onPoly = true;
        } else {
            GameObject otherVert = GameObject.Find("point " + v);
            Vector3 newNormal = ((otherVert.transform.forward * otherVert.GetComponent<MineItem>().duplicatePoints)
                - normal.normalized).normalized;
            otherVert.transform.LookAt(v + newNormal.normalized);
            otherVert.GetComponent<MineItem>().duplicatePoints++;
        }
        GameObject thisVert = GameObject.Find("point " + v);
        foreach (Vector3 vertex in tri) {
            thisVert.GetComponent<MineItem>().addNeighborName(vertex);
            Vector3 thispos = thisVert.transform.localPosition;
            string lineName = Vector3.Min(thispos, vertex).ToString() + Vector3.Max(thispos, vertex).ToString();
            if (!GameObject.Find(lineName)) {
                GameObject l = Instantiate(line, transform);
                l.name = lineName;
                l.GetComponent<LineRenderer>().SetPosition(0, thispos);
                l.GetComponent<LineRenderer>().SetPosition(1, vertex);
            }
        }
            
    }

    ArrayList polyNeighbors(GameObject s) {
        ArrayList ns = new ArrayList();
        ArrayList nsVectors = s.GetComponent<MineItem>().getNeighborNames();
        foreach (Vector3 v in nsVectors) {
            ns.Add(GameObject.Find("point " + v));
        }
        return ns;
    }

    void slowSetupPoly(float bombs, Mesh mesh) {
        clearBoard();
        timer = Time.time;
        pbombs = bombs;
        pmesh = mesh;
        waiting = true;
    }

    void setUpPoly(float bombPercent, Mesh mesh) {
        clearBoard();
        listItems.Clear();
        allItems = new GameObject[1];
        Vector3[] triangleVerts = new Vector3[3];
        for (int i = 0; i < mesh.triangles.Length; i+=3) {
            for (int j = i; j < i+3; j++) {
                triangleVerts[j - i] = mesh.vertices[mesh.triangles[j]];
            }
            for (int j = i; j < i+3; j++) {
                placePoly(mesh.vertices[mesh.triangles[j]], triangleVerts, mesh.normals[mesh.triangles[j]]);
            }
        }
        bombCount = Mathf.FloorToInt(listItems.Count * bombPercent);
        GameObject.Find("FlagCount").GetComponent<UnityEngine.UI.Text>().text = bombCount.ToString();
        foreach (GameObject point in listItems) {
            point.GetComponent<MineItem>().SetNeighbors(polyNeighbors(point));
        }

        camera.transform.localPosition = new Vector3(0, 0, -10);
        camera.GetComponent<Camera>().orthographic = false;
        movingLeftRight = true;
        movingUpDown = true;
    }

    void setUp(int rows, int columns, int bombs, string style) {
        clearBoard();
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
            movingLeftRight = true;
        } else {
            camera.transform.localPosition = new Vector3(rows / 2, columns / 2, -10);
            camera.GetComponent<Camera>().orthographic = true;
            camera.GetComponent<Camera>().orthographicSize = Mathf.Max(rows, columns) / 2 + 1;
        }
        GameObject.Find("FlagCount").GetComponent<UnityEngine.UI.Text>().text = bombs.ToString();
    }

    void clearBoard() {
        transform.eulerAngles = Vector3.zero;
        transform.position = Vector3.zero;
        movingLeftRight = false;
        movingUpDown = false;
        done = false;
        activated = false;
        totalDug = 0;
        GameObject.Find("Timer").GetComponent<timer>().clockReset();
        GameObject.Find("stateImage").GetComponent<Image>().sprite = clock;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void activate(Vector2 start, bool poly, Vector3 start3) {
        activated = true;
        for (int k = 0; k < bombCount;)
        {
            if (poly) {
                int r = Random.Range(0, listItems.Count);
                MineItem rMI = ((GameObject) listItems[r]).GetComponent<MineItem>();
                bool nearStart = false;
                foreach (GameObject n in rMI.getNeighbors()) {
                    if (n.transform.localPosition == start3)
                        nearStart = true;
                }
                if (!rMI.isBomb && rMI.transform.localPosition != start3 && !nearStart) {
                    rMI.isBomb = true;
                    ((GameObject)listItems[r]).name += " B";
                    k++;
                }
            } else {
                int r = Random.Range(0, colCount * rowCount);
                MineItem rMI = allItems[r].GetComponent<MineItem>();
                Vector2 diff = rMI.loc - start;
                if (!rMI.isBomb && !(Mathf.Abs(diff.x) < 3 && Mathf.Abs(diff.y) < 3)) {
                    rMI.isBomb = true;
                    allItems[r].name += " B";
                    k++;
                }
            }
        }
        if (poly) {
            foreach (Object s in listItems) {
                ((GameObject) s).GetComponent<MineItem>().SetNumber();
            }
        } else {
            foreach (GameObject s in allItems) {
                s.GetComponent<MineItem>().SetNumber();
            }
        } 
    }
}
