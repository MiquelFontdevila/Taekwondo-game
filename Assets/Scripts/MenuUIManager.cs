using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] private RectTransform text;
    [SerializeField] private float scaleSeleccionat = 1.25f;
    [SerializeField] private float velocitat = 15f;

    [Header("Estat desactivat (disabled)")]
    [SerializeField] private Color colorDesactivat = new Color(0.45f, 0.45f, 0.45f, 1f);

    private float escalaObjectiu = 1f;
    private Button boto;
    private TMP_Text textTMP;
    private Color colorOriginal;
    private bool desactivatAplicat;

    void Awake()
    {
        boto = GetComponent<Button>();
        textTMP = text.GetComponent<TMP_Text>();
        if (textTMP != null)
        {
            colorOriginal = textTMP.color;
        }
    }

    void OnEnable()
    {
        escalaObjectiu = 1f;
        text.localScale = Vector3.one;
        ActualitzarAspecte();
    }

    // Un botó desactivat no s'amplia ni es pot seleccionar amb el ratolí
    private bool Interactuable()
    {
        return boto == null || boto.IsInteractable();
    }

    private void ActualitzarAspecte()
    {
        bool desactivat = !Interactuable();
        if (textTMP != null && desactivat != desactivatAplicat)
        {
            textTMP.color = desactivat ? colorDesactivat : colorOriginal;
            desactivatAplicat = desactivat;
        }
        if (desactivat)
        {
            escalaObjectiu = 1f;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!Interactuable())
        {
            return;
        }
        escalaObjectiu = scaleSeleccionat;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        escalaObjectiu = 1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactuable())
        {
            return;
        }
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    void Update()
    {
        ActualitzarAspecte();

        float actual = text.localScale.x;
        float nova = Mathf.Lerp(actual, escalaObjectiu, velocitat * Time.unscaledDeltaTime);
        text.localScale = new Vector3(nova, nova, 1f);
    }
}