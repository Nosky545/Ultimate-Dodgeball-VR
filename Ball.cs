using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Ball : XRGrabInteractable
{
    protected GameManager gameManager;
    protected bool isHold = false;
    protected bool launched = false;
    protected bool isBad = true;
    protected bool isMissile = false;
    protected Rigidbody ballRb;
    protected Color ballColor;
    protected Transform camTransform;
    protected AudioSource audioSource;

    public ParticleSystem boomParticules;
    public ParticleSystem explosionParticules;
    public float ballSpeed = 20;
    public float missileSpeed = 10;
    public float badSpeed = 5;

    public AudioClip explosionSound;
    public AudioClip fuzeSound;
    public AudioClip missileSound;
    public AudioClip protectionSound;

    public GameObject selector;

    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        ballColor = GetComponent<Renderer>().material.color;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        ballRb.useGravity = false;
    }

    void Update()
    {
        camTransform = GameObject.Find("Main Camera").transform;

        if (isBad)
        {
            transform.LookAt(camTransform.position);
            transform.Translate(Vector3.forward * badSpeed * Time.deltaTime);
        }

        if (isMissile)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Enemy");
            ballRb.useGravity = false;
            transform.LookAt(target.transform.position);
            transform.Translate(Vector3.forward * missileSpeed * Time.deltaTime);
        }
    }

    protected void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && launched)
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            enemyController.Death();
            boomParticules.Play();

            if (ballColor == Color.green)
            {
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Ball") && isHold && ballColor == Color.blue)
        {
            ballColor = collision.gameObject.GetComponent<Renderer>().material.color;
            GetComponent<Renderer>().material.color = ballColor;
            audioSource.PlayOneShot(protectionSound);
            Debug.Log("Blue Protection !");
        }

        if (collision.gameObject.CompareTag("Ball") && isBad)
        {
            isBad = false;
            ballRb.useGravity = true;
        }

        if (collision.gameObject.CompareTag("Ground") && ballColor != Color.red)
        {
            launched = false;
        }
    }

    protected void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isHold && !launched)
        {
            gameManager.lifeCount--;
            isBad = false;
            ballRb.useGravity = true;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        isHold = true;
        isBad = false;
        ballRb.useGravity = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isHold = false;
        launched = true;
        ballRb.AddForce(camTransform.forward * ballSpeed, ForceMode.Impulse);
        transform.rotation = new Quaternion(0, 0, 0, 0);
        //ballRb.MoveRotation(Quaternion.LookRotation(Vector3.up, Vector3.up));
        ballRb.constraints = RigidbodyConstraints.FreezeRotation;

        if (ballColor != Color.green)
        {
            ballRb.useGravity = true;

            if (ballColor != Color.red)
            {
                StartCoroutine(Explode());
            }
        }

        Power();
        Debug.Log("Ball's out !");
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        selector.SetActive(true);

    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        selector.SetActive(false);
    }

    protected virtual void Power()
    {
        if (ballColor == Color.red)
        {
            // Explosion
            StartCoroutine(Fuze());
            GetComponent<Renderer>().material.color = Color.yellow;
            audioSource.PlayOneShot(fuzeSound);
            Debug.Log("Red Explosion !");
        }

        if (ballColor == Color.green)
        {
            // Missile
            isMissile = true;
            audioSource.PlayOneShot(missileSound);
            Debug.Log("Green Missile !");
        }
    }

    IEnumerator Fuze()
    {
        yield return new WaitForSeconds(2);
        audioSource.PlayOneShot(explosionSound);
        transform.localScale *= 8;
        explosionParticules.Play();
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
