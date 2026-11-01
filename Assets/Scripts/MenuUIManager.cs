using UnityEngine;
using UnityEngine.EventSystems;

public class MenuUIManager : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] private RectTransform text;
    [SerializeField] private float scaleSeleccionat = 1.25f;
    [SerializeField] private float velocitat = 15f;

    private float escalaObjectiu = 1f;

    void OnEnable()
    {
        escalaObjectiu = 1f;
        text.localScale = Vector3.one;
    }

    public void OnSelect(BaseEventData eventData)
    {
        escalaObjectiu = scaleSeleccionat;
    } 

    public void OnDeselect(BaseEventData eventData)
    {
        escalaObjectiu = 1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float actual = text.localScale.x;
        float nova = Mathf.Lerp(actual, escalaObjectiu, velocitat * Time.unscaledDeltaTime);
        text.localScale = new Vector3(nova, nova, 1f);
    }
}
