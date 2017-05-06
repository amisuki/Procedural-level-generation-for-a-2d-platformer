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
    public class DiceRull
    {

        public static bool isLeft(int dice)
        {
            if (dice == 1 || dice == 2)
            {
                return true;
            }
            return false;
        }

        public static bool isRight(int dice)
        {
            if (dice == 3 || dice == 4)
            {
                return true;
            }
            return false;
        }

        public static bool isTop(int dice)
        {
            if (dice == 7 || dice == 8)
            {
                return true;
            }
            return false;
        }

        public static bool isDown(int dice)
        {
            if (dice == 5 || dice == 6)
            {
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
	public class GridMap
	{
        public enum eRoomType
        {
            LeftRight = 1,
            Down = 2,
            Top = 3,
        }

		Size m_MapSize;
        public Size mapSize { get { return m_MapSize; } }

        Size roomSize;

        public List<Pointer> pathLoad = new List<Pointer>();

        public Room[,] m_Rooms;
        public Room[,] rooms { get { return m_Rooms; } }


        Pointer m_Entrance;
        public Pointer entrance { get { return m_Entrance; } }

		Pointer m_Exit;
        public Pointer exit { get { return m_Exit; } }

        KindDice pathDice  = new KindDice(1, 8);


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

		public void SetRoomType (Pointer index, eRoomType type)
		{
            if ((index.x < 0 || index.x >= m_MapSize.x) || ((index.y < 0 || index.y >= m_MapSize.y)))
            {
                Debug.Log("error index : " + index.x + " : " + index.y);
                Debug.DebugBreak();
            }

            pathLoad.Add(new Pointer(index.x, index.y));
            Debug.Log(string.Format("x = {0}, y= {1}", index.x, index.y));
            m_Rooms [index.x, index.y].type = (int)type;
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
			index.y = UnityEngine.Random.Range(0, (int)m_MapSize.y);
			m_Entrance = index;
		}

		public void GeneratorPath()
		{
			Pointer iterator = m_Entrance;
            SetRoomType(iterator, eRoomType.LeftRight );

            pathLoad.Clear();

            int dice = pathDice.GetRandom ();
			iterator = CalcuDiceRull(iterator, dice);
            int deadCount = 120;
            int Counting = 15;
            while (iterator != m_Exit && deadCount >= 0)
            {
                dice = pathDice.GetRandom();
                Pointer temp = iterator;
                iterator = CalcuDiceRull(iterator, dice);
                //yield return new WaitForSeconds(0.3f);
                --deadCount;
                if( deadCount <= 0)
                    m_Exit = iterator;

                if (temp != iterator)
                    Counting--;


                if (Counting <= 0)
                    m_Exit = iterator;
            }

            int distnace = Mathf.Abs((entrance.x + entrance.y) - (m_Exit.x + m_Exit.y));
            if (distnace < 4)
            {
                m_Rooms = new Room[m_MapSize.x, m_MapSize.y];
                for (int x = 0; x < m_MapSize.x; ++x)
                {
                    for (int y = 0; y < m_MapSize.y; ++y)
                    {
                        m_Rooms[x, y] = new Room();
                    }
                }
                GeneratorPath();
            }

            //yield return null;
		}

        bool isValid(Pointer it)
        {
            if(it.x < 0 && it.x < mapSize.x)
            {
                if (it.y < 0 && it.y < mapSize.y)
                {
                    return true;
                }
            }

            return false;
        }

        bool LeftSolid(int dice, Pointer temp)
        {
            if (DiceRull.isLeft(dice))
            {
                temp.x--;
                if (temp.x <= 0)
                {
                    return false;
                }
                if (GetRoomType(temp) == 0)
                    return true;
            }
            return false;
        }


        bool RightSolid(int dice, Pointer temp)
        {
            if (DiceRull.isRight(dice))
            {
                temp.x++;
                if (temp.x >= m_MapSize.x)
                {
                    return false;
                }
                if (GetRoomType(temp) == 0)
                    return true;
            }
            return false;
        }

        bool TopSolid(int dice, Pointer temp)
        {
            if (DiceRull.isTop(dice))
            {
                temp.y++;
                if (temp.y >= m_MapSize.y)
                {
                    return false;
                }
                if (GetRoomType(temp) == 0)
                    return true;
            }
            return false;
        }

        bool DownSolid(int dice, Pointer temp)
        {
            if (DiceRull.isDown(dice))
            {
                temp.y--;
                if (temp.y <= 0)
                {
                    return false;
                }
                if (GetRoomType(temp) == 0)
                    return true;
            }
            return false;
        }

        Pointer CalcuDiceRull(Pointer index, int dice)
		{
            if (DiceRull.isDown(dice) && DownSolid(dice, index) == false)
                return index;
            if (DiceRull.isTop(dice) && TopSolid(dice, index) == false)
                return index;
            if (DiceRull.isRight(dice) && RightSolid(dice, index) == false)
                return index;
            if (DiceRull.isLeft(dice) && LeftSolid(dice, index) == false)
                return index;

            //if (LeftNotSolid(dice, index) || RightNotSolid(dice, index)
            //    || DownNotSolid(dice, index) || TopNotSolid(dice, index))
            //    return index;

            Pointer nextRoom = index;

            if (DiceRull.isDown(dice))
            {
                SetRoomType(nextRoom, eRoomType.Down);
                nextRoom.y--;
                SetRoomType(nextRoom, eRoomType.Top);

                return nextRoom;
            }

            else if (DiceRull.isTop(dice))
            {
                SetRoomType(nextRoom, eRoomType.Top);
                nextRoom.y++;
                SetRoomType(nextRoom, eRoomType.Down);

                return nextRoom;
            }
            else if (DiceRull.isLeft(dice))
            {
                nextRoom.x--;
                SetRoomType(nextRoom, eRoomType.LeftRight);
            }
            else if (DiceRull.isRight(dice))
            {
                nextRoom.x++;
                SetRoomType(nextRoom, eRoomType.LeftRight);
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
        public Text DebugText;

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

            StartCoroutine(SequenceGUI());
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
                    if(m_MapData.entrance.x == x && m_MapData.entrance.y == y)
                    {
                        line += "S";
                    }
                    else if (m_MapData.exit.x == x && m_MapData.exit.y == y)
                    {
                        line += "E";
                    }
                    else
                        line += m_MapData.m_Rooms[x, y].type.ToString();
                }
                text.text += line + "\n";
            }

            text.text += "\nexit = x : " + m_MapData.exit.x + "  y : " + m_MapData.exit.y + "\n";
        }

        IEnumerator SequenceGUI()
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < m_MapData.pathLoad.Count; ++i)
            {
                //DebugGUI(m_MapData.pathLoad[i].x, m_MapData.pathLoad[i].y);
                yield return new WaitForSeconds(0.3f);
            }
            
        }

        void DebugGUI(int xp, int yp)
        {
            string line = DebugText.text;
            for (int y = 0; y < MapSizeY; ++y)
            {
                for (int x = 0; x < MapSizeX; ++x)
                {
                    //if (y == yp && x == xp)
                        //line.Rep = m_MapData.m_Rooms[x, y].type.ToString()[0];
                }
                DebugText.text += line + "\n";
            }
        }
    }

}
