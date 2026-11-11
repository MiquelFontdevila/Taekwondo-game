using System.Collections;
using UnityEngine;

public class FeedbackLluitador : MonoBehaviour
{
    [Header("Prefabs de particules")]
    [SerializeField]
    private ParticleSystem fxImpacte;

    [SerializeField]
    private ParticleSystem fxBloqueig;

    [SerializeField]
    private ParticleSystem fxEsquiva;

    [Header("Punts de contacte")]
    [SerializeField]
    private Transform puntImpacteAlt;

    [SerializeField]
    private Transform puntImpacteCos;

    [Header("Punts de bloqueig")]
    [SerializeField]
    private Transform puntBloqueigAlt;

    [SerializeField]
    private Transform puntBloqueigCos;

    [Header("Punt d'esquiva")]
    [SerializeField]
    private Transform puntEsquiva;

    [Header("Feedback de dany")]
    [SerializeField]
    private Color colorFlash = new Color(1f, 0.55f, 0.35f, 1f);

    [SerializeField]
    private float duradaFlash = 0.10f;

    private SpriteRenderer[] sprites;
    private Color[] colorsOriginals;
    private Coroutine flashActual;

    private void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>(true);

        colorsOriginals = new Color[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            colorsOriginals[i] = sprites[i].color;
        }
    }

    public void MostrarImpacte(bool bloquejat, bool atacAlt)
    {
        Transform punt;

        if (bloquejat)
        {
            punt = atacAlt ? puntBloqueigAlt : puntBloqueigCos;
        }
        else
        {
            punt = atacAlt ? puntImpacteAlt : puntImpacteCos;
        }

        Vector3 posicio = punt != null ? punt.position : transform.position;

        ParticleSystem prefab = bloquejat ? fxBloqueig : fxImpacte;

        if (prefab != null)
        {
            ParticleSystem efecte = Instantiate(prefab, posicio, Quaternion.identity);

            efecte.Play();

            float durada = efecte.main.duration + efecte.main.startLifetime.constantMax;

            Destroy(efecte.gameObject, durada + 0.1f);
        }

        if (!bloquejat)
        {
            if (flashActual != null)
            {
                StopCoroutine(flashActual);
                RestaurarColors();
            }

            flashActual = StartCoroutine(Parpellejar());
        }
    }

    public void MostrarEsquiva(float durada = 0.4f)
    {
        if (fxEsquiva == null || puntEsquiva == null)
            return;

        StartCoroutine(GenerarEstela(durada));
    }

    private IEnumerator GenerarEstela(float durada)
    {
        ParticleSystem efecte = Instantiate(fxEsquiva, puntEsquiva.position, Quaternion.identity);

        efecte.transform.SetParent(puntEsquiva, true);

        efecte.transform.localPosition = Vector3.zero;
        efecte.transform.localRotation = Quaternion.identity;
        efecte.transform.localScale = Vector3.one;

        var main = efecte.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true;

        efecte.Clear();
        efecte.Play();

        yield return new WaitForSeconds(durada);

        efecte.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        yield return new WaitForSeconds(main.startLifetime.constantMax + 0.1f);

        if (efecte != null)
            Destroy(efecte.gameObject);
    }

    private IEnumerator Parpellejar()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
            {
                sprites[i].color = colorsOriginals[i] * colorFlash;
            }
        }

        yield return new WaitForSeconds(duradaFlash);

        RestaurarColors();
        flashActual = null;
    }

    private void RestaurarColors()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
            {
                sprites[i].color = colorsOriginals[i];
            }
        }
    }

    private void OnDisable()
    {
        if (flashActual != null)
        {
            StopCoroutine(flashActual);
            flashActual = null;
        }

        if (colorsOriginals != null)
            RestaurarColors();
    }
}
