using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.AI;

//Maze Generation code
//Started by Luke Kenny
//Start Date: 17/06/2020
//Last Updated: 12/11/2020


public class GenerateMaze : MonoBehaviour
{
    CellCLASS[,] GridData;    //2D array for grid references I.E checking if a space is void or occupied without having to use physics check spheres    
    int SizeX;                      //X grid dimension
    int SizeZ;                      //z grid dimension
    public int VoidNumber;
    int VoidsCreated = 0;
    int SpecialRoomNumber;
    bool FirstHunt = true;
    bool HuntSuccessful = false;        //Remember to change this to false after each kill loop is finished

    public GameObject Stairs;
    public GameObject Floor;
    public GameObject SpecialFloor;
    public GameObject VoidFloor;
    public GameObject Wall;
    public NavMeshSurface navFloor;
    [SerializeField]
    List<NavMeshSurface> navMeshSurfaces;

    void Start()
    {
        SizeX = 150;           //Choosing random grid size in range
        SizeZ = 150;

        if (SizeX % 2 == 0)             //Floor tiles will originate at odd number grid references i.e. 1,1 - so each dimension needs to be an even number.
        {
            SizeX++;                    //The IF statement checks to see if there is a remainder, if there is, then the dimension is an odd number and is increased by 1 to make it even.
        }
        if (SizeZ % 2 == 0)
        {
            SizeZ++;
        }

        GridData = new CellCLASS[SizeX, SizeZ];
        ClearGrid();
        Debug.Log("Here");
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject G in allObjects)
        {
            if (G.GetComponent<NavMeshSurface>() != null)
            {
                Debug.Log("AAAAAAAAAAAA");
                navMeshSurfaces.Add(G.GetComponent<NavMeshSurface>());
            }

        }
        Debug.Log("navMeshSurfaces Count is " + navMeshSurfaces.Count);

        for (int i = 0; i < navMeshSurfaces.Count; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
        //navFloor.BuildNavMesh();
        Debug.Log("End");
    }

    void ClearGrid()                    //Method for giving the grid a clean slate of C's for 'Clear'. When generating the paths, this will be referenced to check if a position is free
    {
        Debug.Log("Grid Created");

        for (int x = 0; x <= SizeX - 1; x++)            //For every x coordinate...
        {
            for (int z = 0; z <= SizeZ - 1; z++)            //... and for every y, add a C at that position
            {
                GridData[x, z] = new CellCLASS();
            }
        }


        Debug.Log("Grid Cleared");
        Debug.Log("Random Example Coordinate: " + GridData[Random.Range(0, GridData.GetLength(0)), Random.Range(0, GridData.GetLength(1))].Status);
        Debug.Log("End Coordinate: " + SizeX + "," + SizeZ);
        CreateStairs();
        CreateSpecials();
        CreateVoids();
        Kill();
    }

    void CreateStairs()     //Creates the stairs to reach the other levels of the maze
    {
        int StairNumber = 2;
        int StairX = 0;
        int StairZ = 0;
        while (StairNumber > 0)
        {
            bool ClearCheck = false;
            while (ClearCheck == false)
            {
                bool xselected = false;
                while (xselected == false)
                {
                    StairX = Random.Range(7, SizeX - 7);
                    if (StairX % 2 != 0)
                    {
                        xselected = true;
                    }
                }
                bool zselected = false;
                while (zselected == false)
                {
                    StairZ = Random.Range(7, SizeZ - 7);
                    if (StairZ % 2 != 0)
                    {
                        zselected = true;
                    }
                }

                //vv checking if the selected centre point coordinate, as well as left of, right of, above, and below (in that order) the coordinate are clear. 
                if (GridData[StairX, StairZ].Status == "Clear" && GridData[StairX + 4, StairZ].Status == "Clear" && GridData[StairX - 4, StairZ].Status == "Clear" && GridData[StairX, StairZ + 4].Status == "Clear" && GridData[StairX, StairZ - 4].Status == "Clear")
                {//Same Again for diagonals
                    if (GridData[StairX + 4, StairZ + 4].Status == "Clear" && GridData[StairX - 4, StairZ + 4].Status == "Clear" && GridData[StairX + 4, StairZ - 4].Status == "Clear" && GridData[StairX - 4, StairZ - 4].Status == "Clear")
                    {
                        ClearCheck = true;
                        StairNumber -= 1;
                    }
                }

            }
            Debug.Log("Creating stairs at " + StairX + "," + StairZ);
            //CREATE STAIRS
            Instantiate(Stairs, new Vector3(StairX, 0, StairZ), Quaternion.Euler(270, 0, 0));       //Spawning stair and updating the grid with the coordinates it is covering
            GridData[StairX, StairZ].Status = "Stair";
            GridData[StairX + 1, StairZ].Status = "Stair";
            GridData[StairX + 2, StairZ].Status = "Stair";
            GridData[StairX + 3, StairZ].Status = "Stair";
            GridData[StairX - 1, StairZ].Status = "Stair";
            GridData[StairX - 2, StairZ].Status = "Stair";
            GridData[StairX - 3, StairZ].Status = "Stair";

        }
        Debug.Log("Stair Loop Exited");


    }

