using System;
using UnityEngine;
using UnityEngine.UI;

// Creates the border around the brid and adjusts the camera position to make sure everything fits on screen.
// Also position UI here
[RequireComponent(typeof(GameManager))]
public class BoardDecorator : MonoBehaviour, ResizeListener.UiResizedListener
{
    private int width;
    private int height;
    private int boardCenter;
    private GameObject pipe;
    private GameObject pipeCorner;

    private float paddingVertical = 1f;
    private float rightSideUiWidth = 8f;
    private float paddingHorizontal = 2f;
    private float paddingButtons = 1;

    [Header("UI")]
    public RectTransform levelPanel;
    public RectTransform statusPanel;
    public RectTransform scorePanel;
    public RectTransform movementPanel;
    public RectTransform rotationPanel;
    // this one is now referenced here and in gamemanager, not ideal?
    public GameObject panelStatus;

    float cameraLeft = 0;
    float cameraRight = 0;

    float movementPanelWidth = 9;
    float movementPanelHeight = 3;
    float rotationPanelWidth = 6;
    float rotationPanelHeight = 3;

    private enum Orientation { PORTRAIT, LANDSCAPE };

    void Awake()
    {
        ResizeListener resizeListener = FindObjectOfType(typeof(ResizeListener)) as ResizeListener;
        resizeListener.AddListener(this);

        panelStatus.SetActive(false);
    }

    public void SetValues(int width, int height, GameObject pipe, GameObject pipeCorner)
    {
        this.width = width;
        this.height = height;
        boardCenter = width / 2;

        this.pipe = pipe;
        this.pipeCorner = pipeCorner;

        Setup();
    }



    public void Setup()
    {
        SetupPipeBorders();



        SetupUiAndCamera();
    }

    public void OnUiResized()
    {
        SetupUiAndCamera();
    }

    private void SetupUiAndCamera()
    {
        SetupCamera();

        PlaceHud();

        PlaceButtons();
    }

