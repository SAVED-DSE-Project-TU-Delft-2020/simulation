using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathplanning : MonoBehaviour
{
    static int N = 50;
    static int W = 10;
    public int t = 0;

    int p = 0;
   
    public Environment environment;
    public GameObject drone;
    public GameObject takeoff_base;
    bool finished;

    public GameObject landing_base;
    public LineRenderer line;
    public GameObject nodeObj;

    public GameObject target;

    List<Node> openSet;
    List<Node> closedSet;
    List<List<Node>> nodes = new List<List<Node>>();
    Node current;
    Node start;
    Node end;

    List<Vector3> path;

    public class Node{
        public int[] coordinates;
        public Vector3 position;
        public GameObject obj;
        public float fScore;
        public float gScore;
        public Node parent;
        public NodeBehaviour script;

        public LineRenderer edge;
        public GameObject node;

        public Node(int[] coor, GameObject n) 
            { 
                node = n;
                coordinates = coor;
                position = coordinate_to_screen(coordinates);
                obj = node.transform.GetChild(0).gameObject;
                obj.transform.position = position;
                Vector3 diagonal =  Singleton.instance.landing_base.transform.position - Singleton.instance.drone.transform.position;
                float cost =  1+Mathf.Abs(Mathf.Sin(position.x) + Mathf.Sin(position.z));
                Color a = new Color((cost-1)/2, 1-(cost-1)/2, 0f, 1.0f);
                obj.transform.localScale = new Vector3(diagonal.x/N, 0.05f, diagonal.z/N)*0.7f;
                obj.GetComponent<Renderer>().material.color = a;

                obj.GetComponent<MeshRenderer>().enabled  = true;
                // obj.transform.localScale = new Vector3(0.05f,0.05f,0.05f)*(Mathf.Abs(Mathf.Sin(2*position.x) + Mathf.Sin(2*position.z)));
                fScore = int.MaxValue;
                gScore = int.MaxValue;
                script = obj.GetComponent<NodeBehaviour>();
                edge = n.transform.GetChild(1).GetComponent<LineRenderer>();

            }
        public Vector3 coordinate_to_screen(int[] coor){
            
            Vector3 diagonal =  Singleton.instance.landing_base.transform.position - Singleton.instance.drone.transform.position;
            return new Vector3(((float)coor[0]/N)*(diagonal.x) + Singleton.instance.drone.transform.position.x, 0.0f, ((float)coor[1]/N)*(diagonal.z) + Singleton.instance.drone.transform.position.z);
        }

        public void setParent(Node par){
            parent = par;
            edge.SetPosition(0, par.position );
            edge.SetPosition(1,position);
            edge.startWidth = 0.1f;
            edge.endWidth = 0.01f;
        }

        public float cost(){
            if (script.isColliding){

                return int.MaxValue;
            }else{
                float c =1+Mathf.Pow(Mathf.Abs(Mathf.Sin(position.x) + Mathf.Sin(position.z)),5);
                return c;
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        initializePP();
    }

    // Update is called once per frame
    void Update()
    {

        if (finished == false){
            
            for (int _ = 0; _ < 10; _++){
                float minScore = int.MaxValue;
        
                foreach (Node node in openSet){
                    if (node.fScore < minScore){
                        minScore = node.fScore;
                        current = node;
                    }
                }
                current.edge.enabled = true;

                if (current == end && path.Count == 0){
                    finished = true;
                    while (current != start && current.obj != null){
                        path.Insert(0, current.position);
                        current.obj.GetComponent<Renderer>().material.color = Color.yellow;
                        current.obj.GetComponent<MeshRenderer>().enabled = true;
                        current.edge.startColor = Color.yellow;
                        current.edge.endColor = Color.yellow;

                        current.edge.startWidth = 0.2f;
                        current.edge.endWidth = 0.2f;
                        current = current.parent;
                    }
                    StartCoroutine(SpawnLoop(path));                     // Start moving drone
                    // initializePP();
                }
                else{
                    openSet.Remove(current);
                    closedSet.Add(current);

                    for (int i = -3; i < 4; i ++){
                        for (int j = -3; j < 4; j ++){
                            int[] newCoor = new int[]{current.coordinates[0]+i,current.coordinates[1]+j};
                            if (!((i==0) && (j==0)) && 0 <= newCoor[0] && newCoor[0] < N && 0 <= newCoor[1] && newCoor[1] < N){
                                Node neighbour = nodes[newCoor[0]][newCoor[1]];
                                if (closedSet.Contains(neighbour)){
                                    continue;
                                }
                                
                                // The distance from start to a neighbor
                                float tentative_gScore = current.gScore +  (current.cost() + neighbour.cost())/2*Vector3.Distance(current.obj.transform.position, neighbour.obj.transform.position);

                                if (tentative_gScore < neighbour.gScore){
                                    neighbour.setParent(current);
                                    neighbour.gScore = tentative_gScore;
                                    neighbour.fScore = neighbour.gScore +  1.0f*Vector3.Distance(neighbour.obj.transform.position, end.obj.transform.position);
                                }

                                if (!openSet.Contains(neighbour)){
                                    openSet.Add(neighbour);
                                }
                            }
                        }
                    }
                }

                
        
            }
            t += 1;
        }
    }

    void initializePP(){
        Debug.Log("Replanning");
        finished = false;
        path = new List<Vector3>();
        openSet = new List<Node>();
        closedSet = new List<Node>();

         for (int i = 0; i < nodes.Count; i++){
            for (int j = 0; j < nodes[0].Count; j++){
                Destroy(nodes[i][j].node);
                Destroy(nodes[i][j].obj);
                Destroy(nodes[i][j].edge);
            }
         }

        nodes = new List<List<Node>>();

        for (int i = 0; i < N; i++){
            List<Node> row = new List<Node>();
            for (int j = 0; j < N; j++){
                row.Add(new Node(new int[]{i,j}, Instantiate(nodeObj)));
            }
            nodes.Add(row);
        }
        start = nodes[0][0];
        end = nodes[N-1][N-1];

        start.obj.GetComponent<MeshRenderer>().enabled = true;
        start.obj.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        end.obj.GetComponent<MeshRenderer>().enabled = true;
        end.obj.transform.localScale = new Vector3(0.5f,0.5f,0.5f);

        start.fScore = 0;
        start.gScore = 0;
        openSet.Add(start);

        // Color nodes
        for (int i = 0; i < N; i++){
            for (int j = 0; j < N; j++){
                if (nodes[i][j].script.isColliding){
                    nodes[i][j].obj.GetComponent<Renderer>().material.color = new Color(1.0f,0f,0f,1.0f);
                }
            }
        }
    }

    IEnumerator SpawnLoop (List<Vector3> pathfound) {
    while (true){
        yield return new WaitForSeconds (0.6f);
            if (p<pathfound.Count){
                target.transform.position = pathfound[p] + new Vector3(0f, 5f, 0f);
                p+=1;
            }
        }
     }
     
}
