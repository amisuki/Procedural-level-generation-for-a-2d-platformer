using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Platformer
{
    public class LevelMaker : MonoBehaviour
    {
        //10, 8
        public Size tileRoomSize;
        public int[,] solid;

		public int width = 10;
		public int height = 8;

        [TextArea]
        public string[] roomtype0;
        [TextArea]
        public string[] roomtype1;
        [TextArea]
        public string[] roomtype2;
        [TextArea]
        public string[] roomtype3;
        [TextArea]
        public string[] roomtype4;
        [TextArea]
        public string[] roomtype5;
        [TextArea]
        public string[] roomtypeCross;

        public GameObject Go1;
        public GameObject GoL;
        public GameObject GoP;

		static void NewMethod (string aa)
		{
			string bb = "";
			for (int i = 0; i < aa.Length; ++i) {
				if (aa [i] != ',')
					bb += aa [i].ToString ();
			}
			Debug.Log (bb);
		}

        void Start()
        {
			
        }


        public void Create(GridMap map)
        {
            GridMap.Room[,] rooms = map.rooms;

            int lX = rooms.GetLength(0);
            int lY = rooms.GetLength(1);
            for(int y = lY - 1; y >= 0; --y)
			//for(int y = 0; y < lY; ++y)
            {
				for (int x = 0; x < lX; ++x)
                {
					//if (x == 1 && y == lY - 2) {
                        string data = GetRoomData (rooms [x, y].type);
                        FillOneRoom (data, x, Mathf.Abs((y + 1) - lY), rooms [x, y].type, width, height, 4, 4);
					//}
                }
            }
        }

		public void FillOneRoom(string data, int parentX, int parentY, int type, int width, int height, int mapSizeX, int mapSizeY)
        {
            //Debug.Log ("parentX : " + parentX + "   parentY : " + parentY + "  type : " + type + " width : " + width + " height : " + height);

            string newdata = string.Join(" ", Regex.Split(data, @"(?:\r\n|\n|\r)"));
            //olddata.Replace("\\n", "");
            Debug.Log(newdata);
            //newdata = olddata;

            int j = 0;
            int y = height - 1;

            float originX = parentX * width;
			float originY = parentY * height;

            //float spriteSize = 0.64f;
            for (int i = 0; i < newdata.Length; ++i)
            {
                char d = newdata[i];
				if(d == ' ')
                {
                    //if (j % width == 0)
                    //{
                        y--;
                    //}
                    continue;
                }

				int YIndex = y;
				int XIndex = j % width;
				j++;


				float pointX = (float)(XIndex + originX) * 0.64f;
				float pointY = (float)(YIndex + originY) * 0.64f;

				//Debug.Log ("X : " + (XIndex + originX) +" Y : " + (YIndex + originY) + " = " + d + " [pointX]=" + pointX + " [pointY]=" + pointY);

                if(d == '1') {
					//GameObject go = 
					Instantiate (Go1,new Vector3(pointX, pointY, 0f), Quaternion.identity);

                }

				if(d == 'P') {
					//GameObject go = 
					Instantiate (GoP,new Vector3(pointX, pointY, 0f), Quaternion.identity);


				}

				if(d == 'L') {
					//GameObject go = 
					Instantiate (GoL,new Vector3(pointX, pointY, 0f), Quaternion.identity);


				}

				if(d == '4') {
					if(Random.Range(0, 100) <= 40)
						Instantiate (GoL,new Vector3(pointX, pointY, 0f), Quaternion.identity);

				}
				if(d == '5') {

					//ObstacleBlocks (newX, newY, false);

				}
				if(d == '6') {

					//ObstacleBlocks (newX, newY, true);
				}

            }
        }

		public void ObstacleBlocks(int x, int y, bool isAir)
		{
			
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
            else if (type == 4)
            {
                index = Random.Range(0, roomtype4.Length);
                data = roomtype4[index];
            }
            else if (type == 5)
            {
                index = Random.Range(0, roomtype5.Length);
                data = roomtype5[index];
            }
            else if (type == 6)
            {
                index = Random.Range(0, roomtypeCross.Length);
                data = roomtypeCross[index];
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