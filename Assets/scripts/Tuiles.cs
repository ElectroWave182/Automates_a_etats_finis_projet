using System;
using System. Collections;
using System. Collections. Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine. Tilemaps;


public class Tuiles
{
	public const int hauteur  = 36;
	public const int longueur = 64;
	
	private static Tilemap
		affichageMurs,
		affichageSols
	;
	
	
	public Tuiles ()
	{
		// Paramétrage de la carte des tuiles pour les murs
		affichageMurs = GameObject. Find ("mine/murs"). GetComponent <Tilemap> ();
		affichageMurs. origin = Vector3Int. zero;
		affichageMurs. size = new Vector3Int (longueur, hauteur, 1);
		
		// Paramétrage de la carte des tuiles pour les sols
		affichageSols = GameObject. Find ("mine/sols"). GetComponent <Tilemap> ();
		affichageSols. origin = Vector3Int. zero;
		affichageSols. size = new Vector3Int (longueur, hauteur, 1);
	}
	
	
	// Remplace toutes les tuiles par des tuiles vides
	
	public static void vider ()
	{
		affichageMurs. ClearAllTiles ();
		affichageSols. ClearAllTiles ();
	}
	
	
	/*
	 *  Remplace la tuile aux coordonnées (x, y)
	 *  par celle dont le nom est donné en paramètre
	 *  dans le dossier "Assets/tuiles/"
	 */
	
	public static void afficherTuile (int x, int y, string nom = "sol")
	{
		// Décalage dû à Unity
		var position = new Vector3Int
		(
			x - (longueur + 1) / 2,
			y - (hauteur + 1) / 2,
			0
		);
		
		var nouvelle = (Tile) AssetDatabase. LoadAssetAtPath ("Assets/tuiles/" + nom + ".asset", typeof (Tile));
		
		// Affichage sur la bonne carte
		if (nom. Substring (0, 3) != "sol")
		{
			affichageMurs. SetTile (position, nouvelle);
		}
		else
		{
			affichageSols. SetTile (position, nouvelle);
		}
	}
	
	
	/*
	 *  Détermine si une case donnée est un sol ou un mur ;
	 *  une case hors-limites est considérée comme un mur.
	 */
	
	public static bool estSol (ref bool [,] carteSols, int x, int y)
	{
		return
			   x >= 0
			&& y >= 0
			&& x < longueur
			&& y < hauteur
			&& carteSols [x, y]
		;
	}
}
