using UnityEngine;

public class IntencioRivalVisual : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField]
    private Sprite spriteAtac;

    [SerializeField]
    private Sprite spriteDefensa;

    [Header("Posicionament al mon")]
    [SerializeField]
    private float alturaIcona = 0.8f;

    [SerializeField]
    private float desplacamentHoritzontal = 0f;

    [Header("Mida")]
    [SerializeField, Min(0.1f)]
    private float alturaSprite = 1f;

    [Header("Animacio")]
    [SerializeField, Min(0.1f)]
    private float duradaDesaparicio = 2f;

    [SerializeField, Min(0f)]
    private float distanciaPujada = 0.7f;

    [Header("Ordre visual")]
    [SerializeField]
    private int ordreAddicional = 100;

    private Lluitador rivalActual;
    private FeedbackLluitador feedbackRival;

    private Transform puntReferencia;
    private GameObject objecteIcona;
    private SpriteRenderer rendererIcona;

    private float tempsTranscorregut;
    private bool intencioVisible;

    public void Mostrar(Lluitador rival, string accio)
    {
        Ocultar();

        if (rival == null)
            return;

        bool atacara = accio == "patadaAlta" || accio == "patadaAlCos" || accio == "empenta";

        bool defensara = accio == "bloqueigAlt" || accio == "bloqueigAlCos" || accio == "esquivar";

        if (!atacara && !defensara)
            return;

        Sprite sprite = atacara ? spriteAtac : spriteDefensa;

        if (sprite == null)
        {
            Debug.LogWarning("Falta assignar el sprite de la intencio.");
            return;
        }

        rivalActual = rival;

        feedbackRival = rival.GetComponent<FeedbackLluitador>();

        puntReferencia = feedbackRival != null ? feedbackRival.PuntIntencio : rival.transform;

        objecteIcona = new GameObject("IconaIntencioRival");

        rendererIcona = objecteIcona.AddComponent<SpriteRenderer>();

        rendererIcona.sprite = sprite;
        rendererIcona.color = Color.white;

        SpriteRenderer spriteRival = rival.GetComponentInChildren<SpriteRenderer>(true);

        if (spriteRival != null)
        {
            rendererIcona.sortingLayerID = spriteRival.sortingLayerID;

            rendererIcona.sortingOrder = spriteRival.sortingOrder + ordreAddicional;
        }
        else
        {
            rendererIcona.sortingOrder = ordreAddicional;
        }

        float alturaOriginal = sprite.bounds.size.y;

        if (alturaOriginal > 0f)
        {
            float escala = alturaSprite / alturaOriginal;

            objecteIcona.transform.localScale = Vector3.one * escala;
        }

        if (feedbackRival != null)
            feedbackRival.MostrarIntencio(atacara);

        tempsTranscorregut = 0f;
        intencioVisible = true;

        ActualitzarPosicio(0f);

        Debug.Log(
            "Intencio rival: "
                + rival.name
                + " | Punt: "
                + puntReferencia.name
                + " | Posicio: "
                + puntReferencia.position
        );
    }

    private void Update()
    {
        if (!intencioVisible)
            return;

        tempsTranscorregut += Time.deltaTime;

        float progres = Mathf.Clamp01(tempsTranscorregut / duradaDesaparicio);

        float opacitat = 1f - progres;

        if (rendererIcona != null)
        {
            Color color = rendererIcona.color;
            color.a = opacitat;
            rendererIcona.color = color;
        }

        if (feedbackRival != null)
            feedbackRival.ActualitzarIntencio(opacitat);

        if (progres >= 1f)
            Ocultar();
    }

    private void LateUpdate()
    {
        if (!intencioVisible)
            return;

        float progres = Mathf.Clamp01(tempsTranscorregut / duradaDesaparicio);

        float moviment = Mathf.SmoothStep(0f, 1f, progres);

        ActualitzarPosicio(moviment);
    }

    private void ActualitzarPosicio(float moviment)
    {
        if (puntReferencia == null || objecteIcona == null)
            return;

        Vector3 posicio = puntReferencia.position;

        posicio.x += desplacamentHoritzontal;

        posicio.y += alturaIcona;
        posicio.y += distanciaPujada * moviment;

        posicio.z = puntReferencia.position.z;

        objecteIcona.transform.position = posicio;
    }

    public void Ocultar()
    {
        intencioVisible = false;
        tempsTranscorregut = 0f;

        if (feedbackRival != null)
            feedbackRival.OcultarIntencio();

        if (objecteIcona != null)
            Destroy(objecteIcona);

        objecteIcona = null;
        rendererIcona = null;

        rivalActual = null;
        feedbackRival = null;
        puntReferencia = null;
    }

    private void OnDisable()
    {
        Ocultar();
    }
}
