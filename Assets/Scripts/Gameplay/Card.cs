using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public GameObject CardPrefab;
    public string cardID;
    public int hokmPower, sunPower;
    public int hokmRank, sunRank;
    public bool isSunGame, isHokmGame;
    public bool isPlayable;
    public bool isDragable;
    [SerializeField] bool faceUp;
    public enum suite{Ace,Black,Heart,Tekri};
    public suite suites; 
    

    public void FlipCard()
    {
        transform.GetChild(0).gameObject.SetActive(faceUp);
        transform.GetChild(1).gameObject.SetActive(!faceUp);
    }
    public int GetRank()
    {
        return 0;
        if (isHokmGame == true)
        {

        }
        else if (isSunGame == true)
        {

        }
    }

    public int GetPower()
    {
        return 0;
        if (isHokmGame == true)
        {

        }
        else if (isSunGame == true)
        {

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDragable) return;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragable) return;

        transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragable) return;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        StartCoroutine(ReturnToHand());

    }

    IEnumerator ReturnToHand()
    {
        yield return new WaitForSeconds(0.15f);
        while (isDragable && transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 2200f * Time.deltaTime);
            yield return new WaitForEndOfFrame();

        }
    }


}
