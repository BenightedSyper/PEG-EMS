using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Table : MonoBehaviour {
	
	public enum PHASE { DRAW, PLAY, PEG, MOVE, END }
	public enum RULESET { CLASSIC }
	
	public float CARDWIDTH = 100f;
	public float CARDHEIGHT = 50f;
	public float CARDBUFFER = 10f;
	
	public Board TableBoard;
	public Deck TableDeck;
	public List<Player> Players;
	public Card LastPlayed;
	public Peg FirstPeg;
	public Peg SecondPeg;
	
	private LOCATION MoveToLocation;
	private int MoveToDistance;
	
	private RULESET RuleSet;
	private int TurnCounter;
	private PHASE PhaseCounter;
	
	private Vector2 scrollPosition;
	public float hSliderValue = 0.0F;
	
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
				//GUILayout.BeginHorizontal("");
				foreach(Card _card in Players[TurnCounter].Hand){
					if(GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* Players[TurnCounter].Hand.IndexOf(_card), CARDBUFFER,CARDWIDTH,CARDHEIGHT),_card.FaceValue())){
						Players[TurnCounter].SelectedCard = Players[TurnCounter].Hand.IndexOf(_card);
					}
				}
				//GUILayout.EndHorizontal();
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
			case PHASE.PEG:
				List<Peg> tempPegs = GetAvailablePegs(TurnCounter, LastPlayed);
				//display the available pegs
				DisplayPegs(tempPegs);
				//check the selected card for available pegs
				//Ace Maintrack Pegs of the player, Castle Pegs, and Home Pegs
				//2-9 Maintrack Pegs of the player, Castle Pegs (needs logic for 7's and 9's upon moving the last Peg into the castle)
				//10 All Main Track Pegs + logic for a selected Peg filtering out any Peg from that Player
				//Jack, Queen, KingMaintrack Pegs of the player and Home Pegs
				//Joker all Player's Pegs.
				/*
				GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* 0, (CARDBUFFER * 2) + CARDHEIGHT,CARDWIDTH,CARDHEIGHT),TableBoard.PlayersPegs[TurnCounter,0].Name());
				GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* 1, (CARDBUFFER * 2) + CARDHEIGHT,CARDWIDTH,CARDHEIGHT),TableBoard.PlayersPegs[TurnCounter,1].Name());
				GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* 2, (CARDBUFFER * 2) + CARDHEIGHT,CARDWIDTH,CARDHEIGHT),TableBoard.PlayersPegs[TurnCounter,2].Name());
				GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* 3, (CARDBUFFER * 2) + CARDHEIGHT,CARDWIDTH,CARDHEIGHT),TableBoard.PlayersPegs[TurnCounter,3].Name());
				GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* 4, (CARDBUFFER * 2) + CARDHEIGHT,CARDWIDTH,CARDHEIGHT),TableBoard.PlayersPegs[TurnCounter,4].Name());
				*/
				if(GUI.Button(new Rect((Camera.main.pixelWidth / 2) + (CARDBUFFER / 2), Camera.main.pixelHeight / 2,CARDWIDTH,CARDHEIGHT),"Select Peg")){
					if(FirstPeg != null){
						PegPhase(Players[TurnCounter]);
					}
				}
				break;
			case PHASE.MOVE:
				//display the move that will happen
				DisplayMove();
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
				GUI.Label(new Rect(0,Camera.main.pixelHeight - (CARDHEIGHT + CARDBUFFER), CARDWIDTH, CARDHEIGHT + CARDBUFFER),"Last Played\n" + LastPlayed.FaceValue());
			}
			if(FirstPeg != null){
				GUI.Label(new Rect((CARDWIDTH + CARDBUFFER) * 1,Camera.main.pixelHeight - (CARDHEIGHT + CARDBUFFER), CARDWIDTH, CARDHEIGHT + CARDBUFFER),"First Peg\n" + FirstPeg.Name());
			}
			if(SecondPeg != null){
				GUI.Label(new Rect((CARDWIDTH + CARDBUFFER) * 2,Camera.main.pixelHeight - (CARDHEIGHT + CARDBUFFER), CARDWIDTH, CARDHEIGHT + CARDBUFFER),"Second Peg\n" + SecondPeg.Name());
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
		PhaseCounter = PHASE.PEG;
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
				//Peg select
				//PhaseCounter = PHASE.PEG
				_player.Counting = 0;
			}
			break;
		}
		//inc PhaseCounter
		PhaseCounter = PHASE.END;
	}
	public void PegPhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.PEG){
			return;
		}
		PhaseCounter = PHASE.MOVE;
	}
	public List<Peg> GetAvailablePegs(int _player, Card _card){
		List<Peg> returnList = new List<Peg>();
		switch(_card.Rank){
		case RANK.Ace:
			//logic for removing pegs at the end of the Castle from list
			returnList.Add(TableBoard.PlayersPegs[_player,0]);
			returnList.Add(TableBoard.PlayersPegs[_player,1]);
			returnList.Add(TableBoard.PlayersPegs[_player,2]);
			returnList.Add(TableBoard.PlayersPegs[_player,3]);
			returnList.Add(TableBoard.PlayersPegs[_player,4]);
			break;
		case RANK.Six:
		case RANK.Eight:
			if(TableBoard.PlayersPegs[_player,0].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,0]);
			}
			if(TableBoard.PlayersPegs[_player,1].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,1]);
			}
			if(TableBoard.PlayersPegs[_player,2].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,2]);
			}
			if(TableBoard.PlayersPegs[_player,3].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,3]);
			}
			if(TableBoard.PlayersPegs[_player,4].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,4]);
			}
			break;
		case RANK.Two:
		case RANK.Three:
		case RANK.Four:
		case RANK.Five:
		case RANK.Seven:
		case RANK.Nine:
			if(TableBoard.PlayersPegs[_player,0].Location != LOCATION.HOME){
				returnList.Add(TableBoard.PlayersPegs[_player,0]);
			}
			if(TableBoard.PlayersPegs[_player,1].Location != LOCATION.HOME){
				returnList.Add(TableBoard.PlayersPegs[_player,1]);
			}
			if(TableBoard.PlayersPegs[_player,2].Location != LOCATION.HOME){
				returnList.Add(TableBoard.PlayersPegs[_player,2]);
			}
			if(TableBoard.PlayersPegs[_player,3].Location != LOCATION.HOME){
				returnList.Add(TableBoard.PlayersPegs[_player,3]);
			}
			if(TableBoard.PlayersPegs[_player,4].Location != LOCATION.HOME){
				returnList.Add(TableBoard.PlayersPegs[_player,4]);
			}
			break;
		case RANK.Ten:
			//more logic needed for selecting 2 different player's pegs
			if(TableBoard.PlayersPegs[_player,0].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,0]);
			}
			if(TableBoard.PlayersPegs[_player,1].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,1]);
			}
			if(TableBoard.PlayersPegs[_player,2].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,2]);
			}
			if(TableBoard.PlayersPegs[_player,3].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,3]);
			}
			if(TableBoard.PlayersPegs[_player,4].Location == LOCATION.MAINTRACK){
				returnList.Add(TableBoard.PlayersPegs[_player,4]);
			}
			break;
		case RANK.Jack:
		case RANK.Queen:
		case RANK.King:
			if(TableBoard.PlayersPegs[_player,0].Location != LOCATION.CASTLE){
				returnList.Add(TableBoard.PlayersPegs[_player,0]);
			}
			if(TableBoard.PlayersPegs[_player,1].Location != LOCATION.CASTLE){
				returnList.Add(TableBoard.PlayersPegs[_player,1]);
			}
			if(TableBoard.PlayersPegs[_player,2].Location != LOCATION.CASTLE){
				returnList.Add(TableBoard.PlayersPegs[_player,2]);
			}
			if(TableBoard.PlayersPegs[_player,3].Location != LOCATION.CASTLE){
				returnList.Add(TableBoard.PlayersPegs[_player,3]);
			}
			if(TableBoard.PlayersPegs[_player,4].Location != LOCATION.CASTLE){
				returnList.Add(TableBoard.PlayersPegs[_player,4]);
			}
			break;
		case RANK.Small:
		case RANK.Big:
			returnList.Add(TableBoard.PlayersPegs[_player,0]);
			returnList.Add(TableBoard.PlayersPegs[_player,1]);
			returnList.Add(TableBoard.PlayersPegs[_player,2]);
			returnList.Add(TableBoard.PlayersPegs[_player,3]);
			returnList.Add(TableBoard.PlayersPegs[_player,4]);
			break;
		}
		return returnList;
	}
	private void DisplayPegs(List<Peg> _pegs){
		foreach(Peg _peg in _pegs){
			//Peg GUI
			if(GUI.Button( new Rect((CARDBUFFER + CARDWIDTH) * _pegs.IndexOf(_peg), CARDHEIGHT + CARDBUFFER + CARDBUFFER, CARDWIDTH, CARDHEIGHT), _peg.Name())){
				//Select Peg
				if(LastPlayed.Rank == RANK.Ten || LastPlayed.Rank == RANK.Nine || LastPlayed.Rank == RANK.Seven){
					SecondPeg = FirstPeg;
				}
				FirstPeg = _peg;
			}
		}
	}
	public void MovePhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.MOVE){
			return;
		}
		if(TestPegMove(FirstPeg, MoveToLocation, MoveToDistance)){
			FirstPeg.Location = MoveToLocation;
			FirstPeg.Distance = MoveToDistance;
		}
		//move logic
		//if(!TestPegMovement()){
		//return;
		//}
		PhaseCounter = PHASE.END;
	}
	public void CheckAceSlider(){
		if(hSliderValue > 0.5f){
			hSliderValue = 1.0f;
		}else{
			hSliderValue = 0.0f;
		}
	}
	public void DisplayMove(){
		string MoveDescription= "";
		switch(LastPlayed.Rank){
		case RANK.Ace:
			MoveDescription = "Player: " + TurnCounter + " played an Ace on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.HOME){
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = TableBoard.GetPlayerHomeExit(TurnCounter);
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
			}
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//two options: 1 space and 11 spaces
				hSliderValue = GUI.HorizontalSlider(new Rect(25, (CARDHEIGHT + CARDBUFFER) * 2, 100, 30), hSliderValue, 0.0F, 1.0F);
				CheckAceSlider();
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = (FirstPeg.Distance + (hSliderValue > 0 ? 11 : 1))% TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistance > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocation = LOCATION.CASTLE;
					MoveToDistance = MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter);
					
				}
			}
			if(FirstPeg.Location == LOCATION.CASTLE){
				MoveToLocation = LOCATION.CASTLE;
				MoveToDistance = FirstPeg.Distance + 1;
			}
			break;
		case RANK.Two:
			MoveDescription = "Player: " + TurnCounter + " played a Two on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = (FirstPeg.Distance + 2 )% TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistance > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocation = LOCATION.CASTLE;
					MoveToDistance = MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			if(FirstPeg.Location == LOCATION.CASTLE){
				MoveToLocation = LOCATION.CASTLE;
				MoveToDistance = FirstPeg.Distance + 2;
			}
			break;
		case RANK.Three:
			MoveDescription = "Player: " + TurnCounter + " played a Three on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = (FirstPeg.Distance + 3) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistance > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocation = LOCATION.CASTLE;
					MoveToDistance = MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			if(FirstPeg.Location == LOCATION.CASTLE){
				MoveToLocation = LOCATION.CASTLE;
				MoveToDistance = FirstPeg.Distance + 3;
			}
			break;
		case RANK.Four:
			MoveDescription = "Player: " + TurnCounter + " played a Four on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = (FirstPeg.Distance + 4) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistance > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocation = LOCATION.CASTLE;
					MoveToDistance = MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			if(FirstPeg.Location == LOCATION.CASTLE){
				MoveToLocation = LOCATION.CASTLE;
				MoveToDistance = FirstPeg.Distance + 4;
			}
			break;
		case RANK.Five:
			MoveDescription = "Player: " + TurnCounter + " played a Five on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = (FirstPeg.Distance + 5) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistance > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocation = LOCATION.CASTLE;
					MoveToDistance = MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			break;
		case RANK.Six:
			MoveDescription = "Player: " + TurnCounter + " played a Six on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = (FirstPeg.Distance + 6) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistance > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocation = LOCATION.CASTLE;
					MoveToDistance = MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			break;
		case RANK.Seven:
			MoveDescription = "Player: " + TurnCounter + " played a Seven on Pegs: " + FirstPeg.Location + " " + FirstPeg.Distance + "and " + SecondPeg + " " + SecondPeg.Distance + "\n";
			
			break;
		case RANK.Eight:
			MoveDescription = "Player: " + TurnCounter + " played an Eight on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = (FirstPeg.Distance + (TableBoard.Length - 8)) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistance > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocation = LOCATION.CASTLE;
					MoveToDistance = MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			break;
		case RANK.Nine:
			MoveDescription = "Player: " + TurnCounter + " played a Nine on Pegs: " + FirstPeg.Location + " " + FirstPeg.Distance + "and " + SecondPeg + " " + SecondPeg.Distance + "\n";
			break;
		case RANK.Ten:
			MoveDescription = "Player: " + TurnCounter + " played a Ten on Pegs: " + FirstPeg.Location + " " + FirstPeg.Distance + "and " + SecondPeg + " " + SecondPeg.Distance + "\n";
			break;
		case RANK.Jack:
		case RANK.Queen:
		case RANK.King:
			MoveDescription = "Player: " + TurnCounter + " played a " + LastPlayed.Rank + " on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.HOME){
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = TableBoard.GetPlayerHomeExit(TurnCounter);
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
			}
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocation = LOCATION.MAINTRACK;
				MoveToDistance = (FirstPeg.Distance + 10) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocation + " " + MoveToDistance;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistance > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocation = LOCATION.CASTLE;
					MoveToDistance = MoveToDistance - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			break;
		case RANK.Small:
		case RANK.Big:
			break;
		}
		GUI.Label(new Rect(0,CARDHEIGHT + CARDBUFFER, Camera.main.pixelWidth, CARDHEIGHT * 2),MoveDescription);
	}
	public bool TestPegMove(Peg _peg, LOCATION _location, int _distance){
		LOCATION beginningLocation = _peg.Location;
		int beginningDistance = _peg.Distance;
		for(int i = 0; i < Players.Count; i++){
			for(int j = 0; j < 5 /*the number of pegs a player has*/; j++){
				//test each peg to see if its in that spot.
				//this should be later optimized for specific cards or locations.
				if(TableBoard.PlayersPegs[i,j].Location == _location){
					
				}
			}
		}
		return true;
	}
	public void EndPhase(Player _player){
		if(TurnCounter != Players.IndexOf(_player) && PhaseCounter != PHASE.END){
			return;
		}
		_player.SelectedCard = -1;
		LastPlayed = null;
		FirstPeg = null;
		SecondPeg = null;
		MoveToDistance = -1;
		
		//roll PhaseCounter
		PhaseCounter = PHASE.DRAW;
		//roll TurnCounter
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
	
	public Peg(int _team, int _player, LOCATION _location, int _distance){
		Team = _team;
		Player = _player;
		Location = _location;
		Distance = _distance;
	}
	public string Name(){
		return "Player: " + Player + "\n" + Location + ": " + Distance;
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
	public int Length;
	public Peg[,] PlayersPegs;
	
	public Board(int _players){
		Length = 18 * _players;
		PlayersPegs = new Peg[_players, 5];
		for(int i = 0; i < _players; i++){
				PlayersPegs[i,0] = new Peg(1, i, LOCATION.HOME, 0);
				PlayersPegs[i,1] = new Peg(1, i, LOCATION.HOME, 1);
				PlayersPegs[i,2] = new Peg(1, i, LOCATION.HOME, 2);
				PlayersPegs[i,3] = new Peg(1, i, LOCATION.HOME, 3);
				PlayersPegs[i,4] = new Peg(1, i, LOCATION.HOME, 4);
		}
	}
	public int GetPlayerHomeExit(int _player){
		return (18 * _player) + 8;
	}
	public int GetPlayerCastleEntrance(int _player){
		return (18 * _player) + 5;
	}
}

public class Player{
	public string Name;
	public int Team;
	public int Counting;
	public List<Card> Hand;
	public int SelectedCard;
	public Peg[] Pegs;
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
