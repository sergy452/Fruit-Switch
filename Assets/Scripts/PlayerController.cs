using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Pieza
{
    public Color colorFruta;
    public TipoFruta tipoFruta;
}
public enum TipoFruta
{
    Manzana,
    Limon,
    Sandia,
    Kiwi,
    Uva
}

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    Rigidbody2D rb;
    public GameObject cambiador;

    public float fuerzaImpulso;
    public float fuerzaGiro;

    public Pieza[] frutasDisponibles;
    public int frutaActual = -1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Salto()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
            rb.angularVelocity = 0f;
            rb.AddForce(Vector3.up * fuerzaImpulso, ForceMode2D.Impulse);
            rb.AddTorque(fuerzaGiro, ForceMode2D.Impulse);
        }
    }

    public void CambiarFruta(bool cambiadorRecogido = true)
    {
        int frutaCambiada = Random.Range(0, frutasDisponibles.Length);
        while (frutaActual == frutaCambiada) frutaCambiada = Random.Range(0, frutasDisponibles.Length);

        frutaActual = frutaCambiada;

        gameObject.GetComponent<SpriteRenderer>().color = frutasDisponibles[frutaActual].colorFruta;

        if (cambiadorRecogido) // Si se ha comenzado la partida, el cambiador no se desplaza ni se suman puntos
        {
            gameManager.puntos++;
            gameManager.GenerarSiguienteObstaculo(Random.Range(0, gameManager.listaObstaculos.Length));
            //gameManager.GenerarSiguienteObstaculo(0);
        }

        //Debug.Log("Fruta cambiada a la número: " + frutaCambiada + " de la lista");
        //Debug.Log("La fruta es: " + frutasDisponibles[frutaActual].tipoFruta);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.vivo)
        {
            Salto();
            rb.rotation += -0.075f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cambiador"))
        {
            CambiarFruta();
        }
        if (collision.CompareTag("Meta"))
        {
            gameManager.Ganar();
        }
    }
}
