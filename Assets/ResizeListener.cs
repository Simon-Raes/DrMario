using UnityEngine;

// Attached to an empty UI canvas to detect when the screen size changes. (shrug)
public class ResizeListener : MonoBehaviour
{


    public interface UiResizedListener
    {
        void OnUiResized();
    }

    private UiResizedListener listener;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnRectTransformDimensionsChange()
    {
		if(listener != null)
		{
			listener.OnUiResized();
		}
    }

    public void AddListener(UiResizedListener listener)
    {
        this.listener = listener;
    }
}
