using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Table : MonoBehaviour {
	
	public enum PHASE { DRAW, PLAY, MOVE, END }
	public enum RULESET { CLASSIC }
	
	public float CARDWIDTH = 100f;
	public float CARDHEIGHT = 50f;
	public float CARDBUFFER = 10f;
	
	public Board TableBoard;
	public Deck TableDeck;
	public List<Player> Players;
	public Card LastPlayed;
	
	private RULESET RuleSet;
	private int TurnCounter;
	private PHASE PhaseCounter;
	
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
		if(Input.GetKeyDown(KeyCode.F3)){
			NextTurn();
		}
	}
	
	void OnGUI(){
		if(Players.Count > 0){
			/*foreach(Player _player in Players){
				if(_player.Hand.Count > 0){
					GUILayout.BeginHorizontal("");
					foreach(Card _card in _player.Hand){
						if(GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* _player.Hand.IndexOf(_card), (CARDHEIGHT + CARDBUFFER) * Players.IndexOf(_player),CARDWIDTH,CARDHEIGHT),_card.FaceValue())){
							_player.SelectedCard = _player.Hand.IndexOf(_card);
						}
					}
					GUILayout.EndHorizontal();
				}
			}*/
			if(Players[TurnCounter].Hand.Count > 0){
				GUILayout.BeginHorizontal("");
				foreach(Card _card in Players[TurnCounter].Hand){
					if(GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* Players[TurnCounter].Hand.IndexOf(_card), CARDBUFFER,CARDWIDTH,CARDHEIGHT),_card.FaceValue())){
						Players[TurnCounter].SelectedCard = Players[TurnCounter].Hand.IndexOf(_card);
					}
				}
				GUILayout.EndHorizontal();
			}
			switch(PhaseCounter){
			case PHASE.DRAW:
				if(GUI.Button(new Rect((Camera.main.pixelWidth / 2) - CARDWIDTH / 2, Camera.main.pixelHeight / 2,CARDWIDTH,CARDHEIGHT),"Draw")){
					DrawPhase(Players[TurnCounter]);
				}
				break;
			case PHASE.PLAY:
				if(GUI.Button(new Rect((Camera.main.pixelWidth / 2) - CARDWIDTH - (CARDBUFFER / 2), Camera.main.pixelHeight / 2,CARDWIDTH,CARDHEIGHT),"Play")){
					PlayPhase(Players[TurnCounter]);
				}
				if(GUI.Button(new Rect((Camera.main.pixelWidth / 2) + (CARDBUFFER / 2), Camera.main.pixelHeight / 2,CARDWIDTH,CARDHEIGHT),"Discard")){
					DiscardPhase(Players[TurnCounter]);
				}
				break;
			case PHASE.MOVE:
				//display the available pegs
				//Players[TurnCounter].SelectedPeg = this Peg
				if(GUI.Button(new Rect((Camera.main.pixelWidth / 2) - CARDWIDTH / 2, Camera.main.pixelHeight / 2,CARDWIDTH,CARDHEIGHT),"Move")){
					MovePhase(Players[TurnCounter]);
				}
				break;
			case PHASE.END:
				if(GUI.Button(new Rect((Camera.main.pixelWidth / 2) - CARDWIDTH / 2, Camera.main.pixelHeight / 2,CARDWIDTH,CARDHEIGHT),"End Turn")){
					EndPhase(Players[TurnCounter]);
				}
				break;
			}
			if(LastPlayed != null){
				GUI.Label(new Rect(0,Camera.main.pixelHeight - CARDHEIGHT, CARDWIDTH, CARDHEIGHT),"Last Played\n" + LastPlayed.FaceValue());
			}
		}
	}
	//player's turn starts
	//player draws a card 
	//player selects a Card to play, or discards
	//player selects a Peg(s) to move
	//(player draws or Counts)
	//player ends turn
	public void DrawPhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.DRAW){
			return;
		}
		//player draws a card
		_player.TakeCard(TableDeck.Draw());
		//inc the PhaseCounter
		PhaseCounter = PHASE.PLAY;
	}
	public void PlayPhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.PLAY){
			return;
		}
		//discard selected card
		_player.Discard.Add(_player.LastDiscarded);
		_player.LastDiscarded = _player.Hand[_player.SelectedCard];
		LastPlayed = _player.Hand[_player.SelectedCard];
		_player.Hand.RemoveAt(_player.SelectedCard);
		//inc PhaseCounter
		PhaseCounter = PHASE.MOVE;
	}
	public void DiscardPhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.PLAY){
			return;
		}
		if(_player.SelectedCard == -1){
			Debug.Log("you must select a card");
			return;
		}
		//discard selected card
		_player.Discard.Add(_player.LastDiscarded);
		_player.LastDiscarded = _player.Hand[_player.SelectedCard];
		LastPlayed = _player.Hand[_player.SelectedCard];
		_player.Hand.RemoveAt(_player.SelectedCard);
		//Player draws or Counts Depending on rule Set
		switch(RuleSet){
		case RULESET.CLASSIC:
			//_player.TakeCard(TableDeck.Draw());
			_player.Counting++;
			if(_player.Counting == 3){
				//move peg out of player's Home to Come Out space;
				_player.Counting = 0;
			}
			break;
		}
		//inc PhaseCounter
		PhaseCounter = PHASE.END;
	}
	public void MovePhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.MOVE){
			return;
		}
		//move logic
		//if(!TestPegMovement()){
		//return;
		//}
		PhaseCounter = PHASE.END;
	}
	public bool TestPegMove(Peg _peg, int _distance){
		for(int i = 0; i < Players.Count; i++){
			//if on main track
			//check all pegs between starting position and ending position
			//if any are owned by the same player, return false
			//check if in Castle
			
			/*
			PlayerPegs[i,0]
			PlayerPegs[i,1] 
			PlayerPegs[i,2] 
			PlayerPegs[i,3] 
			PlayerPegs[i,4] 
			*/
		}
		return false;
	}
	public void EndPhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.END){
			return;
		}
		_player.SelectedCard = -1;
		LastPlayed = null;
		//roll PhaseCounter
		PhaseCounter = PHASE.DRAW;
		//roll TurnCounter
		TurnCounter = (TurnCounter + 1) % Players.Count;
	}
	
	public void NextTurn(){
		Players[TurnCounter].TakeCard(TableDeck.Draw());
		TurnCounter = (TurnCounter + 1) % Players.Count;
		
	}
	public void PlayCard(Player _player, int _cardNumber){
		_player.Discard.Add(_player.LastDiscarded);
		_player.LastDiscarded = _player.Hand[_cardNumber];
		_player.Hand.RemoveAt(_cardNumber);
		
		//next player draws
		//_player.TakeCard(TableDeck.Draw);
	}
	
	public void NewGame(/*default is 2v2*/){
		TurnCounter = 0;
		PhaseCounter = PHASE.DRAW;
		
		TableBoard = new Board(4);
		
		TableDeck = new Deck(2);
		TableDeck.Shuffle();
		
		Players = new List<Player>();
		//needs dynamic logic for player number and team number
		Players.Add(new Player("Player One", 1));
		Players.Add(new Player("Player Two", 2));
		Players.Add(new Player("Player Three", 1));
		Players.Add(new Player("Player Four", 2));
		
		Deal();
		
		//print players cards to test
		/*foreach(Player _player in Players){
			Debug.Log("" + _player.Name);
			foreach(Card _card in _player.Hand){
				Debug.Log(_card.FaceValue());
			}
		}*/
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
public enum LOCATION { MAINTRACK, HOME, CASTLE }
public class Peg{
	public int Team;
	public int Player;
	public LOCATION Location;
	public int Distance;
	public int BoardSegment;
	
	public Peg(int _team, int _player, LOCATION _location, int _distance){
		Team = _team;
		Player = _player;
		Location = _location;
		Distance = _distance;
	}
}
public class Board{
	/*
	public Transform[] MainTrackLocations;
	public Transform[] HomeLocations;
	public Transform[] CastleLocations;
	public int[] PegLocations;
	//0-17 + (18 * player) Main Track
	//0-4 + (5 * player) Home Track
	//0-4 + (5 * player) Castle Locations
	
	public Board(int _players){
		MainTrackLocations = new Transform[18 * _players];
		HomeLocations = new Transform[5 * _players];
		CastleLocations = new Transform[5 * _players];
		PegLocations = new int[5 * _players];
		for(int i = 0; i < PegLocations.Length; i++){
			PegLocations[i] = i;
		}
	}*/
	public Peg[,] PlayersPegs;
	
	public Board(int _players){
		PlayersPegs = new Pegs[_players, 5]();
		for(int i = 0; i < _players; i++){
				PlayerPegs[i,0] = new Peg(Players[i].Team, i, LOCATION.HOME, 0);
				PlayerPegs[i,1] = new Peg(Players[i].Team, i, LOCATION.HOME, 1);
				PlayerPegs[i,2] = new Peg(Players[i].Team, i, LOCATION.HOME, 2);
				PlayerPegs[i,3] = new Peg(Players[i].Team, i, LOCATION.HOME, 3);
				PlayerPegs[i,4] = new Peg(Players[i].Team, i, LOCATION.HOME, 4);
		}
	}
}

public class Player{
	public string Name;
	public int Team;
	public int Counting;
	public List<Card> Hand;
	public int SelectedCard;
	public Peg SelectedPeg;
	public List<Card> Discard;
	public Card LastDiscarded;
	//board skin
	//pegs skin
	public Player(){
		Name = "";
		Team = -1;
		Counting = 0;
		Hand = new List<Card>();
		SelectedCard = -1;
		Discard = new List<Card>();
		LastDiscarded = null;
	}
	public Player(string _name, int _team){
		Name = _name;
		Team = _team;
		Counting = 0;
		Hand = new List<Card>();
		SelectedCard = -1;
		Discard = new List<Card>();
		LastDiscarded = null;
	}
	public void TakeCard(Card _card){
		Hand.Add(_card);
	}
}
