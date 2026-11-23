using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    PlayerController controllerJugador;
    public CinemachineCamera camara;

    public TMP_Text puntosTMP;
    public TMP_Text perderTMP;
    public TMP_Text mensajeInicialTMP;

    public int puntos;
    public bool vivo;

    public GameObject jugador;
    public GameObject cambiador;
    public GameObject siguienteObstaculo;
    public GameObject[] listaObstaculos;
    public GameObject meta;

    [Space(5)]
    [Header("Victoria")]
    public Image panelVictoria;
    public TMP_Text mensajePuntuacionTMP;
    public TMP_Text puntuacionRecordTMP;
    public int puntuacionRecord;

    [Space(5)]
    [Header("Tiempo")]
    public TMP_Text tiempoTMP;
    public Image circuloTiempo;
    float segundos;
    public float tiempoMaximo;

    void Start()
    {
        puntuacionRecord = 0;
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

        // Se resetea el tiempo y los puntos
        tiempoTMP.gameObject.SetActive(true);
        segundos = tiempoMaximo;
        puntos = 0;
        Time.timeScale = 0;

        // Se resetean los elementos de victoria o derrota
        mensajeInicialTMP.gameObject.SetActive(true);
        perderTMP.gameObject.SetActive(false);
        meta.SetActive(false);
        panelVictoria.gameObject.SetActive(false);
        puntosTMP.gameObject.SetActive(true);

        Vector3 posicionInicialCamara = new Vector3(0f, 0f, -10f);
        camara.ForceCameraPosition(posicionInicialCamara, Quaternion.identity);
        camara.GetComponent<CinemachineCamera>().Target.TrackingTarget = jugador.transform;

        jugador.transform.position = Vector3.zero;
        jugador.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        jugador.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        // El cambiador vuelve a su posición inicial
        cambiador.gameObject.SetActive(true);
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

    public void Ganar()
    {
        vivo = false;
        Time.timeScale = 0;
        panelVictoria.gameObject.SetActive(true);
        puntosTMP.gameObject.SetActive(false);
        tiempoTMP.gameObject.SetActive(false);
        mensajePuntuacionTMP.text = "Puntuación final: " + puntos;
        GameObject nuevoRecordTMP = puntuacionRecordTMP.transform.GetChild(0).gameObject;

        if (puntos > puntuacionRecord)
        {
            puntuacionRecordTMP.text = "Puntuación más alta: " + puntos;
            puntuacionRecord = puntos;
            nuevoRecordTMP.gameObject.SetActive(true);
        }
        else
            nuevoRecordTMP.gameObject.SetActive(false);
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

        // Si el tiempo se ha acabado, se generará la meta y acabaremos el nivel
        if (segundos <= 0f)
        {
            meta.SetActive(true);
            meta.transform.position = new Vector2(posicionX, jugador.transform.position.y + 8f);
            cambiador.gameObject.SetActive(false);
            return;
        }

        // Se genera el obstáculo de la lista que se haya recibido y se cambia la posición del cambiador según el tipo de obstáculo
        switch (obstaculoLista)
        {
            case 0: // Se genera un círculo
                // Se asigna su posición
                nuevoObstaculo.transform.position = new Vector2(posicionX, jugador.transform.position.y + 5f);
                bool giraDerecha = Random.Range(0, 2) == 0;
                if (giraDerecha) nuevoObstaculo.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                else nuevoObstaculo.transform.rotation = Quaternion.identity;

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
                    //Debug.Log("Color elegido: " + controllerJugador.frutasDisponibles[indiceFruta].tipoFruta);
                    indiceFruta++;
                }

                // Iniciamos la animación correspondiente al obstáculo
                nuevoObstaculo.GetComponentInChildren<Obstaculos>().animator.GetComponentInChildren<Animator>().SetTrigger("EsCirculo");

                // El cambiador se coloca en el centro del obstáculo en caso de que este sea un círculo
                bool apareceDentro = Random.Range(0, 2) == 0;
                if (apareceDentro) cambiador.transform.position = nuevoObstaculo.transform.position;
                else cambiador.transform.position = new Vector2(0f, nuevoObstaculo.transform.position.y + 2.5f);

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

        tiempoTMP.text = Mathf.CeilToInt(segundos) + "s";
        circuloTiempo.fillAmount = segundos / tiempoMaximo;
        if (segundos > 0) segundos -= Time.deltaTime;

        // Si hemos llegado a la meta, la cámara deja de seguirnos
        if (meta.gameObject.activeSelf && Vector2.Distance(jugador.transform.position, meta.transform.position) < 3f) camara.GetComponent<CinemachineCamera>().Target.TrackingTarget = null;
    }
}
