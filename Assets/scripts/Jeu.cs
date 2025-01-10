using System;
using System. Collections;
using System. Collections. Generic;
using UnityEngine;


public class Jeu: MonoBehaviour
{
	public static float tailleCase;
	
	
	private Course
		capaciteMary,
		capaciteWill
	;
	private Dictionary <Sommet, List <(float, Sommet)>> chemins;
	private float
		distanceMary,
		distanceWill,
		periode
	;
	private GameObject texte;
	private int
		indiceDijkstra,
		indiceAetoile,
		pied
	;
	private Sommet []
		cheminDijkstra,
		cheminAetoile
	;
	private SpriteRenderer
		dessinMary,
		dessinWill
	;
	private string etat;
	private Transform
		personnages,
		mary,
		will
	;
	private Vector3
		directionMary,
		directionWill
	;



	public void Start ()
	{
		// Affectation des composants
		
		this. texte = GameObject. Find ("introduction");
		
		this. personnages = transform. Find ("personnages");
		this. mary = this. personnages. Find ("mary");
		this. will = this. personnages. Find ("will");
		
		this. dessinMary = this. mary. GetComponent <SpriteRenderer> ();
		this. dessinWill = this. will. GetComponent <SpriteRenderer> ();
		
		// Capacités des personnages
		this. capaciteMary = Course. construire (this. mary);
		this. capaciteWill = Course. construire (this. will);
		
		Camera camera = Camera. main;
		float hauteur = camera. orthographicSize * 2f;
		tailleCase = hauteur / Tuiles. hauteur;
		
		
		// L'on génère une première fois la mine
		
		this. regenerer ();
		
		this. etat = "introduction";
	}
	
	
	public void Update ()
	{
		// Lecture du texte d'introduction
		
		if (this. etat == "introduction" && Input. anyKey)
		{
			this. texte. SetActive (false);
			this. personnages. gameObject. SetActive (true);
			this. etat = "entree";
		}
		
		
		// Attente du choix du personnage
		
		if (this. etat == "entree")
		{
			// Le joueur a choisi Mary (Dijkstra)
			if (Input. GetKey (KeyCode. M))
			{
				this. mary. Find ("indicateur"). gameObject. SetActive (true);
				this. mary. Translate (new Vector2 (0, 0.2f));
				this. etat = "deplacement";
				this. capaciteMary. transitionLancement (true);
				this. capaciteWill. transitionLancement (false);
			}
			
			// Le joueur a choisi Will (A*)
			if (Input. GetKey (KeyCode. W))
			{
				this. will. Find ("indicateur"). gameObject. SetActive (true);
				this. mary. Translate (new Vector2 (0, 0.2f));
				this. etat = "deplacement";
				this. capaciteMary. transitionLancement (false);
				this. capaciteWill. transitionLancement (true);
			}
		}
		
		
		// Déplacement des personnages
		
		if (this. etat == "deplacement")
		{
			/*
			 *  Une fois le personnage arrivé à un sommet,
			 *  il prend pour objectif le prochain sommet
			 */
			
			if (this. distanceMary <= 0)
			{
				this. indiceDijkstra ++;
			}
			
			if (this. distanceWill <= 0)
			{
				this. indiceAetoile ++;
			}
			
			
			bool finMary = this. indiceDijkstra >= this. cheminDijkstra. Length;
			bool finWill = this. indiceAetoile  >= this. cheminAetoile.  Length;
			
			periode = Time. deltaTime;
			float translation;
			
			
			// Mary s'arrête une fois arrivée au minerai d'or
			
			if (finMary)
			{
				this. mary. tag = "standing";
			}
			
			else
			{
				// Sinon elle se tourne une fois arrivée à un sommet
				if (this. distanceMary <= 0)
				{
					Sommet prochain = this. cheminDijkstra [this. indiceDijkstra];
					Vector3 objectif = this. tourner (ref this. mary, prochain);
					this. directionMary = objectif. normalized;
					this. distanceMary  = objectif. magnitude;
				}
				
				// Et continue d'avancer
				translation = this. capaciteMary. vitesse * periode;
				this. mary. Translate (this. directionMary * translation);
				this. distanceMary -= translation;
			}
			
			
			// Will s'arrête une fois arrivé au minerai d'or
			
			if (finWill)
			{
				this. will. tag = "standing";
			}
			
			else
			{
				// Sinon il se tourne une fois arrivé à un sommet
				if (this. distanceWill <= 0)
				{
					Sommet prochain = this. cheminAetoile [this. indiceAetoile];
					Vector3 objectif = this. tourner (ref this. will, prochain);
					this. directionWill = objectif. normalized;
					this. distanceWill  = objectif. magnitude;
				}
				
				// Et continue d'avancer
				translation = this. capaciteWill. vitesse * periode;
				this. will. Translate (this. directionWill * translation);
				this. distanceWill -= translation;
			}
			
			
			// Si les 2 personnages sont arrivés,
			
			if (finMary && finWill)
			{
				// Nouvelle mine
				Tuiles. vider ();
				this. regenerer ();
				
				this. mary. Find ("indicateur"). gameObject. SetActive (false);
				this. will. Find ("indicateur"). gameObject. SetActive (false);
				this. etat = "entree";
				this. capaciteMary. transitionArrivee ();
				this. capaciteWill. transitionArrivee ();
			}
		}
	}
	
	
	// Rafraichit l'animation des personnages toutes les 0,1 s.
	
