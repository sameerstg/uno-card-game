using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  

public class LevelLoader : MonoBehaviour
{
 	public GameObject loadingScreen; 
	public Slider slider; 

	public void LoadLevel (int sceneIndex) 
	{ 
		 StartCoroutine(LoadAsync(sceneIndex)); 
		 while (SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("Splash Screen"))
		{

		 	if (PlayerPrefs.HasKey("HasDoneIntroUI"))
         		{

             		SceneManager.LoadScene("Main Menu");

         		}

         		else
         		{

            		 SceneManager.LoadScene("Intro UI");

         		}

		} 
	} 
	
	IEnumerator LoadAsync (int sceneIndex) 
	{ 
		
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
		loadingScreen.SetActive(true);

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / .9f);  
			slider.value = progress; 
			yield return new WaitForSeconds(5);
			yield return null; 
		} 
	} 
}
