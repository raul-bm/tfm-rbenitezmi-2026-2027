using UnityEngine;
using UnityEngine.EventSystems;

public class LetterSpace : MonoBehaviour, IDropHandler
{
    public char letterAssociated;

    [HideInInspector] public bool isOccupied = false;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if(droppedObject != null)
        {
            LetterToDrag letter = droppedObject.GetComponent<LetterToDrag>();

            if(letter != null && !letter.isLocked && !isOccupied)
            {
                if(letter.letterAssociated == letterAssociated)
                {
                    letter.isLocked = true;
                    isOccupied = true;

                    droppedObject.transform.position = transform.position;
                    GameManager.Instance.IsCompleted();
                }
                else
                {
                    letter.ReturnToStart();
                }
            }
        }
    }
}
