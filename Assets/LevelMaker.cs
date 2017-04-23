using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class LevelMaker : MonoBehaviour
    {
        //10, 8
        public Size tileRoomSize;
        public int[,] solid;

        [TextArea]
        public string[] roomtype0;
        [TextArea]
        public string[] roomtype1;
        [TextArea]
        public string[] roomtype2;
        [TextArea]
        public string[] roomtype3;

        public GameObject Go1;
        public GameObject GoL;
        public GameObject GoP;


        void Start()
        {

        }


        public void Create(GridMap map)
        {
            GridMap.Room[,] rooms = map.rooms;

            int lX = rooms.GetLength(0);
            int lY = rooms.GetLength(1);

            for(int y = lY - 1; y <= lY; --y)
            {
                for (int x = lX - 1; x <= lX; --x)
                {
                    string data = GetRoomData(rooms[x, y].type);
                    FillOneRoom(data, x, y, map.mapSize.x, map.mapSize.y);
                }
            }
        }

        public void FillOneRoom(string data, int x, int y, int width, int height)
        {
            //float spriteSize = 0.64f;
            for(int i = 0; i < data.Length; ++i)
            {
                int line = (int)((float)i / (float)width);
                int index = x * line + x;
                char d = data[i];
                if(d == '1')
                {
                    
                    continue;
                }
            }
            
        }

        public string GetRoomData( int type )
        {
            int index = 0;
            string data = "";

            if (type == 1)
            {
                index = Random.Range(0, roomtype1.Length);
                data = roomtype1[index];
            }
            else if (type == 2)
            {
                index = Random.Range(0, roomtype2.Length);
                data = roomtype2[index];
            }
            else if (type == 3)
            {
                index = Random.Range(0, roomtype3.Length);
                data = roomtype3[index];
            }
            else
            {
                index = Random.Range(0, roomtype0.Length);
                data = roomtype0[index];
            }

            return data;
        }
    }

}