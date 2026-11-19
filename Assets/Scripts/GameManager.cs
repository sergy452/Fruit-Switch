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
        //controllerJugador.GetComponent<Rigidbody2D>().AddForce(Vector3.up * controllerJugador.fuerzaImpulso / 2f , ForceMode2D.Impulse);
        ComenzarPartida();
    }

    public void ComenzarPartida()
    {
        mensajeInicialTMP.gameObject.SetActive(false);
        controllerJugador.CambiarFruta();
        Time.timeScale = 1;
        vivo = true;
    }

    public void Perder()
    {
        perderTMP.gameObject.SetActive(true);
        vivo = false;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        puntosTMP.text = "Puntos: " + puntos;
    }
}
