
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class PaginaTutorial
{
    [TextArea(3, 10)]
    public string descripcio;

    public string titol;
    public Sprite captura;
}

public class TutorialManager : MonoBehaviour
{
    [Header("Pagines del tutorial")]
    [SerializeField] private PaginaTutorial[] pagines;

    [Header("Interficie")]
    [SerializeField] private TextMeshProUGUI textTitol;
    [SerializeField] private TextMeshProUGUI textDescripcio;
    [SerializeField] private Image imatgeCaptura;
    [SerializeField] private TextMeshProUGUI textBotoSeguent;

    [Header("Botons")]
    [SerializeField] private Button botoAnterior;
    [SerializeField] private Button botoSeguent;

    [Header("Transicio")]
    [SerializeField] private GameObject chooseCharacterObject;

    private int paginaActual = 0;

    private void Start()
    {
        if (chooseCharacterObject != null)
            chooseCharacterObject.SetActive(false);

        paginaActual = 0;
        ActualitzarPagina();
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
            Debug.LogWarning(
                "No hi ha pagines configurades al tutorial."
            );
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

        if (botoAnterior != null)
        {
            botoAnterior.interactable = paginaActual > 0;
        }

        if (textBotoSeguent != null)
        {
            bool ultimaPagina =
                paginaActual == pagines.Length - 1;

            textBotoSeguent.text = ultimaPagina
                ? "A lluitar!"
                : "Continuar";
        }

        if (EventSystem.current != null)
        {
            GameObject seleccionat =
                botoAnterior != null &&
                botoAnterior.interactable
                    ? botoAnterior.gameObject
                    : botoSeguent != null
                        ? botoSeguent.gameObject
                        : null;

            EventSystem.current.SetSelectedGameObject(
                seleccionat
            );
        }
    }

    public void FinalitzarTutorial()
    {
        if (chooseCharacterObject == null)
        {
            Debug.LogError(
                "Falta assignar ChooseCharacterObject."
            );
            return;
        }

        chooseCharacterObject.SetActive(true);

        gameObject.SetActive(false);
    }
}
