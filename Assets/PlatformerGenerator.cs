using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
	// for typedef
	public struct Size
	{
		public int x;
		public int y;
	}

	// for typedef
	public struct Pointer
	{
		public int x;
		public int y;

        public Pointer(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Pointer lh, Pointer rh)
		{
			return (lh.x == rh.x && lh.y == rh.y);
		}

		public static bool operator ==(Pointer c1, Pointer c2) 
		{
			return c1.Equals(c2);
		}

		public static bool operator !=(Pointer c1, Pointer c2) 
		{
			return !c1.Equals(c2);
		}
	}

	[System.Serializable]
	public class KindDice
	{
		public int min;
		public int max;

		public KindDice(int rMin, int rMax)
		{
			min = rMin;
			max = rMax;
		}

		public int GetRandom()
		{
			return UnityEngine.Random.Range (min, max);
		}
	}

	[System.Serializable]
	public class GridMap
	{
		//using GenList = System.Collections.Generic.List<int>;

		//using MyDict = Dictionary<string, string>;
		//using PointerI = Pointer<uint>;

		Size mapSize;
		Size roomSize;

		public Room[,] rooms;

		Pointer entrance;
		public Pointer exit;

		KindDice pathDice  = new KindDice(1, 6);
		KindDice roomDice  = new KindDice(0, 4);

		[System.Serializable]
		public class Room 
		{
			public int type = 0;
		}

		public void CreateMap(int mapSizeX, int mapSizeY, int roomSizeX, int roomSizeY)
		{
			mapSize.x = mapSizeX;
			mapSize.y = mapSizeY;

			roomSize.x = roomSizeX;
			roomSize.y = roomSizeY;

            entrance.x = -1; entrance.y = -1;
            exit.x = -1; exit.y = -1;
            

            rooms = new Room[mapSize.x, mapSize.y];
			for (int x = 0; x < mapSize.x; ++x) {
				for (int y = 0; y < mapSize.y; ++y) {
					rooms [x, y] = new Room ();
				}
			}
		}

		public void SetRoomType (Pointer index, int type)
		{
            if ((index.x < 0 || index.x >= mapSize.x) || ((index.y < 0 || index.y >= mapSize.y)))
            {
                Debug.Log("error index : " + index.x + " : " + index.y);
                Debug.DebugBreak();
            }
            Debug.Log("index : " + index.x + " : " + index.y + "   : " + type);
            rooms [index.x, index.y].type = type;
		}

        public int GetRoomType(Pointer index)
        {
            if ((index.x < 0 || index.x >= mapSize.x) || ((index.y < 0 || index.y >= mapSize.y)))
            {
                Debug.Log("error index : " + index.x + " : " + index.y);
                Debug.DebugBreak();
            }
                

            return rooms[index.x, index.y].type;
        }

        public void CreateEntrance()
		{
			Pointer index	= new Pointer();
			index.x = UnityEngine.Random.Range (0, (int)mapSize.x);
			index.y = 0;

			SetRoomType (index, 1);

			entrance = index;
		}

		public void GeneratorPath()
		//public IEnumerator GeneratorPath()
		{
			
			Pointer iterator = entrance;
            SetRoomType(iterator, 1);

			int dice = pathDice.GetRandom ();
			iterator = DiceRull (iterator, dice);
            int deadCount = 50;
            while (iterator != exit && deadCount >= 0)
            {
                dice = pathDice.GetRandom();
                iterator = DiceRull(iterator, dice);
                //yield return new WaitForSeconds(0.3f);
                --deadCount;
            }

            //yield return null;
		}

        bool LeftNotSolid(int dice, Pointer temp)
        {
            if (dice == 1 || dice == 2)
            {
                temp.x--;
                if (temp.x <= 0)
                {
                    temp.x = 0;
                }
                if (GetRoomType(temp) != 0)
                    return true;
            }
            return false;
        }

        bool RightNotSolid(int dice, Pointer temp)
        {
            if (dice == 3 || dice == 4)
            {
                temp.x++;
                if (temp.x >= mapSize.x - 1)
                {
                    temp.x = mapSize.x - 1;
                }
                if (GetRoomType(temp) != 0)
                    return true;
            }
            return false;
        }

        Pointer DiceRull(Pointer index, int dice)
		{
            if (LeftNotSolid(dice, index) || RightNotSolid(dice, index))
                return index;


            Pointer nextRoom = index;
            if (dice == 5)
            {
                SetRoomType(nextRoom, 2);
                nextRoom.y++;
                if(nextRoom.y >= mapSize.y)
                {
                    nextRoom.y = mapSize.y - 1;
                    exit = nextRoom;
                    SetRoomType(nextRoom, 1);
                }
                else
                    SetRoomType(nextRoom, 3);

                return nextRoom;
            }
            else if (dice == 1 || dice == 2)
            {
                nextRoom.x--;
                if (nextRoom.x <= 0)
                {
                    nextRoom.x = 0;
                    SetRoomType(nextRoom, 2);
                    nextRoom.y++;
                    if (nextRoom.y >= mapSize.y)
                    {
                        nextRoom.y = mapSize.y - 1;
                        exit = nextRoom;
                        SetRoomType(nextRoom, 1);
                    }
                    else
                        SetRoomType(nextRoom, 3);
                }
                else
                {
                    SetRoomType(nextRoom, 1);
                }
            }
            else if (dice == 3 || dice == 4)
            {
                nextRoom.x++;
                if (nextRoom.x >= mapSize.x - 1)
                {
                    nextRoom.x = mapSize.x - 1;
                    SetRoomType(nextRoom, 2);
                    nextRoom.y++;
                    if (nextRoom.y >= mapSize.y)
                    {
                        nextRoom.y = mapSize.y - 1;
                        exit = nextRoom;
                        SetRoomType(nextRoom, 1);
                    }
                    else
                        SetRoomType(nextRoom, 3);
                }
                else
                {
                    SetRoomType(nextRoom, 1);
                }
            }

            return nextRoom;
		}
	}


	[System.Serializable]
	public class PlatformerGenerator : MonoBehaviour 
	{

		public int MapSizeX;
		public int MapSizeY;

		public int RoomSizeX;
		public int RoomSizeY;

		public GridMap mapData;

		public Text text;

		void Start () 
		{
			ActionGenerator ();
		}

		public void ActionGenerator () 
		{
            text.text = "";

            mapData.CreateMap (MapSizeX, MapSizeY, RoomSizeX, RoomSizeY);
			mapData.CreateEntrance ();

            //StartCoroutine (mapData.GeneratorPath ());
            mapData.GeneratorPath();


            for (int y = 0; y < MapSizeY; ++y) {
				string line = "";
                for (int x = 0; x < MapSizeX; ++x) {
                    line += mapData.rooms [x, y].type.ToString ();
				}
				text.text += line + "\n";
			}

            text.text += "x : " + mapData.exit.x + "  y : " + mapData.exit.y + "\n";


        }
	}

}
