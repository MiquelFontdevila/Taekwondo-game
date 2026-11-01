using UnityEngine;
using UnityEngine.InputSystem;

public class PruevaMovimientoJugador : MonoBehaviour
{
    public InputActionReference accionMover;
    public float velocidad = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direccion = accionMover.action.ReadValue<Vector2>();
        direccion= Vector2.ClampMagnitude(direccion, 1f);
        Vector3 movimiento = new Vector3(direccion.x, direccion.y, 0f) * velocidad * Time.deltaTime;
        transform.position += movimiento;
    }

    public void OnEnable()
    {
        accionMover.action.Enable();
    }
    public void OnDisable()
    {
        accionMover.action.Disable();
    }
}
