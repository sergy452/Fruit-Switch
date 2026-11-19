using Unity.VisualScripting;
using UnityEngine;

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

    public TipoFruta[] frutasDisponibles;
    int frutaActual = -1;


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

    public void CambiarFruta()
    {
        int frutaCambiada = Random.Range(0, frutasDisponibles.Length);
        while (frutaActual == frutaCambiada) frutaCambiada = Random.Range(0, frutasDisponibles.Length);

        frutaActual = frutaCambiada;

        switch (frutaCambiada)
        {
            case 0: // Manzana
                gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                break;

            case 1: // Limon
                gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;

            case 2: // Sandia
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                break;

            case 3: // Kiwi
                gameObject.GetComponent<SpriteRenderer>().color = Color.brown;
                break;

            case 4: // Uva
                gameObject.GetComponent<SpriteRenderer>().color = Color.purple;
                break;
        }

        gameManager.puntos++;
        cambiador.transform.position = new Vector2(cambiador.transform.position.x, cambiador.transform.position.y + 3f);
        Debug.Log("Fruta cambiada a la número: " + frutaCambiada + " de la lista");
        Debug.Log("La fruta es: " + frutasDisponibles[frutaCambiada]);

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.vivo)
        {
            Salto();
            rb.rotation += -0.2f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cambiador"))
        {
            CambiarFruta();
        }
    }
}
