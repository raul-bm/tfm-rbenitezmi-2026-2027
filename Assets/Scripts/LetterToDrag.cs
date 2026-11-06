using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LetterToDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public char letterAssociated {  get; private set; }

    private Vector3 startPosition;
    private CanvasGroup canvasGroup;
    private Image bgImage;

    [HideInInspector] public bool isLocked = false;

    //DEBUG
    [SerializeField] private TextMeshProUGUI debugText;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        bgImage = GetComponent<Image>();
    }

    public void InitiateLetter(char letter)
    {
        letterAssociated = letter;

        debugText.text = letterAssociated.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        startPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        canvasGroup.blocksRaycasts = true;
    }

    public void ReturnToStart()
    {
        transform.position = startPosition;
    }

    public void AutoMoveToSpace(LetterSpace targetSpace)
    {
        isLocked = true;
        canvasGroup.blocksRaycasts = false;

        // Debug
        bgImage.color = Color.lightBlue;

        StartCoroutine(TravelRoutine(targetSpace.transform.position));
    }

    private IEnumerator TravelRoutine(Vector3 targetPosition)
    {
        Vector3 origin = transform.position;
        float elapsed = 0f;
        float duration = GameManager.Instance.helpMoveDuration;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(origin, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        // Debug
        bgImage.color = Color.white;

        GameManager.Instance.IsCompleted(true);
    }
}
