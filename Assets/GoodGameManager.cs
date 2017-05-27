using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodGameManager : MonoBehaviour {

    Platformer.PathGenerator generator;
    Platformer.LevelMaker levelMaker;

	void Start ()
    {
        generator = GetComponent<Platformer.PathGenerator>();
        levelMaker = GetComponent<Platformer.LevelMaker>();
    }

    public void MakePath()
    {
        generator.ActionGenerator();
        generator.DrawGUI();
		DrawTiles ();
    }

    public void DrawTiles()
    {
        levelMaker.Create(generator.mapData);
    }

    void Update ()
    {
		
	}
}