	public void FixedUpdate ()
	{
		animer ("mary", ref this. mary, ref this. dessinMary);
		animer ("will", ref this. will, ref this. dessinWill);
		this. pied = 3 - this. pied;
	}
	
	
	
	// Régénère la mine aléatoirement, et tout ce qui va avec
	
	private void regenerer ()
	{
		Debug. Log ("- Graphe -");
		
		
		// Réinitialisation
		
		this. indiceDijkstra = 0;
		this. indiceAetoile  = 0;
		
		this. distanceMary = 0;
		this. distanceWill = 0;
		
		Generation alea = null;
		Sommet depart  = new Sommet (-1, -1);
		Sommet arrivee = new Sommet (-1, -1);
		
		
		/*
		 *  L'on régénère les cases tant que le graphe ne convient pas :
		 *  l'or doit être accessible depuis le point de départ des personnages
		 */
		
		while
		(
			depart == new Sommet (-1, -1)
			|| this. cheminDijkstra. Length == 0
			|| this. cheminAetoile.  Length == 0
		)
		{
			// Nouvelle mine
			alea = new Generation ();
			bool [,] carte = alea. carteSols;
			depart = alea. placerJoueur ();
			arrivee = alea. placerOr ();
			
			// Nouveaux chemins
			this. chemins = new Graphe (ref carte, depart, arrivee). dictionnaireAdjacences;
			this. cheminDijkstra = Recherche. dijkstra (ref this. chemins, depart, arrivee);
			this. cheminAetoile  = Recherche. aEtoile  (ref this. chemins, depart, arrivee);
		}
		
		
		// Affichage graphique
		alea. changerTuiles ();
		Tuiles. afficherTuile (arrivee. x, arrivee. y, "sol_or");
		
		// Plaçons les personnages à leur position de départ
		Vector2 positionDepart = depart. obtenirDeplacement ();
		this. mary. localPosition = positionDepart;
		this. will. localPosition = positionDepart;
		this. mary. Translate (new Vector2 (0, -0.2f));
		
		this. pied = 1;
	}
	
	
	
	// Oriente l'image du personnage dans la direction où il se déplace
	
	private Vector3 tourner (ref Transform personnage, Sommet passage)
	{
		Vector3 objectif = passage. obtenirDeplacement () - personnage. localPosition;
		
		float angleA = Vector3. Angle (objectif, new Vector3 (-1, 1, 0));
		float angleB = Vector3. Angle (objectif, new Vector3 (1, 1, 0));
		
		switch (2 * enleverAnglePlat (angleA) + enleverAnglePlat (angleB))
		{
			// Tourner vers le haut
			case 0:
				personnage. tag = "up";
				break;
			
			// Tourner vers la gauche
			case 1:
				personnage. tag = "left";
				break;
			
			// Tourner vers la droite
			case 2:
				personnage. tag = "right";
				break;
			
			// Tourner vers le bas
			case 3:
				personnage. tag = "down";
				break;
			
			default:
				Debug. LogError ("Angle formaté supérieur à 3");
				UnityEditor. EditorApplication. isPlaying = false;
				Application. Quit ();
				break;
		}
		
		return objectif;
	}
	
	
	/*
	 *  Retourne 0 si l'angle est dans [0 ; 90[
	 *  Retourne 1 si l'angle est dans [90 ; 180]
	 *  (enlève le cas d'erreur d'angle plat de la fonction Vector3. Angle)
	 */
	
	private static int enleverAnglePlat (float angle)
	{
		angle /= 90;
		return (int) angle - (int) (angle / 2);
	}
	
	
	
	// Animation des personnages
	
	private void animer (string nom, ref Transform objet, ref SpriteRenderer dessin)
	{
		string chemin = nom + "/";
		
		switch (objet. tag)
		{
			case "standing":
				dessin. sprite = Resources. Load <Sprite> (chemin + nom + "_down_standing");
				break;
				
			case "up":
				dessin. sprite = Resources. Load <Sprite> (chemin + nom + "_up_foot" + this. pied);
				break;
				
			case "left":
				dessin. sprite = Resources. Load <Sprite> (chemin + nom + "_left_foot" + this. pied);
				break;
				
			case "right":
				dessin. sprite = Resources. Load <Sprite> (chemin + nom + "_right_foot" + this. pied);
				break;
				
			case "down":
				dessin. sprite = Resources. Load <Sprite> (chemin + nom + "_down_foot" + this. pied);
				break;
		}
	}
}
