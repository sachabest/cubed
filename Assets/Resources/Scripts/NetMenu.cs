using UnityEngine;
using System.Collections;

public class NetMenu : MonoBehaviour
{
	private bool lan, playerConnected;
	private enum guiState {networkPrompt, networkErrorPrompt, hostsPrompt, noHostsPrompt, lanPrompt, waitPrompt};
	private int currentGuiState, currentNetworkState, dotCount;
	private enum networkState {local, noPunch, internet};
	private string ip, networkPromptText, dots, otherPlayer;
	private HostData[] hosts;
	
	// Use this for initialization
	
	void Start () {
		networkPromptText = "Would you like to play on LAN or the internet?";
		currentGuiState = (int) guiState.networkPrompt;
		currentNetworkState = (int) networkState.internet;
		dotCount = 0;
		InvokeRepeating("DotDotDot", 0, 1);
		if (Network.TestConnection() == ConnectionTesterStatus.Error) { //temp
			currentNetworkState = (int) networkState.local;
			networkPromptText = "You have no internet connection. Would you like to play on LAN?";
		}
		if (Network.TestConnectionNAT() == ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted) { //temp
			currentNetworkState = (int) networkState.noPunch;
			networkPromptText = "Incorrect NAT settings. Check your router";
		}
		Network.InitializeSecurity();
	}
	void OnGUI() {
		if (currentGuiState == (int) guiState.networkPrompt) {
			string posButton = "", negButton = "";
			if (currentNetworkState == (int) networkState.internet) {
				posButton = "Internet";
				negButton = "LAN";
			}
			else if (currentNetworkState == (int) networkState.local) {
				posButton = "Yes";
				negButton = "No";
			}
			else if (currentNetworkState == (int) networkState.noPunch) {
				posButton = "Okay";
				negButton = "Quit";
			}
			GUILayout.BeginArea(new Rect(Screen.width/2-250, Screen.height/2-250, 500, 500));
			GUILayout.BeginVertical();
			GUILayout.Box(networkPromptText);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(posButton)) {
				lan = false;
				MasterServer.RequestHostList("Cubed");
				hosts = MasterServer.PollHostList();
				if (hosts.Length > 0)
					currentGuiState = (int) guiState.hostsPrompt;
				else
					currentGuiState = (int) guiState.noHostsPrompt;
				//Application.LoadLevel("Cubed");
			}
			if (GUILayout.Button(negButton)) {
				lan = true;
				currentGuiState = (int) guiState.lanPrompt;
				//Application.LoadLevel("Cubed");
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Back"))
				Application.LoadLevel("MainMenu");
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		else if (currentGuiState == (int) guiState.hostsPrompt) {
			foreach (HostData element in hosts) {
				GUILayout.BeginHorizontal();	
				string name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
				GUILayout.Label(name);	
				GUILayout.Space(5);
				string hostInfo;
				hostInfo = "[";
				foreach (string host in element.ip)
					hostInfo = hostInfo + host + ":" + element.port + " ";
				hostInfo = hostInfo + "]";
				GUILayout.Label(hostInfo);	
				GUILayout.Space(5);
				GUILayout.Label(element.comment);
				GUILayout.Space(5);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Connect")) {
					// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
					Network.Connect(element);			
				}
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button("Host")) {
				Network.InitializeServer(2, 25001, Network.HavePublicAddress());
			}
			GUILayout.EndArea();
		}
		else if (currentGuiState == (int) guiState.noHostsPrompt) {
			GUILayout.BeginArea(new Rect(Screen.width/2 - 250, Screen.height/2 - 250, 500, 500));
			GUILayout.BeginVertical();
			GUILayout.Box("There are no hosts currently online.\nWould you like to host one?");
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Yes")) {
				Network.InitializeServer(2, 25001, Network.HavePublicAddress());
				currentGuiState = (int) guiState.waitPrompt;
			}
			if (GUILayout.Button("No")) {
				Debug.Log("In the No loop");
				Application.Quit();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		else if (currentGuiState == (int) guiState.lanPrompt) {
			GUILayout.BeginArea(new Rect(Screen.width/2 - 250, Screen.height/2 - 250, 500, 500));
			GUILayout.BeginVertical();
			GUILayout.Box("LAN Game Setup\nYou can either host your own server or connect to one\nYour IP is "
				+ Network.player.ipAddress + " with port " + Network.connectionTesterPort);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Host")) {
				Network.InitializeServer(2, 25001, false);
				currentGuiState = (int) guiState.waitPrompt;
			}
			if (GUILayout.Button("Connect")) {
				currentGuiState = (int) guiState.hostsPrompt;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		else if (currentGuiState == (int) guiState.waitPrompt) {
			GUILayout.BeginArea(new Rect(Screen.width/2 - 250, Screen.height/2 - 250, 500, 500));
			GUILayout.BeginVertical();
			GUILayout.Box("Waiting for connection" + dots + otherPlayer);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Cancel")) {
				currentGuiState = (int) guiState.networkPrompt;
				Network.Disconnect();
				playerConnected = false;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
	void OnPlayerConnected() {
		playerConnected = true;
		otherPlayer = Network.connections[0].guid;
	}
	void OnPlayerDisconnected() {
		playerConnected = false;
		otherPlayer = null;
	}
	// can be easily modified to return a string instead of setting an instance variable
	void DotDotDot() {
		string returned;
		if (dotCount == 0) returned = ".";
		else if (dotCount == 1) returned = "..";
		else returned = "...";
		dotCount++;
		dotCount = dotCount % 3;
		dots = returned;
	}	
	void OnServerInitialized() {
		Debug.Log("Server Initialized");
		ip = Network.connectionTesterIP; //may have to be Network.natFacilitatorIP
		MasterServer.RegisterHost("Cubed", ip);
		Debug.Log("Host Registered");
	}
	// Update is called once per frame
	void Update ()
	{
	}
}

