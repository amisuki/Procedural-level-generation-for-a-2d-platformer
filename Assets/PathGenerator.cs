using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    // for typedef
    [System.Serializable]
    public struct Size
	{
		public int x;
		public int y;
	}

    // for typedef
    [System.Serializable]
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
		Size m_MapSize;
        public Size mapSize { get { return m_MapSize; } }

        Size roomSize;

		public Room[,] m_Rooms;
        public Room[,] rooms { get { return m_Rooms; } }


        Pointer m_Entrance;
        public Pointer entrance { get { return m_Entrance; } }

		Pointer m_Exit;
        public Pointer exit { get { return m_Exit; } }

        KindDice pathDice  = new KindDice(1, 6);
		KindDice roomDice  = new KindDice(0, 4);

		[System.Serializable]
		public class Room 
		{
			public int type = 0;
		}

		public void CreateMap(int mapSizeX, int mapSizeY, int roomSizeX, int roomSizeY)
		{
			m_MapSize.x = mapSizeX;
			m_MapSize.y = mapSizeY;

			roomSize.x = roomSizeX;
			roomSize.y = roomSizeY;

            m_Entrance.x = -1; m_Entrance.y = -1;
            m_Exit.x = -1; m_Exit.y = -1;
            

            m_Rooms = new Room[m_MapSize.x, m_MapSize.y];
			for (int x = 0; x < m_MapSize.x; ++x) {
				for (int y = 0; y < m_MapSize.y; ++y) {
					m_Rooms [x, y] = new Room ();
				}
			}
		}

		public void SetRoomType (Pointer index, int type)
		{
            if ((index.x < 0 || index.x >= m_MapSize.x) || ((index.y < 0 || index.y >= m_MapSize.y)))
            {
                Debug.Log("error index : " + index.x + " : " + index.y);
                Debug.DebugBreak();
            }
            
            m_Rooms [index.x, index.y].type = type;
		}

        public int GetRoomType(Pointer index)
        {
            if ((index.x < 0 || index.x >= m_MapSize.x) || ((index.y < 0 || index.y >= m_MapSize.y)))
            {
                Debug.Log("error index : " + index.x + " : " + index.y);
                Debug.DebugBreak();
            }
                

            return m_Rooms[index.x, index.y].type;
        }

        public void CreateEntrance()
		{
			Pointer index	= new Pointer();
			index.x = UnityEngine.Random.Range (0, (int)m_MapSize.x);
			index.y = 0;

			SetRoomType (index, 1);

			m_Entrance = index;
		}

		public void GeneratorPath()
		//public IEnumerator GeneratorPath()
		{
			
			Pointer iterator = m_Entrance;
            SetRoomType(iterator, 1);

			int dice = pathDice.GetRandom ();
			iterator = DiceRull (iterator, dice);
            int deadCount = 50;
            while (iterator != m_Exit && deadCount >= 0)
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
                if (temp.x >= m_MapSize.x - 1)
                {
                    temp.x = m_MapSize.x - 1;
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
                if(nextRoom.y >= m_MapSize.y)
                {
                    nextRoom.y = m_MapSize.y - 1;
                    m_Exit = nextRoom;
					SetRoomType(nextRoom, 3);
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
                    if (nextRoom.y >= m_MapSize.y)
                    {
                        nextRoom.y = m_MapSize.y - 1;
                        m_Exit = nextRoom;
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
                if (nextRoom.x >= m_MapSize.x - 1)
                {
                    nextRoom.x = m_MapSize.x - 1;
                    SetRoomType(nextRoom, 2);
                    nextRoom.y++;
                    if (nextRoom.y >= m_MapSize.y)
                    {
                        nextRoom.y = m_MapSize.y - 1;
                        m_Exit = nextRoom;
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
	public class PathGenerator : MonoBehaviour 
	{

		public int MapSizeX;
		public int MapSizeY;

		public int RoomSizeX;
		public int RoomSizeY;

		GridMap m_MapData;
        public GridMap mapData
        {
            get { return m_MapData; }
        }

        public Text text;

        void Awake () 
		{
            m_MapData = new GridMap();

        }

		public void ActionGenerator () 
		{
            text.text = "";

            m_MapData.CreateMap (MapSizeX, MapSizeY, RoomSizeX, RoomSizeY);
			m_MapData.CreateEntrance ();

            //StartCoroutine (mapData.GeneratorPath ());
            m_MapData.GeneratorPath();
        }

        public void DrawGUI()
        {
            if (text == null)
                return;

            text.text += "entrance = x : " + m_MapData.entrance.x + "  y : " + m_MapData.entrance.y + "\n\n";

            for (int y = 0; y < MapSizeY; ++y)
            {
                string line = "";
                for (int x = 0; x < MapSizeX; ++x)
                {
                    line += m_MapData.m_Rooms[x, y].type.ToString();
                }
                text.text += line + "\n";
            }

            text.text += "\nexit = x : " + m_MapData.exit.x + "  y : " + m_MapData.exit.y + "\n";
        }
	}

}
