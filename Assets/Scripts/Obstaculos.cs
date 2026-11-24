using UnityEngine;

public enum TipoObstaculo
{
    Rotador,
    Movimiento, 
    ZonaMuerte
}

public class Obstaculos : MonoBehaviour
{
    GameManager gameManager;

    public TipoObstaculo tipoObstaculo;

    public Animator animator;

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        ComprobarObstaculo();
    }

    void ComprobarObstaculo()
    {
        switch (tipoObstaculo)
        {
            case TipoObstaculo.Rotador: // Si el obstáculo es un círculo, rotará

                animator = GetComponentInParent<Animator>();
                animator.SetTrigger("Rota");
                break;

            case TipoObstaculo.Movimiento:
                animator = GetComponent<Animator>();
                animator.SetTrigger("SeMueve");
                break;

            case TipoObstaculo.ZonaMuerte:
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
            if (gameManager.dasheando) return;

            if (tipoObstaculo == TipoObstaculo.Rotador)
            {
                PlayerController jugador = collision.GetComponent<PlayerController>();
                if (gameObject.GetComponent<SpriteRenderer>().color == jugador.frutasDisponibles[jugador.frutaActual].colorFruta) return;
            }
            gameManager.Perder();
        }
    }
}
