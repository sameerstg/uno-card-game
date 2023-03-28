using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] Transform playerDroppedCardPosition;
    [SerializeField] Transform Bot1DroppedCardPosition;
    [SerializeField] Transform teammateDroppedCardPosition;
    [SerializeField] Transform Bot2DroppedCardPosition;
    public GameObject playedCard;
    public static DropZone dropZone;
    private void Awake()
    {
        dropZone = this;
    }
    public void OnDrop(PointerEventData eventData)
    {

        if (eventData.pointerDrag != null)
        {
            GameObject draggedObject = eventData.pointerDrag;
            draggedObject.transform.position = playerDroppedCardPosition.position;
            draggedObject.GetComponent<Card>().isDragable = false;
            draggedObject.transform.parent.gameObject.SetActive(false);
            draggedObject.transform.parent = this.transform;
        }
    }
}
