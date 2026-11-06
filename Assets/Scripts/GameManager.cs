using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Las palabras deben tener como máximo 7 letras")]
    [SerializeField] private string[] wordsList;

    [SerializeField] private GameObject letterSpaces;
    [SerializeField] private GameObject letterSpacePrefab;

    [SerializeField] private GameObject panelLettersToDrag;
    [SerializeField] private GameObject letterToDragPrefab;

    //DEBUG
    [SerializeField] private TextMeshProUGUI wordImageTextDebug;

    [Header("Variables ayuda")]
    [SerializeField] private float helpDelay = 15f;
    public float helpMoveDuration = 2.0f;

    private string actualChosenWord;
    private int actualChosenWordLength;

    private int actualLettersCorrect;

    private List<LetterSpace> activeSpaces = new List<LetterSpace>();
    private List<LetterToDrag> activeLetters = new List<LetterToDrag>();
    private float inactivityTimer = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        CreateLevel();
    }

    void Update()
    {
        inactivityTimer += Time.deltaTime;

        if(inactivityTimer >= helpDelay)
        {
            inactivityTimer = 0f;
            TriggerHelp();
        }
    }

    private void CreateLevel()
    {        
        actualChosenWord = wordsList[Random.Range(0, wordsList.Length)];
        actualChosenWordLength = actualChosenWord.Length;

        wordImageTextDebug.text = "Imagen: " + actualChosenWord;

        activeSpaces.Clear();
        activeLetters.Clear();

        // Instanciar los huecos y las letras esparcidas
        for(int i = 0; i < actualChosenWordLength; i++)
        {
            // Instanciar hueco
            GameObject newLetterSpace = Instantiate(letterSpacePrefab, letterSpaces.transform);
            newLetterSpace.GetComponent<LetterSpace>().letterAssociated = actualChosenWord[i];
            activeSpaces.Add(newLetterSpace.GetComponent<LetterSpace>());

            // Instanciar letra
            GameObject newLetterToDrag = Instantiate(letterToDragPrefab, panelLettersToDrag.transform);
            newLetterToDrag.GetComponent<LetterToDrag>().InitiateLetter(actualChosenWord[i]);
            activeLetters.Add(newLetterToDrag.GetComponent<LetterToDrag>());

            // Esparcerla aleatoriamente en el panel de letras que se pueden mover
            RectTransform letterRect = newLetterToDrag.GetComponent<RectTransform>();
            RectTransform panelLettersToDragRect = panelLettersToDrag.GetComponent<RectTransform>();

            float widthPanel = panelLettersToDragRect.rect.width;
            float heightPanel = panelLettersToDragRect.rect.height;
            float widthLetter = letterRect.rect.width;
            float heightLetter = letterRect.rect.height;

            float minX = (-widthPanel / 2f) + (widthLetter / 2f);
            float maxX = (widthPanel / 2f) - (widthLetter / 2f);
            float minY = (-heightPanel / 2f) + (heightLetter / 2f);
            float maxY = (heightPanel / 2f) - (heightLetter / 2f);

            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            letterRect.anchorMin = new Vector2(0.5f, 0.5f);
            letterRect.anchorMax = new Vector2(0.5f, 0.5f);

            letterRect.anchoredPosition = new Vector2(randomX, randomY);
        }

        // TODO: Animación de aparecer
    }

    private void TriggerHelp()
    {
        foreach(LetterSpace space in activeSpaces)
        {
            if(!space.isOccupied)
            {
                foreach(LetterToDrag letter in activeLetters)
                {
                    if(!letter.isLocked && letter.letterAssociated == space.letterAssociated)
                    {
                        space.isOccupied = true;
                        letter.AutoMoveToSpace(space);
                        return;
                    }
                }
            }
        }
    }

    private void ClearLevel()
    {
        // TODO: Animación de desaparecer con espera

        for (int i = letterSpaces.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(letterSpaces.transform.GetChild(i).gameObject);
        }
    }

    public void IsCompleted(bool comesFromHelp = false)
    {
        actualLettersCorrect++;
        
        if(!comesFromHelp) inactivityTimer = 0f;

        Debug.Log(actualLettersCorrect + "/" + actualChosenWordLength);

        if(actualLettersCorrect >= actualChosenWordLength)
        {
            Debug.Log("Palabra completada");
        }
    }
}