    void CreateSpecials()                       //Currently only makes square rooms. Can be changed later when we have specific models to work with
    {
        Debug.Log("Creating Special Rooms");
        SpecialRoomNumber = Random.Range(3, 7);
        Debug.Log("Attempting " + SpecialRoomNumber + " Rooms");

        int SpecialX = 0;
        int SpecialZ = 0;

        while (SpecialRoomNumber > 0)           //Until all special rooms are made
        {
            bool ClearCheck = false;
            while (ClearCheck is false)         //Loop for checking if coordinate and surrounding area are clear
            {
                bool XSelected = false;
                while (XSelected is false)          //Selecting X coordinate
                {
                    SpecialX = Random.Range(6, SizeX - 6);
                    if (SpecialX % 2 != 0)
                    {
                        XSelected = true;
                    }
                }
                bool ZSelected = false;
                while (ZSelected is false)          //Selecting Z coordinate
                {
                    SpecialZ = Random.Range(6, SizeZ - 6);
                    if (SpecialZ % 2 != 0)
                    {
                        ZSelected = true;
                    }
                }
                //vv checking if the selected centre point coordinate, as well as left of, right of, above, and below (in that order) the coordinate are clear. 
                //if (GridData[SpecialX, SpecialZ].Status == "Clear" && GridData[SpecialX + 2, SpecialZ].Status == "Clear" && GridData[SpecialX + 4, SpecialZ].Status == "Clear" && GridData[SpecialX - 2, SpecialZ].Status == "Clear" && GridData[SpecialX - 4, SpecialZ].Status == "Clear" && GridData[SpecialX, SpecialZ + 2].Status == "Clear" && GridData[SpecialX, SpecialZ + 4].Status == "Clear" && GridData[SpecialX, SpecialZ - 2].Status == "Clear" && GridData[SpecialX, SpecialZ - 4].Status == "Clear")
                //{//Same Again for diagonals
                //    if (GridData[SpecialX + 2, SpecialZ + 2].Status == "Clear" && GridData[SpecialX + 4, SpecialZ + 4].Status == "Clear" && GridData[SpecialX - 2, SpecialZ + 2].Status == "Clear" && GridData[SpecialX - 4, SpecialZ + 4].Status == "Clear" && GridData[SpecialX + 2, SpecialZ - 2].Status == "Clear" && GridData[SpecialX + 4, SpecialZ - 4].Status == "Clear" && GridData[SpecialX - 2, SpecialZ - 2].Status == "Clear" && GridData[SpecialX - 4, SpecialZ - 4].Status == "Clear")
                //    {
                //        ClearCheck = true;
                //        SpecialRoomNumber -= 1;
                //    }
                //}
                Collider[] Colliders = Physics.OverlapSphere(new Vector3(SpecialX, 0, SpecialZ), 5f);
                if (Colliders.Length > 1)
                {
                    ClearCheck = false;
                }
                else
                {
                    ClearCheck = true;
                    SpecialRoomNumber -= 1;
                }
                //if (Physics.CheckSphere(new Vector3(SpecialX, 0, SpecialZ), 5f) == false)       //Since the maze is pretty empty at this point, Physics.Overlap is simpler to use than checking each position
                //{                                                                                //If this were a later stage in generation, this wouldn't be suitable as there would be too many objects interfering
                //    ClearCheck = true;
                //    SpecialRoomNumber -= 1;
                //}

            }
            Debug.Log("creating special room at coordiantes: " + SpecialX + "," + SpecialZ);
            //CREATE SPECIAL ROOM HERE
            for (int iv = SpecialZ - 2; iv <= SpecialZ + 2; iv += 2)            //Basically creating a square around the centre point SpecialX and Z 
            {
                for (int ih = SpecialX - 2; ih <= SpecialX + 2; ih += 2)
                {
                    Instantiate(SpecialFloor, new Vector3(ih, 0, iv), Quaternion.Euler(0, 0, 0));
                    GridData[ih, iv].Status = "Special";
                }
            }
            Instantiate(Wall, new Vector3(SpecialX - 2, 2, SpecialZ + 3), Quaternion.Euler(0, 90, 0));       //Creating the walls around the room
            GridData[SpecialX - 2, SpecialZ + 3].Status = "Special";
            Instantiate(Wall, new Vector3(SpecialX, 2, SpecialZ + 3), Quaternion.Euler(0, 90, 0));
            GridData[SpecialX, SpecialZ + 3].Status = "Special";
            Instantiate(Wall, new Vector3(SpecialX + 2, 2, SpecialZ + 3), Quaternion.Euler(0, 90, 0));
            GridData[SpecialX + 2, SpecialZ + 3].Status = "Special";

            Instantiate(Wall, new Vector3(SpecialX + 3, 2, SpecialZ + 2), Quaternion.Euler(0, 0, 0));
            GridData[SpecialX + 3, SpecialZ + 2].Status = "Special";
            Instantiate(Wall, new Vector3(SpecialX + 3, 2, SpecialZ), Quaternion.Euler(0, 0, 0));
            GridData[SpecialX + 3, SpecialZ].Status = "Special";
            Instantiate(Wall, new Vector3(SpecialX + 3, 2, SpecialZ - 2), Quaternion.Euler(0, 0, 0));
            GridData[SpecialX + 3, SpecialZ - 2].Status = "Special";

            Instantiate(Wall, new Vector3(SpecialX - 2, 2, SpecialZ - 3), Quaternion.Euler(0, 90, 0));
            GridData[SpecialX - 2, SpecialZ - 3].Status = "Special";
            //Instantiate(Wall, new Vector3(SpecialX, 2, SpecialZ - 3), Quaternion.Euler(0, 90, 0));            //Commented out so it can be a doorway in the future 
            GridData[SpecialX, SpecialZ - 3].Status = "Special";
            Instantiate(Wall, new Vector3(SpecialX + 2, 2, SpecialZ - 3), Quaternion.Euler(0, 90, 0));
            GridData[SpecialX + 2, SpecialZ - 3].Status = "Special";

            Instantiate(Wall, new Vector3(SpecialX - 3, 2, SpecialZ + 2), Quaternion.Euler(0, 0, 0));
            GridData[SpecialX - 3, SpecialZ + 2].Status = "Special";
            Instantiate(Wall, new Vector3(SpecialX - 3, 2, SpecialZ), Quaternion.Euler(0, 0, 0));
            GridData[SpecialX - 3, SpecialZ].Status = "Special";
            Instantiate(Wall, new Vector3(SpecialX - 3, 2, SpecialZ - 2), Quaternion.Euler(0, 0, 0));
            GridData[SpecialX - 3, SpecialZ - 2].Status = "Special";

        }
        Debug.Log("Exited Loop");
    }

