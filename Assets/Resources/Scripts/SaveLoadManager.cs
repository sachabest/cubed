using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Boomlagoon.JSON;

public class SaveLoadManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_WEBPLAYER
		Network.InitializeSecurity();
		MasterServer.RequestHostList("cubed");
#endif
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
	public Stack<Move> ParseJSONGameStateString(string json)
	{
		Stack<Move> stk = new Stack<Move>();
		JSONObject input = JSONObject.Parse(json);

		foreach (JSONValue val in input.GetArray("moves")) {
			JSONObject obj = val.Obj;
			Move move = new Move((PlayerManager.Faction) obj.GetNumber("faction"),(int)  obj.GetNumber("row") ,(int)  obj.GetNumber("col") ,(int) obj.GetNumber("tier"), (int) obj.GetNumber("removed_tier"));
			stk.Push(move);
		}

		GameInfo.instance.lifeUser = input.GetString ("Life");
		GameInfo.instance.industryUser = input.GetString ("Industry");
		return stk;
	}
}
