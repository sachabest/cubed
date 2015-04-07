using UnityEngine;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

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
		int stringLength = moves.Count * 14 + 25;
		StringBuilder json = new StringBuilder("{\"moves\": [", stringLength);
		foreach (Move move in moves) {
			json.Append("{");
			json.Append(move.getPlayer().ToString());
			json.Append(",");
			json.Append(move.getRow().ToString());
			json.Append(",");
			json.Append(move.getCol().ToString());
			json.Append(",");
			json.Append(move.getTier().ToString());
			json.Append(",");
			json.Append(move.getRemovedTier().ToString());
			json.Append("},");
		}
		json.Remove(json.Length-1, 1);
		json.Append("]}");
		return json.ToString();
	}
	public Stack ParseJSONGameStateString(string json)
	{
		Stack stk = new Stack();
		if (json == null || json.Length < 15)
			return stk;
		json = json.Substring(12,json.Length-15);
		Debug.Log ("JSON: " + json);
		string [] moves = Regex.Split(json,"},{");
		foreach (string str in moves)
		{
			string[] temp = str.Split(',');
			Move move = new Move(int.Parse(temp[0]),int.Parse(temp[1]),int.Parse(temp[2]),int.Parse(temp[3]),int.Parse(temp[4]));
			stk.Push(move);
		}
		return stk;
	}
}
