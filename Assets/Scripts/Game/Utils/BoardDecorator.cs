using UnityEngine;
using UnityEngine.UI;

// Creates the border around the brid and adjusts the camera position to make sure everything fits on screen.
// Also position UI here
public class BoardDecorator
{
    private int width;
    private int height;
    private int boardCenter;
    private GameObject pipe;
    private GameObject pipeCorner;

    private float paddingVertical = 1f;
    private float rightSideUiWidth = 8f;
    private float paddingHorizontal = 2f;

    public BoardDecorator(int width, int height, GameObject pipe, GameObject pipeCorner)
    {
        this.width = width;
        this.height = height;
        boardCenter = width / 2;

        this.pipe = pipe;
        this.pipeCorner = pipeCorner;
    }

    public void Setup()
    {
        SetupPipeBorders();

        SetupCamera();

        SetupHud();
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

        GameObject.Instantiate(pipeCorner, new Vector3(boardCenter - 2, height + 2, 0), Quaternion.AngleAxis(180, Vector3.forward));
        GameObject.Instantiate(pipeCorner, new Vector3(boardCenter + 1, height + 2, 0), Quaternion.AngleAxis(270, Vector3.forward));

        GameObject.Instantiate(pipeCorner, new Vector3(boardCenter - 3, height + 2, 0), Quaternion.identity);
        GameObject.Instantiate(pipeCorner, new Vector3(boardCenter + 2, height + 2, 0), Quaternion.AngleAxis(90, Vector3.forward));

        GameObject.Instantiate(pipe, new Vector3(boardCenter - 3, height + 3, 0), Quaternion.identity);
        GameObject.Instantiate(pipe, new Vector3(boardCenter + 2, height + 3, 0), Quaternion.identity);

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
        float centerVertical = (-paddingVertical - 1f + height + 4f + paddingVertical) / 2f - .5f;
        float viewPortHeight = paddingVertical + 1f + height + 4f + paddingVertical;

        float centerHorizontal = (-paddingHorizontal + width + rightSideUiWidth + paddingHorizontal) / 2f - .5f;
        float viewportWidth = paddingHorizontal + width + rightSideUiWidth + paddingHorizontal;

        float ratio = Screen.width / (float)Screen.height;
        viewportWidth = viewportWidth / ratio;

        float viewportSize = Mathf.Max(viewportWidth, viewPortHeight) / 2f;

        Camera.main.transform.position = new Vector3(centerHorizontal, centerVertical, -1);
        Camera.main.orthographicSize = viewportSize;
    }

    private void SetupHud()
    {
        // todo move score to the top (next to the top opening)
        // make the rest fit below the preview pill
        // both should be world space UIs so they can be measured?
        Vector3 ffse = Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0));

        float ewe = ffse.x;
    }

    private void PlaceButtons()
    {
        
    }
}
