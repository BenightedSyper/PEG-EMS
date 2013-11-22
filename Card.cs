using UnityEngine;
using System.Collections;

public enum RANK { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Big, Small}
public enum SUIT { Joker, Diamond, Heart, Spade, Club }

public class Card {
	public RANK Rank;
	public SUIT Suit;
	
	public Card(){
		Rank = RANK.Small;
		Suit = SUIT.Joker;
	}
	public Card(RANK _rank, SUIT _suit){
		Rank = _rank;
		Suit = _suit;
	}
	public string FaceValue(){
		string faceValue = string.Empty;
		if(Suit != SUIT.Joker){
			faceValue += Rank;
			faceValue += "\n of \n";
			faceValue += Suit;
			faceValue += "s";
			return faceValue;
			//♥ ♦ ♣ ♠
		}else{
			faceValue += Rank;
			faceValue += " ";
			faceValue += Suit;
			return faceValue;
		}
	}
}
