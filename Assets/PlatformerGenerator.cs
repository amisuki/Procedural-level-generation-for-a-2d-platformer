using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
	// for typedef
	public struct Size
	{
		public uint x;
		public uint y;
	}

	// for typedef
	public struct Pointer
	{
		public uint x;
		public uint y;

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
		Pointer exit;

		KindDice pathDice  = new KindDice(0, 6);
		KindDice roomDice  = new KindDice(0, 4);

		[System.Serializable]
		public class Room 
		{
			public int type = 0;
		}

		public void CreateMap(uint mapSizeX, uint mapSizeY, uint roomSizeX, uint roomSizeY)
		{
			mapSize.x = mapSizeX;
			mapSize.y = mapSizeY;

			roomSize.x = roomSizeX;
			roomSize.y = roomSizeY;

			rooms = new Room[mapSize.x, mapSize.y];
			for (int x = 0; x < mapSize.x; ++x) {
				for (int y = 0; y < mapSize.y; ++y) {
					rooms [x, y] = new Room ();
				}
			}
		}

		public void SetRoomType (Pointer index, int type)
		{
			rooms [index.x, index.y].type = type;
		}

		public void CreateEntrance()
		{
			Pointer index	= new Pointer();
			index.x = (uint)UnityEngine.Random.Range (0, (int)mapSize.x);
			index.y = 0;

			SetRoomType (index, 1);

			entrance = index;
		}

		//public void GeneratorPath()
		public IEnumerator GeneratorPath()
		{
			
			Pointer iterator = entrance;

			int dice = pathDice.GetRandom ();
			iterator = DiceRull (iterator, dice);
//			while (iterator != exit) {
//				iterator = DiceRull (iterator, dice);
//				yield return new WaitForSeconds (0.3f);
//			}

			iterator = DiceRull (iterator, dice);
			iterator = DiceRull (iterator, dice);
			iterator = DiceRull (iterator, dice);
			iterator = DiceRull (iterator, dice);
			iterator = DiceRull (iterator, dice);

			yield return null;
		}

		Pointer DiceRull(Pointer index, int dice)
		{
			Pointer nextRoom = index;

			//entrance logic
			if (index == entrance) {
				if (dice == 5) {
					SetRoomType (entrance, 2);
					nextRoom.y++;
					SetRoomType (nextRoom, 3);
					return nextRoom;
				} else if (dice == 1 || dice == 2) {
					if (nextRoom.x - 1 == 0) {
						SetRoomType (entrance, 2);
					}
					nextRoom.x--;

				} else if (dice == 3 || dice == 4) {
					if (nextRoom.x + 1 == mapSize.x - 1) {
						SetRoomType (entrance, 2);
					}
					nextRoom.x++;
				}

				if (nextRoom.x == 0 || nextRoom.x == mapSize.x - 1) {
					
				}
			}
			//find exit logic
			else if (index.y == mapSize.y - 1) {
				if (dice == 1 || dice == 2) {
					nextRoom.x--;
				} else if (dice == 3 || dice == 4) {
					nextRoom.x++;
				} else if (dice == 5) {
					nextRoom = exit = index;
					SetRoomType (exit, 1);
					return nextRoom;
				}
			} else {
				if (dice == 1 || dice == 2) {
					nextRoom.x--;
				} else if (dice == 3 || dice == 4) {
					nextRoom.x++;
				} else if (dice == 5) {
					nextRoom.y++;
				}
			}

			return nextRoom;
		}
	}


	[System.Serializable]
	public class PlatformerGenerator : MonoBehaviour 
	{

		public uint MapSizeX;
		public uint MapSizeY;

		public uint RoomSizeX;
		public uint RoomSizeY;

		public GridMap mapData;

		public Text text;

		void Start () 
		{
			ActionGenerator ();
		}

		void ActionGenerator () 
		{
			mapData.CreateMap (MapSizeX, MapSizeY, RoomSizeX, RoomSizeY);
			mapData.CreateEntrance ();

			StartCoroutine (mapData.GeneratorPath ());


			for (int x = 0; x < MapSizeX; ++x) {
				string line = "";
				for (int y = 0; y < MapSizeY; ++y) {
					line += mapData.rooms [x, y].type.ToString ();
				}
				text.text += line + "\n";
			}

		}
	}

}
