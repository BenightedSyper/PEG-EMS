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
	
	private LOCATION MoveToLocationFirst;
	private int MoveToDistanceFirst;
	private LOCATION MoveToLocationSecond;
	private int MoveToDistanceSecond;
	
	private RULESET RuleSet;
	private int TurnCounter;
	private PHASE PhaseCounter;
	
	private Vector2 scrollPosition;
	public float hSliderValue = 0.0F;
	public float hSliderSevenValue = 1.0f;
	public float hSliderNineValue = 1.0f;
	public float hSliderTenValue = 1.0f;
	public bool NineToggle = true;
	public bool TenFirstToggle = true;
	public bool TenSecondToggle = true;
	public float hSliderJokerValue = 0.0F;
	
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
			if(Players[TurnCounter].Hand.Count > 0){
				foreach(Card _card in Players[TurnCounter].Hand){
					if(GUI.Button(new Rect((CARDWIDTH + CARDBUFFER)* Players[TurnCounter].Hand.IndexOf(_card), CARDBUFFER,CARDWIDTH,CARDHEIGHT),_card.FaceValue())){
						Players[TurnCounter].SelectedCard = Players[TurnCounter].Hand.IndexOf(_card);
					}
				}
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
					if(LastPlayed .Rank == RANK.Seven || LastPlayed .Rank == RANK.Nine || LastPlayed .Rank == RANK.Ten){
						if(FirstPeg != null && SecondPeg != null && FirstPeg != SecondPeg){
							PegPhase(Players[TurnCounter]);	
						}else{
							Debug.Log("You must select 2 different Pegs");
						}
					}else{
						if(FirstPeg != null){
							PegPhase(Players[TurnCounter]);
						}
					}
				}
				if(GUI.Button(new Rect(Camera.main.pixelWidth - (CARDWIDTH + CARDBUFFER), Camera.main.pixelHeight - (CARDHEIGHT + CARDBUFFER), CARDWIDTH, CARDHEIGHT), "Back")){
					Back(Players[TurnCounter]);
				}
				break;
			case PHASE.MOVE:
				//display the move that will happen
				DisplayMove();
				if(GUI.Button(new Rect((Camera.main.pixelWidth / 2) - CARDWIDTH / 2, Camera.main.pixelHeight / 2,CARDWIDTH,CARDHEIGHT),"Move")){
					MovePhase(Players[TurnCounter]);
				}
				if(GUI.Button(new Rect(Camera.main.pixelWidth - (CARDWIDTH + CARDBUFFER), Camera.main.pixelHeight - (CARDHEIGHT + CARDBUFFER), CARDWIDTH, CARDHEIGHT), "Back")){
					Back(Players[TurnCounter]);
				}
				break;
			case PHASE.END:
				if(GUI.Button(new Rect((Camera.main.pixelWidth / 2) - CARDWIDTH / 2, Camera.main.pixelHeight / 2,CARDWIDTH,CARDHEIGHT),"End Turn")){
					EndPhase(Players[TurnCounter]);
				}
				if(GUI.Button(new Rect(Camera.main.pixelWidth - (CARDWIDTH + CARDBUFFER), Camera.main.pixelHeight - (CARDHEIGHT + CARDBUFFER), CARDWIDTH, CARDHEIGHT), "Back")){
					Back(Players[TurnCounter]);
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
	public void Back(Player _player){
		switch(PhaseCounter){
		case PHASE.PEG:
			//go back to play phase
			PhaseCounter = PHASE.PLAY;
			//return the last played card back into the palyers hand
			_player.Discard.Remove(_player.LastDiscarded);
			_player.Hand.Add(_player.LastDiscarded);
			_player.LastDiscarded = null;//may need more logic to retain the last last discarded
			LastPlayed = null;
			FirstPeg = null;
			SecondPeg = null;
			break;
		case PHASE.MOVE:
			FirstPeg = null;
			SecondPeg = null;
			PhaseCounter = PHASE.PEG;
			break;
		case PHASE.END:
			//having a stack of peg states would make this easy
			break;
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
			/*
			if(FirstPeg != null){
				return list with only pegs from other teams
			}
			*/
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
		//needs logic for which peg is moved first.
		if(LastPlayed.Rank == RANK.Ten || LastPlayed.Rank == RANK.Nine || LastPlayed.Rank == RANK.Seven){
			if(TestPegMove(FirstPeg, MoveToLocationFirst, MoveToDistanceFirst)){
				FirstPeg.Location = MoveToLocationFirst;
				FirstPeg.Distance = MoveToDistanceFirst;
			}
			if(TestPegMove(SecondPeg, MoveToLocationSecond, MoveToDistanceSecond)){
				SecondPeg.Location = MoveToLocationSecond;
				SecondPeg.Distance = MoveToDistanceSecond;
			}
		}else{
			if(TestPegMove(FirstPeg, MoveToLocationFirst, MoveToDistanceFirst)){
				FirstPeg.Location = MoveToLocationFirst;
				FirstPeg.Distance = MoveToDistanceFirst;
			}
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
	public void CheckSevenSlider(){
		hSliderSevenValue = Mathf.FloorToInt(hSliderSevenValue);
	}
	public void CheckNineSlider(){
		hSliderNineValue = Mathf.FloorToInt(hSliderNineValue);
	}
	public void CheckTenSlider(){
		hSliderTenValue = Mathf.FloorToInt(hSliderTenValue);
	}
	public void CheckJokerSlider(){
		hSliderJokerValue = Mathf.FloorToInt(hSliderJokerValue);
	}
	public void DisplayMove(){
		string MoveDescription= "";
		switch(LastPlayed.Rank){
		case RANK.Ace:
			MoveDescription = "Player: " + TurnCounter + " played an Ace on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.HOME){
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = TableBoard.GetPlayerHomeExit(TurnCounter);
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
			}
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//two options: 1 space and 11 spaces
				hSliderValue = GUI.HorizontalSlider(new Rect(25, (CARDHEIGHT + CARDBUFFER) * 2, 100, 30), hSliderValue, 0.0F, 1.0F);
				CheckAceSlider();
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = (FirstPeg.Distance + (hSliderValue > 0 ? 11 : 1))% TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistanceFirst > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocationFirst = LOCATION.CASTLE;
					MoveToDistanceFirst = MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter);
					
				}
			}
			if(FirstPeg.Location == LOCATION.CASTLE){
				MoveToLocationFirst = LOCATION.CASTLE;
				MoveToDistanceFirst = FirstPeg.Distance + 1;
			}
			break;
		case RANK.Two:
			MoveDescription = "Player: " + TurnCounter + " played a Two on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = (FirstPeg.Distance + 2 )% TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistanceFirst > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocationFirst = LOCATION.CASTLE;
					MoveToDistanceFirst = MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			if(FirstPeg.Location == LOCATION.CASTLE){
				MoveToLocationFirst = LOCATION.CASTLE;
				MoveToDistanceFirst = FirstPeg.Distance + 2;
			}
			break;
		case RANK.Three:
			MoveDescription = "Player: " + TurnCounter + " played a Three on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = (FirstPeg.Distance + 3) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistanceFirst > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocationFirst = LOCATION.CASTLE;
					MoveToDistanceFirst = MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			if(FirstPeg.Location == LOCATION.CASTLE){
				MoveToLocationFirst = LOCATION.CASTLE;
				MoveToDistanceFirst = FirstPeg.Distance + 3;
			}
			break;
		case RANK.Four:
			MoveDescription = "Player: " + TurnCounter + " played a Four on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = (FirstPeg.Distance + 4) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistanceFirst > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocationFirst = LOCATION.CASTLE;
					MoveToDistanceFirst = MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			if(FirstPeg.Location == LOCATION.CASTLE){
				MoveToLocationFirst = LOCATION.CASTLE;
				MoveToDistanceFirst = FirstPeg.Distance + 4;
			}
			break;
		case RANK.Five:
			MoveDescription = "Player: " + TurnCounter + " played a Five on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = (FirstPeg.Distance + 5) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistanceFirst > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocationFirst = LOCATION.CASTLE;
					MoveToDistanceFirst = MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			break;
		case RANK.Six:
			MoveDescription = "Player: " + TurnCounter + " played a Six on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = (FirstPeg.Distance + 6) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistanceFirst > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocationFirst = LOCATION.CASTLE;
					MoveToDistanceFirst = MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			break;
		case RANK.Seven://needs logic for only one playable peg
			MoveDescription = "Player: " + TurnCounter + " played a Seven on Pegs: " + FirstPeg.Location + " " + FirstPeg.Distance + "and " + SecondPeg + " " + SecondPeg.Distance + "\n";
			//6 options: 1/6 2/5 3/4 4/3 5/2 6/1
			hSliderSevenValue = GUI.HorizontalSlider(new Rect(25, (CARDHEIGHT + CARDBUFFER) * 2, 100, 30), hSliderSevenValue, 1.0F, 6.0F);
			CheckSevenSlider();
			
			MoveDescription += "Moving Peg " + FirstPeg.Location + " " + FirstPeg.Distance + " Forward " + (int)hSliderSevenValue + "\n";
			MoveToDistanceFirst = (FirstPeg.Distance + (int)hSliderSevenValue) % TableBoard.Length;
			MoveToLocationFirst = LOCATION.MAINTRACK;
			MoveDescription += "Moving Peg " + SecondPeg.Location + " " + SecondPeg.Distance + " Forward " + (7 - (int)hSliderSevenValue) + "\n";
			MoveToDistanceSecond = (SecondPeg.Distance + (7 - (int)hSliderSevenValue)) % TableBoard.Length;
			MoveToLocationFirst = LOCATION.MAINTRACK;
			break;
		case RANK.Eight:
			MoveDescription = "Player: " + TurnCounter + " played an Eight on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = (FirstPeg.Distance + (TableBoard.Length - 8)) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
			}
			break;
		case RANK.Nine://needs logic for only one playable peg
			MoveDescription = "Player: " + TurnCounter + " played a Nine on Pegs: " + FirstPeg.Location + " " + FirstPeg.Distance + "and " + SecondPeg + " " + SecondPeg.Distance + "\n";
			
			//16 options: 1/-8 2/-7 3/-6 4/-5 5/-4 6/-3 7/-2 8/-1 -1/8 -2/7 -3/6 -4/5 -5/4 -6/3 -7/2 -8/1 
			hSliderNineValue = GUI.HorizontalSlider(new Rect(25, (CARDHEIGHT + CARDBUFFER) * 2, 100, 30), hSliderNineValue, 1.0F, 8.0F);
			CheckNineSlider();
			
			//needs logic to check if the the peg moving backwards is not in the Castle
			NineToggle = GUI.Toggle(new Rect (25, (CARDHEIGHT + CARDBUFFER) * 2 + 30, 100, 30), NineToggle, (NineToggle?"Forward/Backward":"Backward/Forward"));
			
			MoveDescription += "Moving Peg " + FirstPeg.Location + " " + FirstPeg.Distance + (NineToggle? " Forward ": " Backwards ") + hSliderNineValue + "\n";
			MoveToDistanceFirst = FirstPeg.Distance + (NineToggle? (int)hSliderNineValue : -(int)hSliderNineValue );
			MoveToLocationFirst = LOCATION.MAINTRACK;
			MoveDescription += "Moving Peg " + SecondPeg.Location + " " + SecondPeg.Distance + (NineToggle? " Backwards ": " Forward ") + (9 - hSliderNineValue) + "\n";
			MoveToDistanceSecond = SecondPeg.Distance + (NineToggle? -(9 - (int)hSliderNineValue): (9 - (int)hSliderNineValue) );
			MoveToLocationSecond = LOCATION.MAINTRACK;
			
			break;
		case RANK.Ten://needs logic for only one playable peg
			MoveDescription = "Player: " + TurnCounter + " played a Ten on Pegs: " + FirstPeg.Location + " " + FirstPeg.Distance + "and " + SecondPeg + " " + SecondPeg.Distance + "\n";
			
			//9 options: 1/9 2/8 3/7 4/6 5/5 6/4 7/3 8/2 9/1 
			//+/+ +/- -/+ -/-
			hSliderTenValue = GUI.HorizontalSlider(new Rect(25, (CARDHEIGHT + CARDBUFFER) * 2, 100, 30), hSliderTenValue, 1.0F, 9.0F);
			CheckTenSlider();
			
			TenFirstToggle = GUI.Toggle(new Rect (25, (CARDHEIGHT + CARDBUFFER) * 2 + 30, 100, 30), TenFirstToggle, (TenFirstToggle?"Forward":"Backward"));
			TenSecondToggle = GUI.Toggle(new Rect (25, (CARDHEIGHT + CARDBUFFER) * 2 + 60, 100, 30), TenSecondToggle, (TenSecondToggle?"Forward":"Backward"));
			
			MoveDescription += "Moving Peg " + FirstPeg.Location + " " + FirstPeg.Distance + (TenFirstToggle? " Forward " : " Backwards ") + hSliderTenValue + "\n";
			MoveToDistanceFirst = FirstPeg.Distance + (TenFirstToggle? (int)hSliderTenValue : -(int)hSliderTenValue);
			MoveToLocationFirst = LOCATION.MAINTRACK;
			MoveDescription += "Moving Peg " + SecondPeg.Location + " " + SecondPeg.Distance + (TenSecondToggle? " Forward " : " Backwards ") + (10 - hSliderTenValue) + "\n";
			MoveToDistanceSecond = SecondPeg.Distance + (TenSecondToggle? (10 - (int)hSliderTenValue) : -(10 - (int)hSliderTenValue));
			MoveToLocationSecond = LOCATION.MAINTRACK;
			
			break;
		case RANK.Jack:
		case RANK.Queen:
		case RANK.King:
			MoveDescription = "Player: " + TurnCounter + " played a " + LastPlayed.Rank + " on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			if(FirstPeg.Location == LOCATION.HOME){
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = TableBoard.GetPlayerHomeExit(TurnCounter);
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
			}
			if(FirstPeg.Location == LOCATION.MAINTRACK){
				//2 spaces Forward
				MoveToLocationFirst = LOCATION.MAINTRACK;
				MoveToDistanceFirst = (FirstPeg.Distance + 10) % TableBoard.Length;
				MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
				if( FirstPeg.Distance < TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& MoveToDistanceFirst > TableBoard.GetPlayerCastleEntrance(TurnCounter) 
					&& (MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter)) < 5){
					//give the option to move into the castle
					MoveToLocationFirst = LOCATION.CASTLE;
					MoveToDistanceFirst = MoveToDistanceFirst - TableBoard.GetPlayerCastleEntrance(TurnCounter);
				}
			}
			break;
		case RANK.Small:
		case RANK.Big:
			MoveDescription = "Player: " + TurnCounter + " played a Joker on Peg: " + FirstPeg.Location + " " + FirstPeg.Distance + "\n";
			
			hSliderJokerValue = GUI.HorizontalSlider(new Rect(25, (CARDHEIGHT + CARDBUFFER) * 2, 100, 30), hSliderJokerValue, 0, TableBoard.Length);
			CheckJokerSlider();
			
			MoveToLocationFirst = LOCATION.MAINTRACK;
			MoveToDistanceFirst = Mathf.FloorToInt(hSliderJokerValue);
			MoveDescription += "moving to " + MoveToLocationFirst + " " + MoveToDistanceFirst;
			break;
		}
		GUI.Label(new Rect(0,CARDHEIGHT + CARDBUFFER, Camera.main.pixelWidth, CARDHEIGHT * 2),MoveDescription);
	}
	public void PegToPlayerHome(Peg _peg){
		_peg.Location = LOCATION.HOME;
		_peg.Distance = _peg.Number;
	}
	public bool TestPegMove(Peg _peg, LOCATION _location, int _distance){
		LOCATION beginningLocation = _peg.Location;
		int beginningDistance = _peg.Distance;
		
		//need to check moving distances for valid position
		if((_location == LOCATION.MAINTRACK && _distance > TableBoard.Length) || (_location == LOCATION.CASTLE && _distance > 4)){
			//this is outside the play area
			//should pop and error message
			Debug.Log("Move to: " + _location + " " + _distance + " is outside the play area, invalide move.");
			return false;
		}
		//need to check the landing location of any pegs that was landed on
		if(_peg.Location == LOCATION.HOME && _location == LOCATION.MAINTRACK){
			//check maintrack distance for any other peg
			for(int i = 0; i < Players.Count; i++){
				for(int j = 0; j < 5 /*the number of pegs a player has*/; j++){
					//test each peg to see if its in that spot.
					if(TableBoard.PlayersPegs[i,j].Location == _location && TableBoard.PlayersPegs[i,j].Distance == _distance){
						if(_peg.Player == TableBoard.PlayersPegs[i,j].Player){
							return false;
						}else{//not same player's pegs
							if(_peg.Team == TableBoard.PlayersPegs[i,j].Team){//same team
								return TestPegMove(TableBoard.PlayersPegs[i,j], LOCATION.MAINTRACK, TableBoard.GetPlayerCastleEntrance(i));
							}else{//not same team
								PegToPlayerHome(TableBoard.PlayersPegs[i,j]);
								return true;
							}
						}
					}
				}
			}
		}
		//need to check that we are not moving over any of our own pegs.
		
		
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
		MoveToDistanceFirst = -1;
		MoveToDistanceSecond = -1;
		
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
	public int Number;
	public int Team;
	public int Player;
	public LOCATION Location;
	public int Distance;
	
	public Peg(int _number, int _team, int _player, LOCATION _location, int _distance){
		Number = _number;
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
				PlayersPegs[i,0] = new Peg(0, 1, i, LOCATION.HOME, 0);
				PlayersPegs[i,1] = new Peg(1, 1, i, LOCATION.HOME, 1);
				PlayersPegs[i,2] = new Peg(2, 1, i, LOCATION.HOME, 2);
				PlayersPegs[i,3] = new Peg(3, 1, i, LOCATION.HOME, 3);
				PlayersPegs[i,4] = new Peg(4, 1, i, LOCATION.HOME, 4);
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
