using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class IntroSkip : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         if (PlayerPrefs.HasKey("HasDoneIntroUI"))
         {
             SceneManager.LoadScene("Main Menu");
         }
         else
         {
             SceneManager.LoadScene("Intro UI");
         }
 
         //When the player finish the tutorial you must run:
         PlayerPrefs.SetString("HasDoneTuorial", "yes");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
