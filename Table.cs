using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Table : MonoBehaviour {
	
	public enum PHASE { DRAW, PLAY, MOVE, END }
	public enum RULESET { CLASSIC }
	
	public float CARDWIDTH = 100f;
	public float CARDHEIGHT = 50f;
	public float CARDBUFFER = 10f;
	
	public Deck TableDeck;
	public List<Player> Players;
	
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
			foreach(Player _player in Players){
				if(_player.Hand.Count > 0){
					GUILayout.BeginHorizontal("");
					foreach(Card _card in _player.Hand){
						if(GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* _player.Hand.IndexOf(_card), (CARDHEIGHT + CARDBUFFER) * Players.IndexOf(_player),CARDWIDTH,CARDHEIGHT),_card.FaceValue())){
							_player.SelectedCard = _player.Hand.IndexOf(_card);
						}
					}
					if(GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* _player.Hand.Count, (CARDHEIGHT + CARDBUFFER) * Players.IndexOf(_player),CARDWIDTH,CARDHEIGHT),"Play Card \n" + _player.SelectedCard)){
						if(_player.SelectedCard != -1){
							PlayCard(_player, _player.SelectedCard);
							_player.SelectedCard = -1;
						}else{
							Debug.Log("you must select a card to play");
						}
					}
					GUILayout.EndHorizontal();
				}
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
		_player.TakeCard(TableDeck.Draw);
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
		_player.Hand.RemoveAt(_player.SelectedCard);
		//inc PhaseCounter
		PhaseCounter = PHASE.MOVE;
	}
	public void DiscardPhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.PLAY){
			return;
		}
		//discard selected card
		_player.Discard.Add(_player.LastDiscarded);
		_player.LastDiscarded = _player.Hand[_player.SelectedCard];
		_player.Hand.RemoveAt(_player.SelectedCard);
		//Player draws or Counts Depending on rule Set
		switch(RuleSet){
		case RULESET.CLASSIC:
			_player.TakeCard(TableDeck.Draw);
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
		PhaseCounter = PHASE.END
	}
	public void EndPhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.END){
			return;
		}
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
		P
		
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
		foreach(Player _player in Players){
			Debug.Log("" + _player.Name);
			foreach(Card _card in _player.Hand){
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
	public int SelectedCard;
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
