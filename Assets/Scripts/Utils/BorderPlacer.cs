using UnityEngine;

public class BorderPlacer
{
    private int width;
    private int height;
	private GameObject pipe;
	private GameObject pipeCorner;

    public BorderPlacer(int width, int height, GameObject pipe, GameObject pipeCorner)
    {
		this.width = width;
		this.height = height;

		this.pipe = pipe;
		this.pipeCorner = pipeCorner;
    }

	public void CreateBorders()
	{
		 // Horizontal pipes
        for (int i = 0; i < width; i++)
        {
            GameObject.Instantiate(pipe, new Vector3(i, -1, 0), Quaternion.AngleAxis(90, Vector3.forward));

            if (i < 2 || i > 5)
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
        GameObject.Instantiate(pipeCorner, new Vector3(2, height, 0), Quaternion.AngleAxis(90, Vector3.forward));
        GameObject.Instantiate(pipeCorner, new Vector3(5, height, 0), Quaternion.identity);

        GameObject.Instantiate(pipe, new Vector3(2, height + 1, 0), Quaternion.identity);
        GameObject.Instantiate(pipe, new Vector3(5, height + 1, 0), Quaternion.identity);

        GameObject.Instantiate(pipeCorner, new Vector3(2, height + 2, 0), Quaternion.AngleAxis(180, Vector3.forward));
        GameObject.Instantiate(pipeCorner, new Vector3(5, height + 2, 0), Quaternion.AngleAxis(270, Vector3.forward));

        GameObject.Instantiate(pipeCorner, new Vector3(1, height + 2, 0), Quaternion.identity);
        GameObject.Instantiate(pipeCorner, new Vector3(6, height + 2, 0), Quaternion.AngleAxis(90, Vector3.forward));

        GameObject.Instantiate(pipe, new Vector3(1, height + 3, 0), Quaternion.identity);
        GameObject.Instantiate(pipe, new Vector3(6, height + 3, 0), Quaternion.identity);

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
}
