using UnityEngine;

public enum TipoObstaculo
{
    Circulo,
    Triangulo,
    Cuadrado,
    Rombo,
    ZonaMuerte
}

public class Obstaculos : MonoBehaviour
{
    GameManager gameManager;

    public TipoObstaculo formaObstaculo;

    public Animator animator;

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        ComprobarObstaculo();
    }

    void ComprobarObstaculo()
    {
        switch (formaObstaculo)
        {
            case TipoObstaculo.Circulo: // Si el obstáculo es un círculo, rotará

                animator = GetComponentInParent<Animator>();
                animator.SetTrigger("EsCirculo");
                break;

            case TipoObstaculo.Triangulo:
                animator = GetComponent<Animator>();
                animator.SetTrigger("EsTriangulo");
                break;

            case TipoObstaculo.Cuadrado:

                break;

            case TipoObstaculo.Rombo:

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (formaObstaculo == TipoObstaculo.Circulo)
            {
                if (gameObject.GetComponent<SpriteRenderer>().color == collision.GetComponent<SpriteRenderer>().color) return;
            }
            gameManager.Perder();
        }
    }
}
