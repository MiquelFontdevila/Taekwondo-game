using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class PaginaTutorial
{
    public string titol;

    [TextArea(3, 10)]
    public string descripcio;

    public Sprite captura;

    [Header("Mida de la captura")]
    [Range(100f, 340f)]
    public float alturaImatge = 340f;

    public bool prioritzarMida = false;
}

public class TutorialManager : MonoBehaviour
{
    [Header("Pagines del tutorial")]
    [SerializeField]
    private PaginaTutorial[] pagines;

    [Header("Interficie")]
    [SerializeField]
    private TextMeshProUGUI textTitol;

    [SerializeField]
    private TextMeshProUGUI textDescripcio;

    [SerializeField]
    private Image imatgeCaptura;

    [SerializeField]
    private TextMeshProUGUI textBotoSeguent;

    [Header("Botons")]
    [SerializeField]
    private Button botoAnterior;

    [SerializeField]
    private Button botoSeguent;

    [Header("Transicio")]
    [SerializeField]
    private GameObject chooseCharacterObject;

    [Header("Distribucio adaptable")]
    [SerializeField]
    private RectTransform tutorialContent;

    [SerializeField]
    private LayoutElement layoutImatge;

    [SerializeField, Range(0.2f, 0.7f)]
    private float ampladaMaximaImatge = 0.53f;

    private float ultimaAmplada = -1f;

    private int paginaActual = 0;

    private void Start()
    {
        if (chooseCharacterObject != null)
            chooseCharacterObject.SetActive(false);

        paginaActual = 0;
        ActualitzarPagina();
    }

    private void AjustarDistribucio()
    {
        if (
            tutorialContent == null
            || layoutImatge == null
            || imatgeCaptura == null
            || pagines == null
            || pagines.Length == 0
        )
            return;

        Sprite captura = imatgeCaptura.sprite;

        if (captura == null)
            return;

        float ampladaDisponible = tutorialContent.rect.width;

        float alturaDisponible = tutorialContent.rect.height;

        if (ampladaDisponible <= 0f || alturaDisponible <= 0f)
            return;

        PaginaTutorial pagina = pagines[paginaActual];

        float proporcio = captura.rect.width / captura.rect.height;

        float alturaDesitjada = Mathf.Min(pagina.alturaImatge, alturaDisponible);

        float ampladaNatural = alturaDesitjada * proporcio;

        float ampladaMaxima = ampladaDisponible * ampladaMaximaImatge;

        float ampladaReservadaText = 450f;
        float separacio = 25f;

        ampladaMaxima = Mathf.Min(
            ampladaMaxima,
            Mathf.Max(0f, ampladaDisponible - ampladaReservadaText - separacio)
        );

        float ampladaFinal = Mathf.Min(ampladaNatural, ampladaMaxima);

        float alturaFinal = ampladaFinal / proporcio;

        layoutImatge.preferredWidth = ampladaFinal;
        layoutImatge.preferredHeight = alturaFinal;

        layoutImatge.flexibleWidth = 0f;
        layoutImatge.flexibleHeight = 0f;

        if (pagina.prioritzarMida)
        {
            layoutImatge.minWidth = ampladaFinal;
            layoutImatge.minHeight = alturaFinal;
        }
        else
        {
            layoutImatge.minWidth = -1f;
            layoutImatge.minHeight = -1f;
        }

        LayoutRebuilder.MarkLayoutForRebuild(tutorialContent);
    }

    public void Seguent()
    {
        if (pagines == null || pagines.Length == 0)
            return;

        if (paginaActual < pagines.Length - 1)
        {
            paginaActual++;
            ActualitzarPagina();
        }
        else
        {
            FinalitzarTutorial();
        }
    }

    public void Anterior()
    {
        if (paginaActual <= 0)
            return;

        paginaActual--;
        ActualitzarPagina();
    }

    private void ActualitzarPagina()
    {
        if (pagines == null || pagines.Length == 0)
        {
            Debug.LogWarning("No hi ha pagines configurades al tutorial.");
            return;
        }

        PaginaTutorial pagina = pagines[paginaActual];

        if (textTitol != null)
            textTitol.text = pagina.titol;

        if (textDescripcio != null)
            textDescripcio.text = pagina.descripcio;

        if (imatgeCaptura != null)
        {
            imatgeCaptura.sprite = pagina.captura;
            imatgeCaptura.preserveAspect = true;
            imatgeCaptura.enabled = pagina.captura != null;
        }

        Canvas.ForceUpdateCanvases();
        AjustarDistribucio();

        if (botoAnterior != null)
        {
            botoAnterior.interactable = paginaActual > 0;
        }

        if (textBotoSeguent != null)
        {
            bool ultimaPagina = paginaActual == pagines.Length - 1;

            textBotoSeguent.text = ultimaPagina ? "A lluitar!" : "Continuar";
        }

        if (EventSystem.current != null)
        {
            GameObject seleccionat =
                botoAnterior != null && botoAnterior.interactable ? botoAnterior.gameObject
                : botoSeguent != null ? botoSeguent.gameObject
                : null;

            EventSystem.current.SetSelectedGameObject(seleccionat);
        }
    }

    private void LateUpdate()
    {
        if (tutorialContent == null)
            return;

        float ampladaActual = tutorialContent.rect.width;

        if (Mathf.Abs(ampladaActual - ultimaAmplada) > 0.5f)
        {
            ultimaAmplada = ampladaActual;
            AjustarDistribucio();
        }
    }

    public void FinalitzarTutorial()
    {
        if (chooseCharacterObject == null)
        {
            Debug.LogError("Falta assignar ChooseCharacterObject.");
            return;
        }

        chooseCharacterObject.SetActive(true);

        gameObject.SetActive(false);
    }
}
