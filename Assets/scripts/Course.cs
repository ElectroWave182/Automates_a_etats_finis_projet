using System;
using System. Collections;
using System. Collections. Generic;
using UnityEngine;


public class Course: MonoBehaviour
{
	// Constantes
	
	private static readonly Color blanc = Color. white;
	private static readonly Color bleu = new Color (0.5f, 0.5f, 1);
	private const float modificateurVitesse = 1.5f;
	private const float tempsEffet    = 5;
	private const float tempsRecharge = 30;
	private const float vitesseBase = 2;
	
	
	// Attributs
	
	public float vitesse {get; set;}
	
	private bool estChoisi;
	private Coureur etat = Coureur. Suspend;
	private float effet;
	private float recharge = -1;
	private SpriteRenderer fleche;
	
	
	private enum Coureur
	{
		Suspend,
		Disponible,
		Courant,
		Fatigue
	}
	
	
	// Constructeur
	
	public static Course construire (Transform personnage)
	{
		Course c = personnage. gameObject. AddComponent <Course> ();
		
		c. fleche = personnage. Find ("indicateur"). GetComponent <SpriteRenderer> ();
		c. vitesse = vitesseBase;
		
		return c;
	}
	
	
	public void Update ()
	{
		switch (this. etat) 
		{
			case Coureur. Disponible:
				
				// Disponible -> Courant
				if (this. estChoisi && Input. GetKey (KeyCode. C))
				{
					this. etat = Coureur. Courant;
					this. effet = tempsEffet;
					this. modifierVitesse (1);
				}
				
				break;
			
			
			case Coureur. Courant:
				
				// Courant -> Fatigue
				if (this. effet <= 0)
				{
					this. etat = Coureur. Fatigue;
					this. recharge = tempsRecharge;
					this. modifierVitesse (-1);
					this. indiquer ();
				}
				
				// L'effet de course a une durée limitée
				this. effet -= Time. deltaTime;
				break;
			
			
			case Coureur. Fatigue:
				
				// Fatigue -> Disponible
				if (this. recharge <= 0)
				{
					this. etat = Coureur. Disponible;
					this. indiquer ();
				}
				
				// Le personnage doit patienter avant de pouvoir courir de nouveau
				this. recharge -= Time. deltaTime;
				break;
		}
	}
	
	
	// Affiche le temps restant de recharge toutes les secondes
	
	public void FixedUpdate ()
	{
		if (this. estChoisi && this. etat == Coureur. Fatigue)
			Debug. Log ("Temps restant : " + (int) this. recharge + " s.");
	}
	
	
	// Transition après le choix du personnage
	
	public void transitionLancement (bool choisi)
	{
		this. estChoisi = choisi;
		
		// Suspend -> Disponible
		if (this. recharge <= 0)
			this. etat = Coureur. Disponible;
		
		// Suspend -> Fatigue
		else
			this. etat = Coureur. Fatigue;
		
		this. indiquer ();
	}
	
	
	// Transition après le changement de mine
	
	public void transitionArrivee ()
	{
		// Stoppe la course en réinitialisant les décomptes et les vitesses
		if (this. etat == Coureur. Courant)
		{
			this. effet    = tempsEffet;
			this. recharge = tempsRecharge;
			this. vitesse = vitesseBase;
		}
		
		// * -> Suspend
		this. etat = Coureur. Suspend;
	}
	
	
	// Fait accélérer ou ralentir le personnage choisi
	
	private void modifierVitesse (int exposant)
	{
		// Le personnage accélère si l'exposant est positif, et ralentit sinon
		this. vitesse *= Convert. ToSingle (Math. Pow (modificateurVitesse, exposant));
	}
	
	
	// Change de couleur la flèche au dessus du personnage choisi
	
	private void indiquer ()
	{
		switch (this. etat)
		{
			// Filtrée en bleu s'il est fatigué
			case Coureur. Fatigue:
				this. fleche. color = bleu;
				break;
			
			// Non filtrée s'il peut courir
			case Coureur. Disponible:
				this. fleche. color = blanc;
				break;
		}
	}
}
