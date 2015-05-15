using UnityEngine;
using System.Collections;

/// <summary>
/// This class serves as an interface to the player system, coordinating the two playing factions
/// as they participate in either single or multiplayer gmaes. 
/// </summary>
public class PlayerManager : MonoBehaviour {

	public enum Faction {
		Life = 1,
		Industry = 2
	}

	public class Player {
		public bool human;
		public short intRep;
		public Faction faction;

		public Player(bool human, short intRep, Faction faction) {
			this.human = human;
			this.intRep = intRep;
			this.faction = faction;
		}
	}

	public static Faction SwitchFaction(Faction input) {
		if (input == Faction.Life)
			return Faction.Industry;
		else 
			return Faction.Life;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
