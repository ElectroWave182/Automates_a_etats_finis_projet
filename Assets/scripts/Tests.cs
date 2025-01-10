using System;
using System. Collections;
using System. Collections. Generic;
using UnityEngine;


public class Tests: MonoBehaviour
{
	public void Start ()
	{
		// Initialisation
		
		Debug. Log ("- Tests -");
		
		Sommet [] chemin;
		
		var sommetA = new Sommet (5, -4);
		var sommetB = new Sommet (2, 0);
		var sommetC = new Sommet (-3, 7);
		var sommetD = new Sommet (0, 0);
		var sommetE = new Sommet (9, -7);
		var sommetF = new Sommet (0, 5);
		var sommetG = new Sommet (4, -4);
		var sommetH = new Sommet (-7, -3);
		var sommetI = new Sommet (2, 8);
		var sommetJ = new Sommet (5, 0);
		var sommetK = new Sommet (-6, 1);
		
		var grapheA = new Dictionary <Sommet, List <(float, Sommet)>> ()
		{
			{sommetA, new List <(float, Sommet)> ()},
			{sommetB, new List <(float, Sommet)> () {(6, sommetD), (1, sommetC)}},
			{sommetC, new List <(float, Sommet)> () {(1, sommetB), (3, sommetF)}},
			{sommetD, new List <(float, Sommet)> () {(5, sommetH), (6, sommetB)}},
			{sommetE, new List <(float, Sommet)> () {(1, sommetG)}},
			{sommetF, new List <(float, Sommet)> () {(15, sommetG), (3, sommetC)}},
			{sommetG, new List <(float, Sommet)> () {(1, sommetE), (4, sommetH), (10, sommetK), (15, sommetF)}},
			{sommetH, new List <(float, Sommet)> () {(4, sommetG), (5, sommetD), (4, sommetI), (9, sommetJ)}},
			{sommetI, new List <(float, Sommet)> () {(4, sommetH), (3, sommetJ)}},
			{sommetJ, new List <(float, Sommet)> () {(9, sommetH), (2, sommetK), (3, sommetI)}},
			{sommetK, new List <(float, Sommet)> () {(2, sommetJ), (10, sommetG)}},
		};
		
		
		// Tests sur Dijkstra
		
		chemin = Recherche. dijkstra (ref grapheA, sommetB, sommetK);
		Debug. Log (string. Join (", ", chemin));
		
		chemin = Recherche. dijkstra (ref grapheA, sommetK, sommetB);
		Debug. Log (string. Join (", ", chemin));
		
		chemin = Recherche. dijkstra (ref grapheA, sommetB, sommetB);
		Debug. Log (string. Join (", ", chemin));
		
		/*
		 *  Retourne une erreur, car sommetA est inaccessible depuis sommetB :
		 *  chemin = Recherche. dijkstra (ref grapheA, sommetB, sommetA);
		 *  Debug. Log (string. Join (", ", chemin));
		 */
		
		
		// Tests sur A*
		
		Debug. Log (Recherche. distanceVol (sommetA, sommetB));
		Debug. Log (Recherche. distanceVol (sommetB, sommetA));
		Debug. Log (Recherche. distanceVol (sommetA, sommetA));
		
		chemin = Recherche. aEtoile (ref grapheA, sommetB, sommetK);
		Debug. Log (string. Join (", ", chemin));
		
		chemin = Recherche. aEtoile (ref grapheA, sommetK, sommetB);
		Debug. Log (string. Join (", ", chemin));
		
		chemin = Recherche. aEtoile (ref grapheA, sommetB, sommetB);
		Debug. Log (string. Join (", ", chemin));
		
		/*
		 *  Retourne une erreur, car sommetA est inaccessible depuis sommetB :
		 *  chemin = Recherche. aEtoile (ref grapheA, sommetB, sommetA);
		 *  Debug. Log (string. Join (", ", chemin));
		 */
	}
}
