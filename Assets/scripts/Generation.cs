using System;
using System. Collections;
using System. Collections. Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine. Tilemaps;


public class Generation
{
	// Constantes : paramètres de la génération aléatoire
	
    private const int chanceSolDebut = 72;
    private const int nbRepetitions = 3;
    private const int seuilSurvie = 5;
    private const int seuilNaissance = 7;
	
	
    public bool [,] carteSols;
	
	private System. Random r;
	
	
	
	// Constructeur : construit la carte de la mine
	
	public Generation ()
	{
		new Tuiles ();
		this. carteSols = new bool [Tuiles. longueur, Tuiles. hauteur];
		this. r = new System. Random ();
		
		this. initialiserCarte ();
	}
	
	
	/*
	 *  Affecte une valeur booléenne à chaque case de la carte :
	 *  "vrai" si la case est un sol, "faux" sinon
	 */
	
	private void initialiserCarte ()
	{
		double alea;
		
		for (int x = 0; x < Tuiles. longueur; x ++)
		for (int y = 0; y < Tuiles. hauteur; y ++)
		{
			// Initialisation à des valeurs aléatoires de départ
			alea = 100 * this. r. NextDouble ();
			this. carteSols [x, y] = alea < chanceSolDebut;
		}
		
		// Lissage de l'aléatoire d'un nombre choisi de fois
		for (int _ = 0; _ < nbRepetitions; _ ++)
		{
			this. repeterVie ();
		}
	}
	
	
	
	// Effectue 1 itération d'un automate cellulaire réaliste
	
	public void repeterVie ()
	{
		int
			nbSols,
			xVoisin,
			yVoisin
		;
		
		var carteNouvelle = new bool [Tuiles. longueur, Tuiles. hauteur];
		(int, int) [] voisinage =
		{
			(-1, -1),
			(-1, 0),
			(-1, 1),
			(0, -1),
			(0, 1),
			(1, -1),
			(1, 0),
			(1, 1)
		};
		
		
		// Pour chaque case,
		
		for (int x = 0; x < Tuiles. longueur; x ++)
		for (int y = 0; y < Tuiles. hauteur; y ++)
		{
			
			// Comptons ses voisins qui sont des sols
			
			nbSols = 0;
			
			foreach ((int dx, int dy) decalage in voisinage)
			{
				xVoisin = x + decalage. dx;
				yVoisin = y + decalage. dy;
				
				// Passons les voisins en dehors de la carte
				if
				(
					   xVoisin < 0
					|| xVoisin >= Tuiles. longueur
					|| yVoisin < 0
					|| yVoisin >= Tuiles. hauteur
				)
				{
					continue;
				}
				
				// Et ajoutons 1 si le voisin est un sol
				if (this. carteSols [xVoisin, yVoisin])
				{
					nbSols ++;
				}
			}
			
			// Le sol survit ssi. son nombre de sols voisins dépasse le seuil
			if (this. carteSols [x, y])
			{
				carteNouvelle [x, y] = nbSols >= seuilSurvie;
			}
			
			// Le mur fait naître un sol ssi. son nombre de sols voisins dépasse le seuil
			else
			{
				carteNouvelle [x, y] = nbSols >= seuilNaissance;
			}
		}
		
		
		this. carteSols = carteNouvelle;
	}
	
	
	
	// Affiche toutes les tuiles en fonction des valeurs de la carte
	
	public void changerTuiles ()
	{
		bool a, b, c, d, e, f, g, h;
		string nomTuile;
		
		
		// Pour chaque tuile,
		
		for (int x = 0; x < Tuiles. longueur; x ++)
		for (int y = 0; y < Tuiles. hauteur; y ++)
		{
			
			// Vérification du sol
			
			if (this. carteSols [x, y])
			{
				nomTuile = "sol";
			}
			
			
			// Vérification d'une case pleine
			
			else
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
				
				a = Tuiles. estSol (ref this. carteSols, x, y + 1);
				b = Tuiles. estSol (ref this. carteSols, x - 1, y);
				c = Tuiles. estSol (ref this. carteSols, x, y - 1);
				d = Tuiles. estSol (ref this. carteSols, x + 1, y);
				e = Tuiles. estSol (ref this. carteSols, x - 1, y + 1);
				f = Tuiles. estSol (ref this. carteSols, x - 1, y - 1);
				g = Tuiles. estSol (ref this. carteSols, x + 1, y - 1);
				h = Tuiles. estSol (ref this. carteSols, x + 1, y + 1);
				
				
				// Trouvons le nom de la tuile avec ces repères spatiaux
				
				nomTuile = "";
				
				// Vérification des murs
				if (a || b || c || d)
				{
					nomTuile += "mur";
					if (a) nomTuile += "A";
					if (b) nomTuile += "B";
					if (c) nomTuile += "C";
					if (d) nomTuile += "D";
				}
				
				// Vérification des coins
				if (!a && (e && !b || h && !d) || !c && (f && !b || g && !d))
				{
					nomTuile += "coin";
					if (e && ! (a || b)) nomTuile += "E";
					if (f && ! (c || b)) nomTuile += "F";
					if (g && ! (c || d)) nomTuile += "G";
					if (h && ! (a || d)) nomTuile += "H";
				}
				
				// Vérification de la roche entourée de murs
				if (! (a || b || c || d || e || f || g || h))
				{
					nomTuile = "roche";
				}
			}
			
			
			Tuiles. afficherTuile (x, y, nomTuile);
		}
	}
	
	
	
	// Détermine une case, aléatoire de fait, où placer nos personnages
	
	public Sommet placerJoueur ()
	{
		// Parcours de la ligne médiane de gauche à droite
		int y = Tuiles. hauteur / 2;
		for (int x = 0; x < Tuiles. longueur - 1; x ++)
		{
			// L'on prend ainsi le premier emplacement disponible
			if (this. carteSols [x, y])
			{
				return new Sommet (x, y);
			}
		}
		
		Debug. LogWarning ("Pas de sol");
		return new Sommet (-1, -1);
	}
	
	
	// Détermine une case, aléatoire de fait, où placer notre minerai d'or
	
	public Sommet placerOr ()
	{
		// Parcours de la ligne médiane de droite à gauche
		int y = Tuiles. hauteur / 2;
		for (int x = Tuiles. longueur - 1; x >= 0 ; x --)
		{
			// L'on prend ainsi le premier emplacement disponible
			if (this. carteSols [x, y])
			{
				return new Sommet (x, y);
			}
		}
		
		Debug. LogWarning ("Pas de sol");
		return new Sommet (-1, -1);
	}
}
