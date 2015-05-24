using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using Boomlagoon.JSON;

public class SaveLoadManager : MonoBehaviour {

	public Stack<Move> moves;

	public static SaveLoadManager instance;
	// Use this for initialization
	void Start () {

	}
	void Awake() {
		instance = this;
	}
	public byte[] Encode(string json) {
		return UTF8Encoding.UTF8.GetBytes(json);
	}
	public string CreateJSONGameStateString(Stack<Move> moves) {
		JSONArray moveArr = new JSONArray();
		foreach (Move move in moves) {
			JSONObject moveJSON = new JSONObject();
			moveJSON.Add("faction",(int) move.getFaction());
			moveJSON.Add("row", move.getRow());
			moveJSON.Add("col", move.getCol());
			moveJSON.Add("tier", move.getTier());
			moveJSON.Add("removed_tier", move.getRemovedTier());
			moveArr.Add(moveJSON);
		}
		JSONObject final = new JSONObject ();
		final.Add ("Life", GameInfo.instance.lifeUser);
		final.Add ("Industry", GameInfo.instance.industryUser);
		final.Add ("Moves", moveArr);
		return final.ToString ();
	}
	public void ParseJSONGameStateString(string json)
	{
		Stack<Move> stk = new Stack<Move>();
		JSONObject input = null;
		try {
			input = JSONObject.Parse(json);

			foreach (JSONValue val in input.GetArray("moves")) {
				JSONObject obj = val.Obj;
				Move move = new Move((PlayerManager.Faction) obj.GetNumber("faction"),(int)  obj.GetNumber("row") ,(int)  obj.GetNumber("col") ,(int) obj.GetNumber("tier"), (int) obj.GetNumber("removed_tier"));
				stk.Push(move);
			}

		} catch (Exception e) {
			Debug.Log("Couldn't parse moves JSON ");
		} try {
			GameInfo.instance.lifeUser = input.GetString ("Life");
			GameInfo.instance.industryUser = input.GetString ("Industry");
		} catch (Exception ex) {
			Debug.Log("Couldn't get player names from saved data. ");
			GameInfo.instance.lifeUser = "";
			GameInfo.instance.industryUser = "";
		}
		// this to handle the caee when a game invitation is sent - the original sender
		// does not know who the opponenet is yet, but the receiver of that inviatation
		// will obviously know, as it is him/hersel. This will then propogate back to the 
		// original user on the next turn save
		// if (GameInfo.instance.lifeUser.Equals(GCCubedListener.UNDEF_OPPONENT)) {
		// 	GameInfo.instance.lifeUser = GCCubedListener.instance.localName();
		// } else if (GameInfo.instance.industryUser.Equals(GCCubedListener.UNDEF_OPPONENT)) {
		// 	GameInfo.instance.industryUser = GCCubedListener.instance.localName();
		// }
		moves = stk;

	}
}
