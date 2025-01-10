using System;
using System. Collections;
using System. Collections. Generic;
using UnityEngine;


public struct Sommet
{
	// Attributs
	
    public int x {get;}
    public int y {get;}
	
	
	// Constructeur
	
	public Sommet (int xp, int yp)
	{
		this. x = xp;
		this. y = yp;
	}
	
	
	// Affichage
	
	public override string ToString ()
	{
		return
			"("
			+ this. x
			+ ", "
			+ this. y
			+ ")"
		;
	}
	
	
	// Opérateurs d'égalité
	
	public static bool operator == (Sommet a, Sommet b)
    {
        return
			a. x == b. x &&
			a. y == b. y
		;
    }
	
	public static bool operator != (Sommet a, Sommet b)
    {
        return ! (a == b);
    }
	
	public override bool Equals (object autre)
	{
		return this == (Sommet) autre;
	}
	
	public override int GetHashCode()
	{
		return
			this. x. GetHashCode () ^
			this. y. GetHashCode ()
		;
	}
	
	
	// Opérateurs d'inégalité
	
	public static bool operator < (Sommet a, Sommet b)
    {
        return
			a. x < b. x ||
			a. y < b. y && a. x == b. x
		;
    }
	
	public static bool operator <= (Sommet a, Sommet b)
    {
        return
			a < b ||
			a == b
		;
    }
	
	public static bool operator > (Sommet a, Sommet b)
    {
        return ! (a <= b);
    }
	
	public static bool operator >= (Sommet a, Sommet b)
    {
        return
			a > b ||
			a == b
		;
    }
	
	
	// Calcule la distance avec un autre sommet
	
	public float distance (Sommet autre)
	{
		return Convert. ToSingle (Math. Sqrt
		(
			Math. Pow (this. x - autre. x, 2) +
			Math. Pow (this. y - autre. y, 2) 
		));
	}
	
	
	// Retourne le déplacement depuis l'origine
	
	public Vector3 obtenirDeplacement ()
	{
		return new Vector3 (this. x, this. y, 0) * Jeu. tailleCase;
	}
}
