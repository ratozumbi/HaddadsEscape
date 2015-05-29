using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorController : MonoBehaviour {

    public bool started = false;

    public float objectsMinDistance;
    public float objectsMaxDistance;

    //Criação dos backgrounds
    public GameObject[] availableRooms;
    public List<GameObject> currentRooms;

    //Criação dos objetos de interação
    public GameObject water;
    public GameObject[] availableObjects;
    public List<GameObject> objects;

    private int countObject = 0;
    private int objectsToWater = 0;
    private float objectsYup = -1.38f;
    private float objectsYdown = -2.63f;
    private float screenWidthInPoints;

	void Start () 
    {
        float height = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;

        this.objectsToWater = this.SetDifficult();
	}

    int SetDifficult()
    {
        int difficultLevel = PlayerPrefs.GetInt("Difficult", 2);

        switch (difficultLevel)
        {
            case 1:
                return 8;
            case 2:
                return 10;
            case 3:
                return 16;
            default:
                return 10;
        }
    }

    void FixedUpdate()
    {
        if (started)
        {
            GenerateRoomIfRequired();
            GenerateObjectsIfRequired();
        }
    }

    void AddRoom(float farhtestRoomEndX)
    {
        int randomRoomIndex = Random.Range(0, availableRooms.Length);
        GameObject room = (GameObject)Instantiate(availableRooms[randomRoomIndex]);
        float roomWidth = room.transform.FindChild("ground_object_down").localScale.x;
        float roomCenter = farhtestRoomEndX + roomWidth * 0.5f;

        room.transform.position = new Vector3(roomCenter, 1.75f, 0);

        currentRooms.Add(room);
    }

    void AddObject(float lastObjectX)
    {
        GameObject obj;
        float randomY = 0;
        int randomIndex = Random.Range(0, availableObjects.Length);
        float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);

        if (countObject == objectsToWater)
        {
            countObject = 0;
            obj = (GameObject)Instantiate(water);
        }
        else
            obj = (GameObject)Instantiate(availableObjects[randomIndex]);
        
        if (obj.name.Contains("obstaculo_arvore3"))
            randomY = -1.69f;
        else if (obj.name.Contains("obstaculo_arvore1") || obj.name.Contains("obstaculo_arvore2"))
            randomY = Random.Range(0, 2) == 1 ? -3.05f : -1.82f;
        else
            randomY = Random.Range(0, 2) == 1 ? objectsYdown : objectsYup;

        obj.transform.position = new Vector3(objectPositionX, randomY, 0);
        objects.Add(obj);
        countObject++;
    }

    void GenerateRoomIfRequired()
    {
        bool addRooms = true;
        List<GameObject> roomsToRemove = new List<GameObject>();

        float farthestRoomEndX = 0;
        float playerX = transform.position.x;
        float removeRoomX = playerX - screenWidthInPoints;
        float addRoomX = playerX + screenWidthInPoints;

        foreach (var room in currentRooms)
        {
            float roomWidth = room.transform.FindChild("ground_object_down").localScale.x;
            float roomStartX = room.transform.position.x - (roomWidth * 0.5f);
            float roomEndX = roomStartX + roomWidth;

            if (roomStartX > addRoomX)
                addRooms = false;

            if (roomEndX < removeRoomX)
                roomsToRemove.Add(room);

            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
        }

        foreach (var room in roomsToRemove)
        {
            currentRooms.Remove(room);
            Destroy(room);
        }

        if (addRooms)
            AddRoom(farthestRoomEndX);
    }

    void GenerateObjectsIfRequired()
    {
        float playerX = transform.position.x;
        float removeObjectsX = playerX - screenWidthInPoints;
        float addObjectX = playerX + screenWidthInPoints;
        float farthestObjectX = 0;

        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (var obj in objects)
        {
            if(obj != null) {
                float objX = obj.transform.position.x;

                farthestObjectX = Mathf.Max(farthestObjectX, objX);

                if (objX < removeObjectsX)
                    objectsToRemove.Add(obj);
            }
        }

        foreach (var obj in objectsToRemove)
        {
            objects.Remove(obj);
            Destroy(obj);
        }

        if (farthestObjectX < addObjectX)
            AddObject(farthestObjectX);
    }
}
