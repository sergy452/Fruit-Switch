using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    PlayerController controllerJugador;
    public int puntos;
    public TMP_Text puntosTMP;
    public TMP_Text perderTMP;
    public TMP_Text mensajeInicialTMP;

    public bool vivo;

    public GameObject jugador;
    public GameObject cambiador;
    public GameObject siguienteObstaculo;
    public GameObject[] listaObstaculos;

    public CinemachineCamera camara;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controllerJugador = jugador.GetComponent<PlayerController>();
        StartCoroutine(EmpezarAJugar());
    }

    public void AlPulsarReiniciar()
    {
        StartCoroutine(EmpezarAJugar());
    }

    public IEnumerator EmpezarAJugar()
    {
        // Se resetean todos los obstáculos
        GameObject[] obstaculosGenerados = FindObjectsByType<GameObject>();
        foreach (GameObject obstaculo in obstaculosGenerados)
        {
            if (obstaculo.name.EndsWith("(Clone)")) Destroy(obstaculo);
        }

        puntos = 0;
        Time.timeScale = 0;
        mensajeInicialTMP.gameObject.SetActive(true);
        perderTMP.gameObject.SetActive(false);
        Vector3 posicionInicialCamara = new Vector3(0f, 0f, -10f);
        camara.ForceCameraPosition(posicionInicialCamara, Quaternion.identity);

        jugador.transform.position = Vector3.zero;
        jugador.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        jugador.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        cambiador.transform.position = new Vector2(0f, 2.5f);

        yield return new WaitUntil(() => Input.GetButtonDown("Jump"));
        controllerJugador.GetComponent<Rigidbody2D>().AddForce(Vector3.up * controllerJugador.fuerzaImpulso, ForceMode2D.Impulse);
        controllerJugador.GetComponent<Rigidbody2D>().AddTorque(controllerJugador.fuerzaGiro, ForceMode2D.Impulse);
        ComenzarPartida();
    }

    public void ComenzarPartida()
    {
        mensajeInicialTMP.gameObject.SetActive(false);
        controllerJugador.CambiarFruta(false);
        Time.timeScale = 1;
        vivo = true;
    }

    public void Perder()
    {
        perderTMP.gameObject.SetActive(true);
        vivo = false;
        Time.timeScale = 0;
    }

    public void GenerarSiguienteObstaculo(int obstaculoLista)
    {
        GameObject nuevoObstaculo = Instantiate(listaObstaculos[obstaculoLista]);
        float posicionX = 0f;

        Debug.Log("Siguiente obstáculo: " + listaObstaculos[obstaculoLista].name);

        // Se genera el obstáculo de la lista que se haya recibido y e cambia la posición del cambiador según el tipo de obstáculo
        switch (obstaculoLista)
        {
            case 0: // Se genera un círculo
                // Se asigna su posición
                nuevoObstaculo.transform.position = new Vector2(posicionX, jugador.transform.position.y + 5f);

                // Se crea una lista con los trozos del círculo
                SpriteRenderer[] trozosCirculo = nuevoObstaculo.GetComponentsInChildren<SpriteRenderer>();

                // Obtenemos el color actual del jugador
                Color colorJugador = controllerJugador.frutasDisponibles[controllerJugador.frutaActual].colorFruta;

                // Le asignamos primero ese color a un trozo
                int trozoElegido = Random.Range(0, trozosCirculo.Length);
                SpriteRenderer trozoMismoColor = trozosCirculo[trozoElegido];
                trozoMismoColor.color = colorJugador;

                // Le asignamos al resto de trozos los colores que queden de la lista
                int indiceFruta = 0;
                for (int i = 0; i <  trozosCirculo.Length; i++)
                {
                    if (i == trozoElegido)
                        continue;

                    if (indiceFruta == controllerJugador.frutaActual)
                        indiceFruta++;

                    trozosCirculo[i].color = controllerJugador.frutasDisponibles[indiceFruta].colorFruta;
                    Debug.Log("Color elegido: " + controllerJugador.frutasDisponibles[indiceFruta].tipoFruta);
                    indiceFruta++;
                }

                // Iniciamos la animación correspondiente al obstáculo
                nuevoObstaculo.GetComponentInChildren<Obstaculos>().animator.GetComponentInChildren<Animator>().SetTrigger("EsCirculo");

                // El cambiador se coloca en el centro del obstáculo en caso de que este sea un círculo
                cambiador.transform.position = nuevoObstaculo.transform.position;
                break;

            case 1: // Se genera un triángulo
                // Se asigna su posición
                posicionX = Random.Range(-2.5f, 2.5f);
                nuevoObstaculo.transform.position = new Vector2(posicionX, jugador.transform.position.y + 5f);

                // Iniciamos la animación correspondiente al obstáculo
                nuevoObstaculo.GetComponentInChildren<Obstaculos>().animator.SetTrigger("EsTriangulo");

                // El cambiador se coloca un poco debajo del obstáculo en caso de que este sea un triángulo moviéndose
                cambiador.transform.position = new Vector2(0f, nuevoObstaculo.transform.position.y + 2.5f);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        puntosTMP.text = "Puntos: " + puntos;
    }
}