    void CreateVoids()
    {
        Debug.Log("Creating Void Spaces");
        //VoidNumber = SizeX / 4;                                      //Selecting number of void paths
        Debug.Log("Attempting to create " + VoidNumber + " Void Paths");
        int VoidPositionX = 0;
        int VoidPositionZ = 0;

        int VoidTargetX = 0;
        int VoidTargetZ = 0;

        int VoidTargetForwardX = 0;
        int VoidTargetForwardZ = 0;

        int VoidTargetSide1X = 0;
        int VoidTargetSide1Z = 0;

        int VoidTargetSide2X = 0;
        int VoidTargetSide2Z = 0;

        while (VoidNumber > 0)                          //Will decrease by 1 every time a path is made
        {

            bool ClearCheck = false;
            while (ClearCheck is false)
            {
                bool XSelected = false;
                while (XSelected is false)                          //Selecting a path beginning location that isn't an even number
                {
                    VoidPositionX = Random.Range(1, SizeX - 1);
                    if (VoidPositionX % 2 != 0 && VoidPositionX > 1 && VoidPositionX < SizeX - 1)
                    {
                        XSelected = true;
                    }

                }
                bool ZSelected = false;
                while (ZSelected is false)
                {
                    VoidPositionZ = Random.Range(1, SizeZ - 1);
                    if (VoidPositionZ % 2 != 0 && VoidPositionZ > 1 && VoidPositionZ < SizeZ - 1)
                    {
                        ZSelected = true;
                    }

                }
                if (Physics.CheckSphere(new Vector3(VoidPositionX, 0, VoidPositionZ), 4f) == false)
                {
                    ClearCheck = true;
                }
                else
                {
                    ClearCheck = false;
                    VoidNumber -= 1;
                }
            }
            

            Instantiate(VoidFloor, new Vector3(VoidPositionX, 0, VoidPositionZ), Quaternion.Euler(0, 0, 0));        //First tile of void
            GridData[VoidPositionX, VoidPositionZ].Status = "Void";

            Instantiate(Wall, new Vector3(VoidPositionX, 2, VoidPositionZ + 1), Quaternion.Euler(0, 90, 0));
            GridData[VoidPositionX, VoidPositionZ + 1].Status = "Wall";

            Instantiate(Wall, new Vector3(VoidPositionX, 2, VoidPositionZ - 1), Quaternion.Euler(0, 90, 0));
            GridData[VoidPositionX, VoidPositionZ - 1].Status = "Wall";

            Instantiate(Wall, new Vector3(VoidPositionX - 1, 2, VoidPositionZ), Quaternion.Euler(0, 0, 0));
            GridData[VoidPositionX - 1, VoidPositionZ].Status = "Wall";

            Instantiate(Wall, new Vector3(VoidPositionX + 1, 2, VoidPositionZ), Quaternion.Euler(0, 0, 0));
            GridData[VoidPositionX + 1, VoidPositionZ].Status = "Wall";



            int VoidLength = 80;
            for (int i = 0; i < VoidLength; i++)                //For the number of spaces the void will be...
            {
                bool NoMoves = false;
                if (NoMoves is true)                //Condition for exiting the loop if there are no possible directions to be moved to
                {
                    break;
                }

                List<string> Directions = new List<string>();       //List of possible directions. If a direction wouldn't work, it is removed from the list
                Directions.Add("up");
                Directions.Add("down");
                Directions.Add("left");
                Directions.Add("right");

                bool Check = false;
                while(Check is false)
                {
                    if (Directions.Count > 0)
                    {
                        int DirectionChoiceInt = Random.Range(0, Directions.Count);
                        string DirectionChoice = Directions[DirectionChoiceInt];
                        Directions.Remove(Directions[DirectionChoiceInt]);

                        try
                        {
                            switch (DirectionChoice)
                            {
                                case "up":
                                    VoidTargetX = VoidPositionX;                //Setting the positions that need to be checked around the target position
                                    VoidTargetZ = VoidPositionZ + 2;

                                    VoidTargetForwardX = VoidTargetX;
                                    VoidTargetForwardZ = VoidTargetZ + 2;

                                    VoidTargetSide1X = VoidTargetX - 2;
                                    VoidTargetSide1Z = VoidTargetForwardZ;

                                    VoidTargetSide2X = VoidTargetX + 2;
                                    VoidTargetSide2Z = VoidTargetForwardZ;
                                    break;

                                case "down":
                                    VoidTargetX = VoidPositionX;
                                    VoidTargetZ = VoidPositionZ - 2;

                                    VoidTargetForwardX = VoidTargetX;
                                    VoidTargetForwardZ = VoidTargetZ - 2;

                                    VoidTargetSide1X = VoidTargetX - 2;
                                    VoidTargetSide1Z = VoidTargetForwardZ;

                                    VoidTargetSide2X = VoidTargetX + 2;
                                    VoidTargetSide2Z = VoidTargetForwardZ;

                                    break;

                                case "left":
                                    VoidTargetX = VoidPositionX - 2;
                                    VoidTargetZ = VoidPositionZ;

                                    VoidTargetForwardX = VoidTargetX - 2;
                                    VoidTargetForwardZ = VoidTargetZ;

                                    VoidTargetSide1X = VoidTargetForwardX;
                                    VoidTargetSide1Z = VoidTargetForwardZ - 2;

                                    VoidTargetSide2X = VoidTargetForwardX;
                                    VoidTargetSide2Z = VoidTargetForwardZ + 2;

                                    break;

                                case "right":
                                    VoidTargetX = VoidPositionX + 2;
                                    VoidTargetZ = VoidPositionZ;

                                    VoidTargetForwardX = VoidTargetX + 2;
                                    VoidTargetForwardZ = VoidTargetZ;

                                    VoidTargetSide1X = VoidTargetForwardX;
                                    VoidTargetSide1Z = VoidTargetForwardZ - 2;

                                    VoidTargetSide2X = VoidTargetForwardX;
                                    VoidTargetSide2Z = VoidTargetForwardZ + 2;

                                    break;

                            }
                            // 'if' below is checking that the target and surrounding area is clear
                            if (GridData[VoidTargetX, VoidTargetZ].Status == "Clear" && GridData[VoidTargetForwardX, VoidTargetForwardZ].Status == "Clear" && GridData[VoidTargetSide1X, VoidTargetSide1Z].Status == "Clear" && GridData[VoidTargetSide2X, VoidTargetSide2Z].Status == "Clear" && VoidTargetX > 1 && VoidTargetX < SizeX - 3 && VoidTargetZ > 1 && VoidTargetZ < SizeZ - 3)
                            {

                                VoidPositionX = VoidTargetX;        //Updating the current position to the target position after the check
                                VoidPositionZ = VoidTargetZ;

                                Instantiate(VoidFloor, new Vector3(VoidPositionX, 0, VoidPositionZ), Quaternion.Euler(0, 0, 0));
                                GridData[VoidPositionX, VoidPositionZ].Status = "Void";
                                Collider[] HitColliders;
                                switch (DirectionChoice)        //Creating walls around the new tile except the wall that would seperate it from the previous tile
                                {
                                    case "up":             

                                        HitColliders = Physics.OverlapSphere(new Vector3(VoidPositionX, 2, VoidPositionZ - 1), 0.5f);
                                        foreach(Collider hit in HitColliders)
                                        {
                                            GridData[VoidPositionX, VoidPositionZ - 1].Status = "Clear";
                                            Destroy(hit.gameObject);
                                        }

                                        if (GridData[VoidPositionX, VoidPositionZ + 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX, 2, VoidPositionZ + 1), Quaternion.Euler(0, 90, 0));
                                            GridData[VoidPositionX, VoidPositionZ + 1].Status = "Wall";
                                        }
                                        if (GridData[VoidPositionX - 1, VoidPositionZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX - 1, 2, VoidPositionZ), Quaternion.Euler(0, 0, 0));
                                            GridData[VoidPositionX - 1, VoidPositionZ].Status = "Wall";
                                        }
                                        if (GridData[VoidPositionX + 1, VoidPositionZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX + 1, 2, VoidPositionZ), Quaternion.Euler(0, 0, 0));
                                            GridData[VoidPositionX + 1, VoidPositionZ].Status = "Wall";
                                        }


                                        
                                        break;

                                    case "down":

                                        HitColliders = Physics.OverlapSphere(new Vector3(VoidPositionX, 2, VoidPositionZ + 1), 0.5f);
                                        foreach (Collider hit in HitColliders)
                                        {
                                            GridData[VoidPositionX, VoidPositionZ + 1].Status = "Clear";
                                            Destroy(hit.gameObject);
                                        }

                                        if (GridData[VoidPositionX, VoidPositionZ - 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX, 2, VoidPositionZ - 1), Quaternion.Euler(0, 90, 0));
                                            GridData[VoidPositionX, VoidPositionZ - 1].Status = "Wall";
                                        }
                                        if (GridData[VoidPositionX - 1, VoidPositionZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX - 1, 2, VoidPositionZ), Quaternion.Euler(0, 0, 0));
                                            GridData[VoidPositionX - 1, VoidPositionZ].Status = "Wall";
                                        }
                                        if (GridData[VoidPositionX + 1, VoidPositionZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX + 1, 2, VoidPositionZ), Quaternion.Euler(0, 0, 0));
                                            GridData[VoidPositionX + 1, VoidPositionZ].Status = "Wall";
                                        }

                                        
                                        break;

                                    case "left":

                                        HitColliders = Physics.OverlapSphere(new Vector3(VoidPositionX + 1, 2, VoidPositionZ), 0.5f);
                                        foreach (Collider hit in HitColliders)
                                        {
                                            GridData[VoidPositionX + 1, VoidPositionZ].Status = "Clear";
                                            Destroy(hit.gameObject);
                                        }

                                        if (GridData[VoidPositionX, VoidPositionZ - 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX, 2, VoidPositionZ - 1), Quaternion.Euler(0, 90, 0));
                                            GridData[VoidPositionX, VoidPositionZ - 1].Status = "Wall";
                                        }
                                        if (GridData[VoidPositionX - 1, VoidPositionZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX - 1, 2, VoidPositionZ), Quaternion.Euler(0, 0, 0));
                                            GridData[VoidPositionX - 1, VoidPositionZ].Status = "Wall";
                                        }
                                        if (GridData[VoidPositionX, VoidPositionZ + 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX, 2, VoidPositionZ + 1), Quaternion.Euler(0, 90, 0));
                                            GridData[VoidPositionX, VoidPositionZ + 1].Status = "Wall";
                                        }

                                        
                                        break;

                                    case "right":

                                        HitColliders = Physics.OverlapSphere(new Vector3(VoidPositionX - 1, 2, VoidPositionZ), 0.5f);
                                        foreach (Collider hit in HitColliders)
                                        {
                                            GridData[VoidPositionX - 1, VoidPositionZ].Status = "Clear";
                                            Destroy(hit.gameObject);
                                        }

                                        if (GridData[VoidPositionX, VoidPositionZ - 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX, 2, VoidPositionZ - 1), Quaternion.Euler(0, 90, 0));
                                            GridData[VoidPositionX, VoidPositionZ - 1].Status = "Wall";
                                        }
                                        if (GridData[VoidPositionX + 1, VoidPositionZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX + 1, 2, VoidPositionZ), Quaternion.Euler(0, 0, 0));
                                            GridData[VoidPositionX + 1, VoidPositionZ].Status = "Wall";
                                        }
                                        if (GridData[VoidPositionX, VoidPositionZ + 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(VoidPositionX, 2, VoidPositionZ + 1), Quaternion.Euler(0, 90, 0));
                                            GridData[VoidPositionX, VoidPositionZ + 1].Status = "Wall";
                                        }

                                        
                                        break;
                                }

                                Check = true;
                            }
                        }
                        catch
                        {
                            Debug.Log("Target was outside of grid");
                        }
                    }
                    else
                    {
                        NoMoves = true;
                        break;
                    }
                }
            }
            VoidNumber -= 1;
            
        }
    }

    void Hunt(out int XStart, out int ZStart)       //Kill method should run first and call to Hunt to pass the starting coordinates out to Kill method
    {
        XStart = 0;
        ZStart = 0;
        Debug.Log("Beginning hunt and kill phase");
        if (FirstHunt is true)                          //This IF is only true once: when there are no existing paths in the maze
        {                                              //All paths must connect by being built from an existing path, this loop creates the first path from random coordinates
            bool found = false;
            while (found is false)
            {
                XStart = Random.Range(1, SizeX);
                if (XStart % 2 == 0)
                {
                    XStart++;
                }

                ZStart = Random.Range(1, SizeZ);
                if (ZStart % 2 == 0)
                {
                    ZStart++;
                }

                if (GridData[XStart, ZStart].Status == "Clear")
                {
                    found = true;
                }
            }
            FirstHunt = false;
            HuntSuccessful = true;
        }
        else
        {                                               //This loop runs every time other than the first time as it finds an existing path tile, and builds from it
            bool PathFound = false;
            for (int x = 1; x <= SizeX - 2; x += 2)
            {
                for (int z = 1; z <= SizeZ - 2; z += 2)
                {
                    if (GridData[x, z].Status == "Path")        //Checks to see if space is an existing path
                    {
                        try
                        {
                            if (GridData[x, z + 2].Status == "Clear")
                            {  //Checks that space has an adjacent clear tile that can be moved to in Kill method
                                XStart = x;
                                ZStart = z;
                                PathFound = true;
                                HuntSuccessful = true;
                            }
                        }
                        catch
                        {
                            Debug.Log("Hunt clear check was outside index range");
                        }
                        try
                        {
                            if (GridData[x, z - 2].Status == "Clear")
                            {
                                XStart = x;
                                ZStart = z;
                                PathFound = true;
                                HuntSuccessful = true;
                            }
                        }
                        catch
                        {
                            Debug.Log("Hunt clear check was outside index range");
                        }
                        try
                        {
                            if (GridData[x - 2, z].Status == "Clear")
                            {
                                XStart = x;
                                ZStart = z;
                                PathFound = true;
                                HuntSuccessful = true;
                            }
                        }
                        catch
                        {
                            Debug.Log("Hunt clear check was outside index range");
                        }
                        try
                        {
                            if (GridData[x + 2, z].Status == "Clear")
                            {
                                XStart = x;
                                ZStart = z;
                                PathFound = true;
                                HuntSuccessful = true;
                            }
                        }
                        catch
                        {
                            Debug.Log("Hunt clear check was outside index range");
                        } 
                    }
                    if (PathFound is true)
                    {
                        break;
                    }
                }
                if (PathFound is true)
                {
                    break;
                }
            }
        }
             

        
        //Obsolete, proof that first tile can be placed
        //Instantiate(Floor, new Vector3(XStart, 0, ZStart), Quaternion.Euler(0, 0, 0));
        //GridData[XStart, ZStart].Status = "Path";
        //if (GridData[XStart, ZStart + 1].Status == "Clear")
        //{
        //    Instantiate(Wall, new Vector3(XStart, 2, ZStart + 1), Quaternion.Euler(0, 90, 0));
        //}
        //if (GridData[XStart, ZStart - 1].Status == "Clear")
        //{
        //    Instantiate(Wall, new Vector3(XStart, 2, ZStart - 1), Quaternion.Euler(0, 90, 0));
        //}
        //if (GridData[XStart - 1, ZStart].Status == "Clear")
        //{
        //    Instantiate(Wall, new Vector3(XStart - 1, 2, ZStart), Quaternion.Euler(0, 0, 0));
        //}
        //if (GridData[XStart + 1, ZStart].Status == "Clear")
        //{
        //    Instantiate(Wall, new Vector3(XStart + 1, 2, ZStart), Quaternion.Euler(0, 0, 0));
        //}

        

    }

    void Kill()
    {
        //NOTE: HuntSuccessful attribute must be false at end of every loop
        //, Check if starting coordinates from Hunt are clear or not. First path will always be clear but co-ords after that should always be an existing path 
        //- to avoid placing the first tile on top of an existing "Path" space
        int FreeSpaces = 0;
        bool HuntAndKill = true;
        while (HuntAndKill is true)
        {
            //for (int x = 1; x <= SizeX - 1; x += 2)
            //{
            //    for (int z = 1; z <= SizeZ - 1; z += 2)
            //    {
            //        if (GridData[x,z].Status == "Clear")
            //        {
            //            FreeSpaces += 1;
            //        }
            //    }
            //}

            //Debug.Log(FreeSpaces.ToString());
            //FreeSpaces = 0;
            int CurrentX = 0;
            int CurrentZ = 0;
            int TargetX = 0;
            int TargetZ = 0;
            Hunt(out CurrentX, out CurrentZ);

            if (HuntSuccessful is true)
            {
                HuntSuccessful = false;
                bool killing = true;
                int test = 0;
                while (killing is true)
                {
                    test++;
                    Debug.Log(test.ToString());
                    List<string> Directions = new List<string>() { "up", "down", "left", "right" };
                    bool DirectionFound = false;
                    bool NoDirections = false;
                    while (DirectionFound is false)
                    {
                        if (Directions.Count == 0)
                        {
                            NoDirections = true;
                            break;
                        }
                        int choicenum = Random.Range(0, Directions.Count);
                        string choice = Directions[choicenum];
                        Directions.Remove(Directions[choicenum]);

                        switch (choice)
                        {
                            case "up":
                                TargetX = CurrentX;
                                TargetZ = CurrentZ + 2;
                                break;

                            case "down":
                                TargetX = CurrentX;
                                TargetZ = CurrentZ - 2;
                                break;

                            case "left":
                                TargetX = CurrentX - 2;
                                TargetZ = CurrentZ;
                                break;

                            case "right":
                                TargetX = CurrentX + 2;
                                TargetZ = CurrentZ;
                                break;
                        }

                        try
                        {
                            if (GridData[TargetX, TargetZ].Status == "Clear")
                            {
                                
                                Collider[] HitColliders;
                                switch (choice)
                                {
                                    case "up":
                                        HitColliders = Physics.OverlapSphere(new Vector3(TargetX, 2, TargetZ - 1), 0.5f);
                                        foreach (Collider c in HitColliders)
                                        {
                                            GridData[TargetX, TargetZ - 1].Status = "Clear";
                                            Destroy(c.gameObject);
                                        }


                                        if (GridData[TargetX, TargetZ + 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX, 2, TargetZ + 1), Quaternion.Euler(0, 90, 0));
                                            GridData[TargetX, TargetZ + 1].Status = "Wall";
                                        }
                                        if (GridData[TargetX - 1, TargetZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX - 1, 2, TargetZ), Quaternion.Euler(0, 0, 0));
                                            GridData[TargetX - 1, TargetZ].Status = "Wall";
                                        }
                                        if (GridData[TargetX + 1, TargetZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX + 1, 2, TargetZ), Quaternion.Euler(0, 0, 0));
                                            GridData[TargetX + 1, TargetZ].Status = "Wall";
                                        }
                                        break;

                                    case "down":
                                        HitColliders = Physics.OverlapSphere(new Vector3(TargetX, 2, TargetZ + 1), 0.5f);
                                        foreach (Collider c in HitColliders)
                                        {
                                            GridData[TargetX, TargetZ + 1].Status = "Clear";
                                            Destroy(c.gameObject);
                                        }


                                        if (GridData[TargetX, TargetZ - 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX, 2, TargetZ - 1), Quaternion.Euler(0, 90, 0));
                                            GridData[TargetX, TargetZ - 1].Status = "Wall";
                                        }
                                        if (GridData[TargetX - 1, TargetZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX - 1, 2, TargetZ), Quaternion.Euler(0, 0, 0));
                                            GridData[TargetX - 1, TargetZ].Status = "Wall";
                                        }
                                        if (GridData[TargetX + 1, TargetZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX + 1, 2, TargetZ), Quaternion.Euler(0, 0, 0));
                                            GridData[TargetX + 1, TargetZ].Status = "Wall";
                                        }
                                        break;

                                    case "left":
                                        HitColliders = Physics.OverlapSphere(new Vector3(TargetX + 1, 2, TargetZ), 0.5f);
                                        foreach (Collider c in HitColliders)
                                        {
                                            GridData[TargetX + 1, TargetZ].Status = "Clear";
                                            Destroy(c.gameObject);
                                        }


                                        if (GridData[TargetX - 1, TargetZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX - 1, 2, TargetZ), Quaternion.Euler(0, 0, 0));
                                            GridData[TargetX - 1, TargetZ].Status = "Wall";
                                        }
                                        if (GridData[TargetX, TargetZ + 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX, 2, TargetZ + 1), Quaternion.Euler(0, 90, 0));
                                            GridData[TargetX, TargetZ + 1].Status = "Wall";
                                        }
                                        if (GridData[TargetX, TargetZ - 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX, 2, TargetZ - 1), Quaternion.Euler(0, 90, 0));
                                            GridData[TargetX, TargetZ - 1].Status = "Wall";
                                        }
                                        break;

                                    case "right":
                                        HitColliders = Physics.OverlapSphere(new Vector3(TargetX - 1, 2, TargetZ), 0.5f);
                                        foreach (Collider c in HitColliders)
                                        {
                                            GridData[TargetX - 1, TargetZ].Status = "Clear";
                                            Destroy(c.gameObject);
                                        }


                                        if (GridData[TargetX + 1, TargetZ].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX + 1, 2, TargetZ), Quaternion.Euler(0, 0, 0));
                                            GridData[TargetX + 1, TargetZ].Status = "Wall";
                                        }
                                        if (GridData[TargetX, TargetZ + 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX, 2, TargetZ + 1), Quaternion.Euler(0, 90, 0));
                                            GridData[TargetX, TargetZ + 1].Status = "Wall";
                                        }
                                        if (GridData[TargetX, TargetZ - 1].Status == "Clear")
                                        {
                                            Instantiate(Wall, new Vector3(TargetX, 2, TargetZ - 1), Quaternion.Euler(0, 90, 0));
                                            GridData[TargetX, TargetZ - 1].Status = "Wall";
                                        }
                                        break;
                                }
                                Instantiate(Floor, new Vector3(TargetX, 0, TargetZ), Quaternion.Euler(0, 0, 0));
                                GridData[TargetX, TargetZ].Status = "Path";
                                CurrentX = TargetX;
                                CurrentZ = TargetZ;
                                DirectionFound = true;
                            }
                        }
                        catch
                        {
                            Debug.Log("Kill direction check was outside of range");
                        }
                    }
                    if (NoDirections is true)
                    {
                        Debug.Log("Killing finished");
                        killing = false;
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("No spaces left, all possible paths created");
                HuntAndKill = false;
            }
        }
    }
}
