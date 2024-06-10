using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private enum ERoomTypes
    {
        empty = -1,
        normal,
        start
    }

    private const int MAP_WIDTH = 9;
    private const int MAP_HIGHT = 8;
    
    private ERoomTypes[,] map;

    private void Awake()
    {
        GenerateMap(1);
        Debug.LogWarning(LogMap(map));
    }

    private void GenerateMap(int _level)
    {
        map = new ERoomTypes[MAP_WIDTH, MAP_HIGHT];

        for (int y = 0; y < MAP_HIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                map[x, y] = ERoomTypes.empty;
            }
        }

        int neededRoomCount = Random.Range(0, 2) + 5 + (int)(_level * 2.6f);

        int maxIterations = 3000;
        int currIteractionCount = 0;

        bool isMapValid = false;
        while (!isMapValid && currIteractionCount < maxIterations)
        {
            //Generate new Level
            ERoomTypes[,] mapToValidate = GenerateLevelToValidate(map, neededRoomCount);

            //Validate new Level
            isMapValid = ValidateMap(mapToValidate, neededRoomCount);

            if (isMapValid)
            {
                map = mapToValidate;
            }

            currIteractionCount++;
        }

        if (currIteractionCount >= maxIterations)
            Debug.LogError("MAX ITERATIONS REACHED: BUG DETECTED!");
    }

    private ERoomTypes[,] GenerateLevelToValidate(ERoomTypes[,] _baseLevel, int _numRoomsToGenerate)
    {
        Queue<Vector2Int> positionsToExpand = new Queue<Vector2Int>();

        Vector2Int midPos = new Vector2Int(MAP_WIDTH / 2, MAP_HIGHT / 2);
        _baseLevel[midPos.x, midPos.y] = ERoomTypes.start;

        positionsToExpand.Enqueue(midPos);
        int currRoomCount = 1;

        while (positionsToExpand.Count > 0)
        {
            Vector2Int currPosToExpand = positionsToExpand.Dequeue();

            Vector2Int[] positionsToCheck = new Vector2Int[]
            {
                currPosToExpand + Vector2Int.up,
                currPosToExpand + Vector2Int.right,
                currPosToExpand + Vector2Int.down,
                currPosToExpand + Vector2Int.left
            };

            for (int checkPosIdx = 0; checkPosIdx < positionsToCheck.Length; checkPosIdx++)
            {
                Vector2Int toCheck = positionsToCheck[checkPosIdx];

                //Check if in Bounds
                if(toCheck.x >= 0 && toCheck.x < MAP_WIDTH && toCheck.y >= 0 && toCheck.y < MAP_HIGHT)
                {
                    //Check die Regeln:
                    //REGEL 1: Wenn genug Räume erzeugt wurden -> continue to next Room
                    if (currRoomCount >= _numRoomsToGenerate)
                        continue;

                    //REGEL 2: Wenn an der Stelle schon ein Raum ist -> continue to next Room
                    if (_baseLevel[toCheck.x, toCheck.y] != ERoomTypes.empty)
                        continue;

                    //REGEL 3: 50% Chance -> continue to next Room
                    float rndPercent = Random.Range(0f, 1f);
                    if(rndPercent <= 0.5f)
                        continue;

                    //REGEL 4: Wenn mehr als 1 Nachbar vorhanden ist an der Position -> continue to next Room
                    int neighbourCount = GetNeighbourCount(_baseLevel, toCheck);
                    if (neighbourCount > 1)
                        continue;

                    //ANSONSTEN: Generiere den Raum
                    _baseLevel[toCheck.x, toCheck.y] = ERoomTypes.normal;
                    positionsToExpand.Enqueue(toCheck);
                    currRoomCount++;
                }
            }
        }

        return _baseLevel;
    }

    private bool ValidateMap(ERoomTypes[,] _levelToValidate, int _neededRoomCount)
    {
        int count = 0;

        for (int y = 0; y < MAP_HIGHT; y++)
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                if (_levelToValidate[x, y] != ERoomTypes.empty)
                    count++;
            }
        }

        return count >= _neededRoomCount;
    }

    private int GetNeighbourCount(ERoomTypes[,] _level, Vector2Int _positionToCheck)
    {
        int count = 0;

        Vector2Int[] positionsToCheck = new Vector2Int[]
        {
                _positionToCheck + Vector2Int.up,
                _positionToCheck + Vector2Int.right,
                _positionToCheck + Vector2Int.down,
                _positionToCheck + Vector2Int.left
        };

        for (int neighbourPosIdx = 0; neighbourPosIdx < positionsToCheck.Length; neighbourPosIdx++)
        {
            Vector2Int toCheck = positionsToCheck[neighbourPosIdx];

            //Check if in Bounds
            if (toCheck.x >= 0 && toCheck.x < MAP_WIDTH && toCheck.y >= 0 && toCheck.y < MAP_HIGHT)
            {
                if (_level[toCheck.x, toCheck.y] != ERoomTypes.empty)
                    count++;
            }
        }

        return count;
    }

    private string LogMap(ERoomTypes[,] _map)
    {
        int mapWidth = _map.GetLength(0);
        int mapHeight = _map.GetLength(1);

        string output = "";

        for (int y = 0; y < mapHeight; y++)
        {
            output += "|";
            for (int x = 0; x < mapWidth; x++)
            {
                int num = (int)_map[x, y];

                if (num >= 0)
                    output += " ";

                output += num + "|";
            }
            output += "\n";
        }

        return output;
    }
}
