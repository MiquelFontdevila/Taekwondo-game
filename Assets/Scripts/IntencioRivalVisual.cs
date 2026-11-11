
using UnityEngine;
using UnityEngine.UI;

public class IntencioRivalVisual : MonoBehaviour
{
    [Header("Icona de la UI")]
    [SerializeField] private Image imatgeIntencio;

    [Header("Sprites")]
    [SerializeField] private Sprite spriteAtac;
    [SerializeField] private Sprite spriteDefensa;

    [Header("Posicionament")]
    [SerializeField] private Camera cameraCombat;

    [SerializeField] private float alturaIcona = 2f;
    [SerializeField] private float desplacamentHoritzontal = 0f;

    [Header("Mida")]
    [SerializeField, Min(1f)]
    private float midaIcona = 80f;

    [Header("Animacio de desaparicio")]
    [SerializeField, Min(0.1f)]
    private float duradaDesaparicio = 2f;

    [SerializeField, Min(0f)]
    private float distanciaPujada = 65f;

    private Lluitador rivalActual;
    private FeedbackLluitador feedbackRival;

    private RectTransform pareIcona;
    private Canvas canvasIcona;

    private bool intencioVisible;
    private float tempsTranscorregut;

    private void Awake()
    {
        if (imatgeIntencio == null)
            return;

        pareIcona =
            imatgeIntencio.transform.parent as RectTransform;

        canvasIcona =
            imatgeIntencio.GetComponentInParent<Canvas>();

        imatgeIntencio.raycastTarget = false;
        imatgeIntencio.gameObject.SetActive(false);
    }

    public void Mostrar(Lluitador rival, string accio)
    {
        if (rival == null || imatgeIntencio == null)
            return;

        Ocultar();

        bool atacara =
            accio == "patadaAlta" ||
            accio == "patadaAlCos" ||
            accio == "empenta";

        bool defensara =
            accio == "bloqueigAlt" ||
            accio == "bloqueigAlCos" ||
            accio == "esquivar";

        if (!atacara && !defensara)
            return;

        rivalActual = rival;

        feedbackRival =
            rivalActual.GetComponent<FeedbackLluitador>();

        imatgeIntencio.sprite =
            atacara ? spriteAtac : spriteDefensa;

        imatgeIntencio.preserveAspect = true;

        imatgeIntencio.rectTransform.sizeDelta =
            new Vector2(midaIcona, midaIcona);

        imatgeIntencio.color = Color.white;
        imatgeIntencio.enabled = true;
        imatgeIntencio.gameObject.SetActive(true);

        if (feedbackRival != null)
            feedbackRival.MostrarIntencio(atacara);

        tempsTranscorregut = 0f;
        intencioVisible = true;

        ActualitzarPosicio(0f);
    }

    private void Update()
    {
        if (!intencioVisible)
            return;

        tempsTranscorregut += Time.deltaTime;

        float progres = Mathf.Clamp01(
            tempsTranscorregut / duradaDesaparicio
        );

        float moviment = Mathf.SmoothStep(
            0f,
            1f,
            progres
        );

        float opacitat = 1f - progres;

        if (imatgeIntencio != null)
        {
            Color color = imatgeIntencio.color;
            color.a = opacitat;
            imatgeIntencio.color = color;
        }

        if (feedbackRival != null)
            feedbackRival.ActualitzarIntencio(opacitat);

        if (progres >= 1f)
        {
            Ocultar();
        }
    }

    private void LateUpdate()
    {
        if (!intencioVisible)
            return;

        float progres = Mathf.Clamp01(
            tempsTranscorregut / duradaDesaparicio
        );

        float moviment = Mathf.SmoothStep(
            0f,
            1f,
            progres
        );

        ActualitzarPosicio(moviment);
    }

    private void ActualitzarPosicio(float moviment)
    {
        if (rivalActual == null ||
            imatgeIntencio == null ||
            pareIcona == null ||
            canvasIcona == null)
            return;

        Camera cam = cameraCombat != null
            ? cameraCombat
            : Camera.main;

        if (cam == null)
            return;

        Vector3 posicioMon =
            rivalActual.transform.position +
            Vector3.up * alturaIcona +
            Vector3.right * desplacamentHoritzontal;

        Vector3 posicioPantalla =
            cam.WorldToScreenPoint(posicioMon);

        if (posicioPantalla.z <= 0f)
        {
            imatgeIntencio.enabled = false;
            return;
        }

        imatgeIntencio.enabled = true;

        Camera camUI =
            canvasIcona.renderMode ==
            RenderMode.ScreenSpaceOverlay
                ? null
                : canvasIcona.worldCamera;

        Vector2 posicioLocal;

        bool correcte =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                pareIcona,
                posicioPantalla,
                camUI,
                out posicioLocal
            );

        if (!correcte)
            return;

        posicioLocal.y += distanciaPujada * moviment;

        imatgeIntencio.rectTransform.anchoredPosition =
            posicioLocal;
    }

    public void Ocultar()
    {
        intencioVisible = false;
        tempsTranscorregut = 0f;

        if (feedbackRival != null)
            feedbackRival.OcultarIntencio();

        if (imatgeIntencio != null)
        {
            imatgeIntencio.gameObject.SetActive(false);
            imatgeIntencio.color = Color.white;
        }

        rivalActual = null;
        feedbackRival = null;
    }

    private void OnDisable()
    {
        Ocultar();
    }
}
