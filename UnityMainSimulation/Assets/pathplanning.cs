using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathplanning : MonoBehaviour
{
    static int N = 40;
    static int W = 10;

   
    public string[] familyMembers = new string[ ]{"Greg", "Kate", "Adam", "Mia"} ; 

    public GameObject drone;
    public GameObject takeoff_base;
    public GameObject landing_base;

    List<Node> openSet = new List<Node>();
    List<Node> closedSet = new List<Node>();
    List<List<Node>> nodes = new List<List<Node>>();
    Node current;
    Node start;
    Node end;

    class Node{
        public int[] coordinates;
        public Vector3 position;
        public GameObject obj;
        public float fScore;
        public float gScore;
        public Node parent;



        public Node(int[] coor) 
            { 
                coordinates = coor;
                position = coordinate_to_screen(coordinates);
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.position = position;
                obj.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                fScore = int.MaxValue;
                gScore = int.MaxValue;


            }
        public Vector3 coordinate_to_screen(int[] coor){
            return new Vector3(((float)coor[0]/N-0.5f)*W, 0, ((float)coor[1]/N-0.5f)*W);
        }

        public void setParent(Node par){
            parent = par;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        // Node[] nodes = new Node[]{};
        for (int i = 0; i < N; i++){
            List<Node> row = new List<Node>();
            for (int j = 0; j < N; j++){
                row.Add(new Node(new int[]{i,j}));
            }
            nodes.Add(row);
        }
        start = nodes[0][0];
        end = nodes[N-1][N-1];

        start.fScore = 1*Vector3.Distance(start.obj.transform.position, end.obj.transform.position);
        start.gScore = 0;
        openSet.Add(start);
    }

    // Update is called once per frame
    void Update()
    {
        float minScore = int.MaxValue;

        
        foreach (Node node in openSet){
            if (node.fScore < minScore){
                minScore = node.fScore;
                current = node;
            }
            
        }

        // if (current == end){
        //     # Reconstruct Path
        // }
        Debug.Log(openSet.Count);
        Debug.Log(closedSet.Count);

        current.obj.GetComponent<Renderer>().material.color = Color.red;

        openSet.Remove(current);
        closedSet.Add(current);

        for (int i = -2; i < 3; i ++){
            for (int j = -2; j < 3; j ++){
                int[] newCoor = new int[]{current.coordinates[0]+i,current.coordinates[1]+j};
                if (!((i==0) && (j==0)) && 0 <= newCoor[0] && newCoor[0] < N && 0 <= newCoor[1] && newCoor[1] < N){
                    Node neighbour = nodes[newCoor[0]][newCoor[1]];
                    
                    // The distance from start to a neighbor
                    float tentative_gScore = current.gScore +  Vector3.Distance(current.obj.transform.position, neighbour.obj.transform.position);

                    if (tentative_gScore < neighbour.gScore){
                        neighbour.setParent(current);
                        neighbour.gScore = tentative_gScore;
                        neighbour.fScore = neighbour.gScore +  1*Vector3.Distance(neighbour.obj.transform.position, end.obj.transform.position);
                    }

                    if (!openSet.Contains(neighbour)){
                        openSet.Add(neighbour);

                    }
                }
            }
        }

    }
}
