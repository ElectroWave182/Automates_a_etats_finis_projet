using System;
using System. Collections;
using System. Collections. Generic;
using UnityEngine;


public class Graphe
{
	// Le graphe en question sous forme d'un dictionnaire d'adjacences
	public Dictionary <Sommet, List <(float, Sommet)>> dictionnaireAdjacences {get; set;}
	
	private bool [,] carte;
	private Sommet
		depart,
		arrivee
	;
	
	
	// Constructeur : construit le graphe en fonction des paramètres concernant la mine
	
	public Graphe (ref bool [,] c, Sommet d, Sommet a)
	{
		this. carte = c;
		this. depart = d;
		this. arrivee = a;
		
		construireSommets ();
		construireAretes ();
		
		Debug. Log ("Nouveau graphe : " + this);
	}
	
	
	/*
	 *  Les sommets du graphe concernent uniquement les cases
	 *  où le plus court chemin est susceptible de tourner
	 */
	
	private void construireSommets ()
	{
		// Initialisation
		
		bool a, b, c, d, e, f, g, h;
		int longueur = this. carte. GetLength (0);
		int hauteur  = this. carte. GetLength (1);
		Sommet sommetAjoute;
		
		
		// Ajout des points de départ et d'arrivée
		
		dictionnaireAdjacences = new Dictionary <Sommet, List <(float, Sommet)>> ();
		
		this. dictionnaireAdjacences [this. depart]  = new List <(float, Sommet)> ();
		this. dictionnaireAdjacences [this. arrivee] = new List <(float, Sommet)> ();
		
		
		// Ajout des points de passage possibles
		
		for (int x = 0; x < longueur; x ++)
		for (int y = 0; y < hauteur; y ++)
		{
			/*
			 *  ┌───┬───┬───┐
			 *  │ e │ a │ h │
			 *  ├───┼───┼───┤
			 *  │ b │x,y│ d │
			 *  ├───┼───┼───┤
			 *  │ f │ c │ g │
			 *  └───┴───┴───┘
			 */
			
			a = Tuiles. estSol (ref this. carte, x, y + 1);
			b = Tuiles. estSol (ref this. carte, x - 1, y);
			c = Tuiles. estSol (ref this. carte, x, y - 1);
			d = Tuiles. estSol (ref this. carte, x + 1, y);
			e = Tuiles. estSol (ref this. carte, x - 1, y + 1);
			f = Tuiles. estSol (ref this. carte, x - 1, y - 1);
			g = Tuiles. estSol (ref this. carte, x + 1, y - 1);
			h = Tuiles. estSol (ref this. carte, x + 1, y + 1);
			
			// La case est un point de passage s'il s'agît d'un sol en coin de mur
			if (this. carte [x, y] && (a && b && !e || b && c && !f || c && d && !g || d && a && !h))
			{
				sommetAjoute = new Sommet (x, y);
				this. dictionnaireAdjacences [sommetAjoute] = new List <(float, Sommet)> ();
			}
		}
	}
	
	
	// Ajout des arêtes en force brute
	
	private void construireAretes ()
	{
		// Initialisation
		
		bool aretePossible;
		float
			cout,
			xDelta,
			yDelta
		;
		int
			debut,
			fin,
			saut
		;
		Sommet origineZone;
		
		
		// Pour chaque couple de sommets,
		
		foreach (Sommet a in this. dictionnaireAdjacences. Keys)
		foreach (Sommet b in this. dictionnaireAdjacences. Keys)
		{
			// Inutile de passer 2 fois la même arête non orientée
			if (a >= b)
			{
				continue;
			}
			
		
			// L'arête forme un segment entre ses 2 extrémités
		
			xDelta = Math. Abs (a. x - b. x);
			yDelta = Math. Abs (a. y - b. y);
			
			
			/*
			 *  Le segment ne sera pas parcouru dans le même sens
			 *  suivant s'il monte ou s'il descend
			 */
			
			origineZone = new Sommet
			(
				Math. Min (a. x, b. x),
				Math. Min (a. y, b. y)
			);
			
			if (origineZone == a)
			{
				saut = 1;
			}
			else
			{
				saut = -1;
			}
		
			
			// Le segment est plutôt horizontal
			if (xDelta > yDelta)
			{
				if (origineZone == a)
				{
					debut = a. x;
					fin = b. x;
				}
				else
				{
					debut = b. x;
					fin = a. x;
				}
				
				aretePossible = degagee (debut, fin, saut, xDelta, yDelta, origineZone. y, (0, 1));
			}
			
			// Le segment est plutôt vertical
			else
			{
				debut = a. y;
				fin = b. y;
				
				aretePossible = degagee (debut, fin, saut, yDelta, xDelta, a. x, (1, 0));
			}
			
			
			// S'il y a arête, l'on ajoute ses informations
			
			if (aretePossible)
			{
				cout = a. distance (b);
				
				dictionnaireAdjacences [a]. Add ((cout, b));
				dictionnaireAdjacences [b]. Add ((cout, a));
			}
		}
	}
	
	
	// Détermine si un segment donné (une arête) est dégagé
	
	private bool degagee
	(
		int debut,
		int fin,
		int saut,
		float hDelta,
		float vDelta,
		int vDebut,
		(int x, int y) direction
	)
	{
		float coefDirecteur = vDelta / hDelta;
		float vSegment = vDebut - coefDirecteur * 0.5f;
		
		int
			vCase,
			xCase,
			yCase
		;
		
		
		/*
		 *  Parcours du segment de bas en haut s'il est horizontal,
		 *  ou de gauche à droite s'il est vertical
		 */
		
		for (int hCase = debut; hCase != fin; hCase += saut)
		{
			vCase = (int) Math. Round (vSegment);
			vSegment += coefDirecteur;
			
			if (direction. x == 0)
			{
				xCase = hCase;
				yCase = vCase;
			}
			else
			{
				xCase = vCase;
				yCase = hCase;
			}
			
			// Si la case n'est pas un sol, alors le segment est obstrué
			if
			(
				! (this. carte [xCase, yCase] && (vSegment <= vCase + 0.5
				|| this. carte [xCase + direction. x, yCase + direction. y]))
			)
			{
				return false;
			}
		}
		
		
		// Toutes les cases où le segment passe sont des sols
		return true;
	}
	
	
	// Affichage du graphe
	
	public override string ToString ()
	{
		string sortie = "{\n";
		
		// Mise en sortie du dictionnaire d'adjacences
		foreach (Sommet sommet in this. dictionnaireAdjacences. Keys)
		{
			sortie +=
				"    "
				+ sommet
				+ " ["
			;
			foreach ((float cout, Sommet voisin) in dictionnaireAdjacences [sommet])
			{
				sortie +=
					"("
					+ cout
					+ ", "
					+ voisin
					+ "), "
				;
			}
			sortie = sortie. TrimEnd (new char [] {' ', ','});
			sortie += "]\n";
		}
		
		sortie += "}";
		
		return sortie;
	}
}
