using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Custom button event that sends out an OnPressed event as long as the button is held down.
public class PressedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler//, IPointerEnterHandler, IPointerExitHandler
{
    private bool pressed;

	public UnityEvent onPressed;
	public UnityEvent onReleased;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pressed = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
		onReleased.Invoke();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            onPressed.Invoke();
        }
    }
}
