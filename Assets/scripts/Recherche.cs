using System;
using System. Collections;
using System. Collections. Concurrent;
using System. Collections. Generic;
using UnityEngine;


public static class Recherche
{
	/*
	 *  Implémentation de l'algorithme de Dijkstra
	 *  qui renvoie le plus court chemin sous la forme d'un tableau de sommets,
	 *  le tout à partir d'un graphe fourni par dictionnaire d'adjacences,
	 *  d'un point de départ et d'un point d'arrivée.
	 */
	
	public static Sommet [] dijkstra (ref Dictionary <Sommet, List <(float, Sommet)>> graphe, Sommet depart, Sommet arrivee)
	{
		float
			coutPrecedent,
			coutSommet,
			coutSommetMin
		;
		
		Sommet? passage;
		Sommet
			sommetExplore,
			sommetRetour,
			voisin,
			voisinDepart,
			voisinExplore
		;
		
		var aExplorerFerme = new ConcurrentPriorityQueue <float, Sommet> ();
		var aExplorerOuvert = new HashSet <Sommet> ();
		var chemin = new List <Sommet> ();
		var coutsSommetsMin    = new Dictionary <Sommet, float> ();
		var passagesPrecedents = new Dictionary <Sommet, Sommet> ();
		
		
		// Réalisation d'une première exploration depuis le départ
		
		foreach ((float cout, Sommet voisin) areteDepart in graphe [depart])
		{
			voisinDepart = areteDepart. voisin;
			aExplorerOuvert. Add (voisinDepart);
		}
		
		
		// Initialisation des coûts à +l'infini, sauf pour le départ
		
		foreach (Sommet sommet in graphe. Keys)
		{
			coutsSommetsMin [sommet] = Single. PositiveInfinity;
		}
		
		aExplorerFerme. Enqueue (0, depart);
		coutsSommetsMin [depart] = 0;
		
		
		// Boucle principale d'exploration des sommets de moindre coûts
		
		while (aExplorerOuvert. Count != 0)
		{
			coutSommetMin = Single. PositiveInfinity;
			passage = null;
			sommetExplore = pop (ref aExplorerOuvert);
			voisinExplore = aExplorerFerme. Peek (). Value;
			
			// Exploration des voisins du sommet
			foreach ((float cout, Sommet sommet) arete in graphe [sommetExplore])
			{
				voisin = arete. sommet;
				coutPrecedent = coutsSommetsMin [voisin];
				coutSommet = coutPrecedent + arete. cout;
				
				// Un voisin à explorer a été trouvé
				if (coutPrecedent == Single. PositiveInfinity)
				{
					aExplorerOuvert. Add (voisin);
				}
				
				// Un meilleur chemin a été trouvé
				if (coutSommetMin > coutSommet)
				{
					coutSommetMin = coutSommet;
					passage = arete. sommet;
				}
			}
			
			// Erreur inattendue, car il existe toujours un chemin moins coûteux que +l'infini
			if (passage == null)
			{
				Debug. LogError ("Aucun passage n'a été trouvé.");
				UnityEditor. EditorApplication. isPlaying = false;
				Application. Quit ();
			}
			
			aExplorerFerme. Enqueue (coutSommetMin, sommetExplore);
			coutsSommetsMin [sommetExplore] = coutSommetMin;
			passagesPrecedents [sommetExplore] = passage. Value;
		}
		
		
		// Reconstruction du chemin
		
		sommetRetour = arrivee;
		
		while (sommetRetour != depart)
		{
			// Renvoi vide si l'enchaînement est brisé
			if (! passagesPrecedents. ContainsKey (sommetRetour))
			{
				Debug. LogWarning
				(
					"Arrivée "
					+ arrivee
					+ " non accessible depuis le départ "
					+ depart
					+ "."
				);
				return new Sommet [0];
			}
			
			chemin. Add (sommetRetour);
			sommetRetour = passagesPrecedents [sommetRetour];
		}
		
		chemin. Add (depart);
		chemin. Reverse ();
		
		return chemin. ToArray ();
	}
	
	
	// Retire un sommet de l'ensemble, et le renvoie
	