    private void SetupPipeBorders()
    {

        // Horizontal pipes
        for (int i = 0; i < width; i++)
        {
            GameObject.Instantiate(pipe, new Vector3(i, -1, 0), Quaternion.AngleAxis(90, Vector3.forward));

            if (i < boardCenter - 2 || i > boardCenter + 1)
            {
                GameObject.Instantiate(pipe, new Vector3(i, height, 0), Quaternion.AngleAxis(270, Vector3.forward));
            }
        }

        // Vertical pipes
        for (int i = 0; i < height; i++)
        {
            GameObject.Instantiate(pipe, new Vector3(-1, i, 0), Quaternion.identity);
            GameObject.Instantiate(pipe, new Vector3(width, i, 0), Quaternion.AngleAxis(180, Vector3.forward));
        }

        // Corners
        GameObject.Instantiate(pipeCorner, new Vector3(-1, -1, 0), Quaternion.identity);
        GameObject.Instantiate(pipeCorner, new Vector3(-1, height, 0), Quaternion.AngleAxis(270, Vector3.forward));
        GameObject.Instantiate(pipeCorner, new Vector3(width, -1, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(pipeCorner, new Vector3(width, height, 0), Quaternion.AngleAxis(180, Vector3.forward));

        // Top opening
        GameObject.Instantiate(pipeCorner, new Vector3(boardCenter - 2, height, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(pipeCorner, new Vector3(boardCenter + 1, height, 0), Quaternion.identity);

        GameObject.Instantiate(pipe, new Vector3(boardCenter - 2, height + 1, 0), Quaternion.identity);
        GameObject.Instantiate(pipe, new Vector3(boardCenter + 1, height + 1, 0), Quaternion.identity);

        // Top pipes from original game, ugly and take up too much space
        // GameObject.Instantiate(pipeCorner, new Vector3(boardCenter - 2, height + 2, 0), Quaternion.AngleAxis(180, Vector3.forward));
        // GameObject.Instantiate(pipeCorner, new Vector3(boardCenter + 1, height + 2, 0), Quaternion.AngleAxis(270, Vector3.forward));
        // GameObject.Instantiate(pipeCorner, new Vector3(boardCenter - 3, height + 2, 0), Quaternion.identity);
        // GameObject.Instantiate(pipeCorner, new Vector3(boardCenter + 2, height + 2, 0), Quaternion.AngleAxis(90, Vector3.forward));
        // GameObject.Instantiate(pipe, new Vector3(boardCenter - 3, height + 3, 0), Quaternion.identity);
        // GameObject.Instantiate(pipe, new Vector3(boardCenter + 2, height + 3, 0), Quaternion.identity);

        // Around pill preview
        GameObject.Instantiate(pipe, new Vector3(width + 3, height - 3), Quaternion.identity);
        GameObject.Instantiate(pipe, new Vector3(width + 3, height - 2), Quaternion.identity);
        GameObject.Instantiate(pipe, new Vector3(width + 3, height - 1), Quaternion.identity);

        GameObject.Instantiate(pipe, new Vector3(width + 8, height - 3), Quaternion.AngleAxis(180, Vector3.forward));
        GameObject.Instantiate(pipe, new Vector3(width + 8, height - 2), Quaternion.AngleAxis(180, Vector3.forward));
        GameObject.Instantiate(pipe, new Vector3(width + 8, height - 1), Quaternion.AngleAxis(180, Vector3.forward));

        GameObject.Instantiate(pipe, new Vector3(width + 4, height - 4, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(pipe, new Vector3(width + 5, height - 4, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(pipe, new Vector3(width + 6, height - 4, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(pipe, new Vector3(width + 7, height - 4, 0), Quaternion.AngleAxis(90, Vector3.forward));

        GameObject.Instantiate(pipe, new Vector3(width + 4, height, 0), Quaternion.AngleAxis(270, Vector3.forward));
        GameObject.Instantiate(pipe, new Vector3(width + 5, height, 0), Quaternion.AngleAxis(270, Vector3.forward));
        GameObject.Instantiate(pipe, new Vector3(width + 6, height, 0), Quaternion.AngleAxis(270, Vector3.forward));
        GameObject.Instantiate(pipe, new Vector3(width + 7, height, 0), Quaternion.AngleAxis(270, Vector3.forward));

        GameObject.Instantiate(pipeCorner, new Vector3(width + 3, height - 4, 0), Quaternion.identity);
        GameObject.Instantiate(pipeCorner, new Vector3(width + 3, height, 0), Quaternion.AngleAxis(270, Vector3.forward));
        GameObject.Instantiate(pipeCorner, new Vector3(width + 8, height - 4, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(pipeCorner, new Vector3(width + 8, height, 0), Quaternion.AngleAxis(180, Vector3.forward));

    }


    private void SetupCamera()
    {
        Orientation orienation = GetOrientation();
        float extraWidthForUi = orienation == Orientation.LANDSCAPE ? movementPanelWidth + (paddingButtons * 2) * 2 : 0;
        float extraHeightForUi = orienation == Orientation.PORTRAIT ? movementPanelHeight + (paddingButtons * 2) : 0;

        float contentHeight = paddingVertical + 2f + height + 4f + paddingVertical + extraHeightForUi;
        float centerVertical = (contentHeight) / 2f - extraHeightForUi - 2;

        float centerHorizontal = (-paddingHorizontal + width + rightSideUiWidth + paddingHorizontal) / 2f - .5f;
        float contentWidth = paddingHorizontal + width + rightSideUiWidth + paddingHorizontal + extraWidthForUi;

        float ratio = GetRatio();
        float viewPortWidth = contentWidth / ratio;

        float viewportSize = Mathf.Max(viewPortWidth, contentHeight) / 2f;

        Camera.main.transform.position = new Vector3(centerHorizontal, centerVertical, -1);
        Camera.main.orthographicSize = viewportSize;

        cameraLeft = centerHorizontal - contentWidth / 2f;
        cameraRight = centerHorizontal + contentWidth / 2f;


        float cameraSize = Camera.main.orthographicSize;
        float www = cameraSize * GetRatio();
        print("actually " + cameraSize);
        print("actually " + www);
        // print("actually " + viewportSize);


    }

    // todo could cache this value and invalidate it when necessary
    private Orientation GetOrientation()
    {
        float ratio = Screen.width / (float)Screen.height;
        return ratio > 1 ? Orientation.LANDSCAPE : Orientation.PORTRAIT;
    }

    private float GetRatio()
    {
        return Screen.width / (float)Screen.height;
    }


    private void PlaceHud()
    {
        PlaceLevelPanel();
        PlaceScorePanel();
        PlaceStatusPanel();
    }

    private void PlaceLevelPanel()
    {
        RectTransform rectTransform = levelPanel;

        int pillPreviewHeight = 5;
        int pillPreviewWidth = 6;

        int panelWidth = pillPreviewWidth;
        int panelHeight = height + 1 - pillPreviewHeight;

        rectTransform.position = new Vector3(width + 5.5f, (panelHeight - 2.5f) / 2, 0);
        rectTransform.sizeDelta = new Vector2(panelWidth, panelHeight);
    }

    private void PlaceScorePanel()
    {
        RectTransform rectTransformScore = scorePanel;

        // TODO figure out how we can calculate this width
        float mainContentWidth = paddingHorizontal + width + rightSideUiWidth + paddingHorizontal;
        // float width = (Mathf.Abs(cameraLeft) + Mathf.Abs(cameraRight)) /2; it ain't this

        float panelWidth = mainContentWidth - 2 * paddingButtons;
        float panelHeight = 4;

        // TODO figure out why I need this -2.5f here
        rectTransformScore.position = new Vector3(mainContentWidth / 2 - 2.5f, height + 4);
        rectTransformScore.sizeDelta = new Vector2(panelWidth, panelHeight);
    }

    private void PlaceStatusPanel()
    {


        RectTransform rectTransformStatus = statusPanel;

        int statusPanelWidth = width - 1;
        rectTransformStatus.position = new Vector3(width / 2 - .5f, height / 2, 0); //- statusPanelWidth / 2
        rectTransformStatus.sizeDelta = new Vector2(statusPanelWidth, 8);
    }

    private void PlaceButtons()
    {
        PlaceMovementButtons();
        PlaceRotationButtons();
    }

    private void PlaceMovementButtons()
    {

        float movementPanelX;
        float movementPanelY;

        RectTransform rectTransformMovement = movementPanel;

        if (GetOrientation() == Orientation.PORTRAIT)
        {
            // Below the board
            movementPanelX = cameraLeft + movementPanelWidth / 2 + paddingButtons;
            movementPanelY = -movementPanelHeight - paddingButtons;
        }
        else
        {
            // To the left and right of it

            float cameraSize = Camera.main.orthographicSize;
            float www = cameraSize * GetRatio();

            movementPanelX = Camera.main.transform.position.x - www + movementPanelWidth / 2;
            movementPanelY = Camera.main.transform.position.y;
        }

        rectTransformMovement.position = new Vector3(movementPanelX, movementPanelY, 0);
        rectTransformMovement.sizeDelta = new Vector2(movementPanelWidth, movementPanelHeight);
    }

    private void PlaceRotationButtons()
    {

        float rotationPanelX;
        float rotationPanelY;

        RectTransform rectTransformRotation = rotationPanel;

        if (GetOrientation() == Orientation.PORTRAIT)
        {
            // Below the board
            rotationPanelX = cameraRight - (rotationPanelWidth / 2 + paddingButtons);
            rotationPanelY = -rotationPanelHeight - paddingButtons;
        }
        else
        {
            // To the left and right of it
            float cameraSize = Camera.main.orthographicSize;
            float www = cameraSize * GetRatio();

            rotationPanelX = Camera.main.transform.position.x + www - movementPanelWidth / 2; ;
            rotationPanelY = Camera.main.transform.position.y;
        }

        rectTransformRotation.position = new Vector3(rotationPanelX, rotationPanelY, 0);
        rectTransformRotation.sizeDelta = new Vector2(rotationPanelWidth, rotationPanelHeight);
    }


}
