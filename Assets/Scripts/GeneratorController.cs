﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorController : MonoBehaviour {

    //Criação dos backgrounds
    public GameObject[] availableRooms;
    public List<GameObject> currentRooms;

    //Criação dos objetos de interação
    public GameObject[] availableObjects;
    public List<GameObject> objects;

    public float objectsYup = -1.38f;
    public float objectsYdown = -2.63f;

    public float objectsMinDistance = 6.0f;
    public float objectsMaxDistance = 12.0f;

    private float screenWidthInPoints;

	void Start () {
        float height = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;
	}

    void FixedUpdate()
    {
        GenerateRoomIfRequired();
        GenerateObjectsIfRequired();
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
        int randomIndex = Random.Range(0, availableObjects.Length);

        GameObject obj = (GameObject)Instantiate(availableObjects[randomIndex]);

        float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
        float randomY = Random.Range(0, 2) == 1 ? objectsYdown : objectsYup;
        
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);
        objects.Add(obj);
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