	public static Sommet pop (ref HashSet <Sommet> ensemble)
	{
		foreach (Sommet s in ensemble)
		{
			ensemble. Remove (s);
			return s;
		}
		
		// Erreur si l'ensemble est vide
		Debug. LogError ("Tentative de suppression dans un ensemble vide.");
		UnityEditor. EditorApplication. isPlaying = false;
		Application. Quit ();
		return new Sommet (0, 0);
	}
	
	
	
	/*
	 *  Implémentation de l'algorithme A*
	 *  qui renvoie le plus court chemin sous la forme d'un tableau de sommets,
	 *  le tout à partir d'un graphe fourni par dictionnaire d'adjacences,
	 *  d'un point de départ et d'un point d'arrivée.
	 */
	
	public static Sommet [] aEtoile (ref Dictionary <Sommet, List <(float, Sommet)>> graphe, Sommet depart, Sommet arrivee)
	{
		float coutSommet;
		
		Sommet
			sommetExplore,
			voisin,
			sommetRetour
		;
		
		var aExplorerFerme = new ConcurrentPriorityQueue <float, Sommet> ();
		var aExplorerOuvert = new HashSet <Sommet> ();
		var chemin = new List <Sommet> ();
		var coutsEstimesMin = new Dictionary <Sommet, float> ();
		var coutsSommetsMin = new Dictionary <Sommet, float> ();
		var passagesPrecedents = new Dictionary <Sommet, Sommet> ();
		
		
		// Initialisation des coûts à +l'infini, sauf pour le départ
		
		foreach (Sommet sommet in graphe. Keys)
		{
			coutsEstimesMin [sommet] = Single. PositiveInfinity;
			coutsSommetsMin [sommet] = Single. PositiveInfinity;
		}
		
		coutsEstimesMin [depart] = distanceVol (depart, arrivee);
		coutsSommetsMin [depart] = 0;
		aExplorerFerme. Enqueue (coutsEstimesMin [depart], depart);
		aExplorerOuvert. Add (depart);
		
		
		// Boucle principale d'exploration des sommets de moindre coûts
		
		while (aExplorerOuvert. Count != 0)
		{
			sommetExplore = aExplorerFerme. Dequeue (). Value;
			aExplorerOuvert. Remove (sommetExplore);
			
			// Arrêt si l'on a atteint l'arrivée
			if (sommetExplore == arrivee)
			{
				break;
			}
			
			// Exploration des voisins du sommet
			foreach ((float cout, Sommet sommet) arete in graphe [sommetExplore])
			{
				coutSommet = coutsSommetsMin [sommetExplore] + arete. cout;
				voisin = arete. sommet;
				
				// Un meilleur chemin a été trouvé
				if (coutsSommetsMin [voisin] > coutSommet)
				{
					coutsEstimesMin [voisin] = coutSommet + distanceVol (voisin, arrivee);
					coutsSommetsMin [voisin] = coutSommet;
					passagesPrecedents[voisin] = sommetExplore;
					
					if (! aExplorerOuvert. Contains (voisin))
					{
						aExplorerFerme. Enqueue (coutsEstimesMin [voisin], voisin);
						aExplorerOuvert. Add (voisin);
					}
				}
			}
		}
		
		
		// Reconstruction du chemin
		
		sommetRetour = arrivee;
		
		while (sommetRetour != depart)
		{
			// Renvoi vide si l'enchaînement est brisé
			if (! passagesPrecedents. ContainsKey (sommetRetour))
			{
				Debug. LogWarning
				(
					"Arrivée "
					+ arrivee
					+ " non accessible depuis le départ "
					+ depart
					+ "."
				);
				return new Sommet [0];
			}
			
			chemin. Add (sommetRetour);
			sommetRetour = passagesPrecedents [sommetRetour];
		}
		
		chemin. Add (depart);
		chemin. Reverse ();
		
		return chemin. ToArray ();
	}
	
	
	// Calcul de la distance à vol d'oiseau entre deux sommets
	
	public static float distanceVol (Sommet a, Sommet b)
	{
		return Convert. ToSingle (Math. Sqrt
		(
			Math. Pow (a. x - b. x, 2) +
			Math. Pow (a. y - b. y, 2)
		));
	}
}
