using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoneIntroUI : MonoBehaviour
{
	public void DoneIntro()
	{ 
		PlayerPrefs.SetInt("HasDoneIntroUI", 1); 

	} 
}
