using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using Photon.Pun; 

public class ServerCheck : MonoBehaviourPunCallbacks
{
	public GameObject connectingUI; 
	public GameObject faildUI; 

		public override void OnConnectedToMaster()

		{ 

			SceneManager.LoadScene("Main Menu");

		} 


		public void OnConnectionFail()

		{ 
			faildUI.SetActive(true);
			connectingUI.SetActive(false);

		} 

	 


		void Start()

      	{

			PhotonNetwork.ConnectUsingSettings();

		}


		public void RetryLoad() 
		{ 
	
			PhotonNetwork.ConnectUsingSettings();

		} 


  
}
