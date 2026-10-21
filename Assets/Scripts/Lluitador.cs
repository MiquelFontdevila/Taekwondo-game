using UnityEngine;

public class Lluitador : MonoBehaviour
{
    public int puntuacio;
    public int energia;
    public int posicio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void AugmentarPunts(int punts)
    {
        puntuacio =+ punts;
    }
    public void AugmentarPosicio(int numPosicio)
    {
        posicio =+ numPosicio;
    }
    public void DisminuirPosicio(int numPosicio)
    {
        posicio =+ numPosicio;
    }
}
