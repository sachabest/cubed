using UnityEngine;
using System.Collections;
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
	public string CreateJSONGameStateString(Stack moves) {
		JSONArray moveArr = new JSONArray();
		foreach (Move move in moves) {
			JSONObject moveJSON = new JSONObject();
			moveJSON.Add("faction", move.getFaction());
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
	public Stack ParseJSONGameStateString(string json)
	{
		Stack stk = new Stack();
		JSONObject input = new JSONObject (json);

		foreach (JSONObject obj in input.GetArray("moves")) {
			Move move = new Move((PlayerManager.Faction) obj.GetNumber("faction"), obj.GetNumber("row") , obj.GetNumber("col") ,obj.GetNumber("tier"), obj.GetNumber("removed_tier"));
			stk.Push(move);
		}

		GameInfo.instance.lifeUser = input.GetString ("Life");
		GameInfo.instance.industryUser = input.GetString ("Industry");
		return stk;
	}
}
