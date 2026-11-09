using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MenuNavigator : MonoBehaviour
{

    [SerializeField] private RectTransform[] panells;
    [SerializeField] private RectTransform panellPrincipal;
    [SerializeField] private CanvasGroup grupCanvas;
    [SerializeField] private float durada = 0.35f;
    [SerializeField] private AnimationCurve corba = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform actual;
    private bool movent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (RectTransform p in panells)
        {
            p.anchoredPosition = Vector2.zero;
            p.gameObject.SetActive(p == panellPrincipal);
        }
        actual = panellPrincipal;
        SeleccionarPrimerBoto(actual);
    }

    public void Endavant(GameObject nou)
    {
        Canviar(nou, 1f);
    }

    public void Enrere(GameObject nou)
    {
        Canviar(nou, -1f);
    }

    private void Canviar(GameObject nou, float direccio)
    {
        if (movent)
        {
            return;
        }
        StartCoroutine(Transicio(nou.GetComponent<RectTransform>(), direccio));
    }

    private IEnumerator Transicio(RectTransform nou, float direccio)
    {
        movent = true;
        grupCanvas.interactable = false;
        float amplada = ((RectTransform)transform).rect.width;
        Vector2 entrada = new Vector2(amplada * direccio, 0f);
        Vector2 sortida = new Vector2(-amplada * direccio, 0f);

        nou.gameObject.SetActive(true);
        nou.anchoredPosition = entrada;

        float t = 0f;
        while (t < durada)
        {
            float k = corba.Evaluate(t / durada);
            actual.anchoredPosition = Vector2.LerpUnclamped(Vector2.zero, sortida, k);
            nou.anchoredPosition = Vector2.LerpUnclamped(entrada, Vector2.zero, k);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        actual.anchoredPosition = Vector2.zero;
        actual.gameObject.SetActive(false);
        nou.anchoredPosition = Vector2.zero;
        actual = nou;

        grupCanvas.interactable = true;
        SeleccionarPrimerBoto(actual);
        movent = false;
    }

    private void SeleccionarPrimerBoto(RectTransform panell)
    {
        EventSystem.current.SetSelectedGameObject(null);
        Button boto = panell.GetComponentInChildren<Button>();
        if (boto != null)
        {
            EventSystem.current.SetSelectedGameObject(boto.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToLevelSelection()
    {
        AudioManager.CanviarEscena("LevelSelectionScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}