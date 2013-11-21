using UnityEngine;
using System.Collections;

public class Table : MonoBehaviour {
	public Deck TableDeck;
	public List<Player> Players;
	private TurnCounter;
	
	//RuleSet
	private int HandSize = 5;
	private bool DrawFirst = true;
	
	void Start () {
		Players = new List<Player>();
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.F2)){
			NewGame();
		}
	}
	
	public void NewGame(/*default is 2v2*/){
		TurnCounter = 0;
		
		TableDeck = new Deck(2);
		TableDeck.Shuffle();
		
		//needs dynamic logic for player number and team number
		Players.Add(new Player("Player One", 1));
		Players.Add(new Player("Player Two", 2));
		Players.Add(new Player("Player Three", 1));
		Players.Add(new Player("Player Four", 2));
		
		Deal();
		
		//print players cards to test
		foreach(Player _play in Players){
			Debug.Log("" + _player.Name);
			foreach(Card _card in Hand){
				Debug.Log(_card.FaceValue());
			}
		}
	}
	public void Deal(){
		for(int cardNum = 0; cardNum < HandSize; cardNum++){
			foreach(Player _player in Players){
				//draw a card from the deck and give it to the player
				_player.TakeCard(TableDeck.Draw());//may need null ref check
			}
		}
	}
}

public class Player{
	public string Name;
	public int Team;
	public int Counting;
	public List<Card> Hand;
	private int SelectedCard;
	public List<Card> Discard;
	public Card LastDiscarded;
	//board skin
	//pegs skin
	public Player(){
		string Name = "";
		int Team = -1;
		int Counting = 0;
		List<Card> Hand = new List<Card>();
		int SelectedCard = null;
		List<Card> Discard = new List<Card>();
		Card LastDiscarded = null;
	}
	public Player(string _name, int _team){
		string Name = _name;
		int Team = _team;
		int Counting = 0;
		List<Card> Hand = new List<Card>();
		int SelectedCard = null;
		List<Card> Discard = new List<Card>();
		Card LastDiscarded = null;
	}
	public void TakeCard(Card _card){
		Hand.Add(_card);
	}
}
