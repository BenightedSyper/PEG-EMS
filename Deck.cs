using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck {
	private IList<Card> Set;
	
	public Deck(){
		Set = new List<Card>();
		AddDeck();
		Shuffle();
		PrintCards();
	}
	public Deck(int _numberOfDecks){
		Set = new List<Card>();
		for(int i = 0; i < _numberOfDecks; i++){
			AddDeck();
		}
		Shuffle();
		PrintCards();
	}
	public void AddDeck(){
		//Diamonds
		Set.Add(new Card(RANK.Ace,SUIT.Diamond));
		Set.Add(new Card(RANK.Two,SUIT.Diamond));
		Set.Add(new Card(RANK.Three,SUIT.Diamond));
		Set.Add(new Card(RANK.Four,SUIT.Diamond));
		Set.Add(new Card(RANK.Five,SUIT.Diamond));
		Set.Add(new Card(RANK.Six,SUIT.Diamond));
		Set.Add(new Card(RANK.Seven,SUIT.Diamond));
		Set.Add(new Card(RANK.Eight,SUIT.Diamond));
		Set.Add(new Card(RANK.Nine,SUIT.Diamond));
		Set.Add(new Card(RANK.Ten,SUIT.Diamond));
		Set.Add(new Card(RANK.Jack,SUIT.Diamond));
		Set.Add(new Card(RANK.Queen,SUIT.Diamond));
		Set.Add(new Card(RANK.King,SUIT.Diamond));
		//Hearts
		Set.Add(new Card(RANK.Ace,SUIT.Heart));
		Set.Add(new Card(RANK.Two,SUIT.Heart));
		Set.Add(new Card(RANK.Three,SUIT.Heart));
		Set.Add(new Card(RANK.Four,SUIT.Heart));
		Set.Add(new Card(RANK.Five,SUIT.Heart));
		Set.Add(new Card(RANK.Six,SUIT.Heart));
		Set.Add(new Card(RANK.Seven,SUIT.Heart));
		Set.Add(new Card(RANK.Eight,SUIT.Heart));
		Set.Add(new Card(RANK.Nine,SUIT.Heart));
		Set.Add(new Card(RANK.Ten,SUIT.Heart));
		Set.Add(new Card(RANK.Jack,SUIT.Heart));
		Set.Add(new Card(RANK.Queen,SUIT.Heart));
		Set.Add(new Card(RANK.King,SUIT.Heart));
		//Spades
		Set.Add(new Card(RANK.Ace,SUIT.Spade));
		Set.Add(new Card(RANK.Two,SUIT.Spade));
		Set.Add(new Card(RANK.Three,SUIT.Spade));
		Set.Add(new Card(RANK.Four,SUIT.Spade));
		Set.Add(new Card(RANK.Five,SUIT.Spade));
		Set.Add(new Card(RANK.Six,SUIT.Spade));
		Set.Add(new Card(RANK.Seven,SUIT.Spade));
		Set.Add(new Card(RANK.Eight,SUIT.Spade));
		Set.Add(new Card(RANK.Nine,SUIT.Spade));
		Set.Add(new Card(RANK.Ten,SUIT.Spade));
		Set.Add(new Card(RANK.Jack,SUIT.Spade));
		Set.Add(new Card(RANK.Queen,SUIT.Spade));
		Set.Add(new Card(RANK.King,SUIT.Spade));
		//Clubs
		Set.Add(new Card(RANK.Ace,SUIT.Club));
		Set.Add(new Card(RANK.Two,SUIT.Club));
		Set.Add(new Card(RANK.Three,SUIT.Club));
		Set.Add(new Card(RANK.Four,SUIT.Club));
		Set.Add(new Card(RANK.Five,SUIT.Club));
		Set.Add(new Card(RANK.Six,SUIT.Club));
		Set.Add(new Card(RANK.Seven,SUIT.Club));
		Set.Add(new Card(RANK.Eight,SUIT.Club));
		Set.Add(new Card(RANK.Nine,SUIT.Club));
		Set.Add(new Card(RANK.Ten,SUIT.Club));
		Set.Add(new Card(RANK.Jack,SUIT.Club));
		Set.Add(new Card(RANK.Queen,SUIT.Club));
		Set.Add(new Card(RANK.King,SUIT.Club));
		//Jokers
		Set.Add(new Card(RANK.Big,SUIT.Joker));
		Set.Add(new Card(RANK.Small,SUIT.Joker));
	}
	public void Shuffle(){
		Random.seed = (int) Time.time;
		int length = Set.Count;
		while(length > 1){
			int randCard = Random.Range(0,length);
			Card tempCard = Set[randCard];
			Set[randCard] = Set[length - 1];
			Set[length - 1] = tempCard;
			length--;
		}
	}
	public void PrintCards(){
		foreach(Card _card in Set){
			Debug.Log(_card.FaceValue());
		}
	}
}
