using UnityEngine;
using System.Collections;

public class Table : MonoBehaviour {
	public Deck tableDeck;
	
	// Use this for initialization
	void Start () {
		tableDeck = new Deck(2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
