using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class EnemyController : MonoBehaviour
{
    // Variables
    private GameObject player;
    private Vector3 offset = new (0, -1, 2);
    private float enemySpeed = 1f;
    public bool isAlive = true;
    private float distanceToPlayer;
    private bool readyToShoot = true;

    public GameObject ballPrefab;
    public float ballSpeed;

    private Animator animator;
    private AudioSource enemyAudio;
    private Rigidbody enemyRb;

    public ParticleSystem deathParticles;

    public AudioClip deathSound;
    public AudioClip attackSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Main Camera");
        animator = GetComponentInChildren<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        enemyRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = (player.transform.position - transform.position).magnitude;

        if(isAlive)
        {
            transform.LookAt(player.transform.position);

            if (distanceToPlayer > 10)
            {
                Approach();
                animator.SetBool("isMoving", true);
            }

            else if (distanceToPlayer < 10)
            {
                Attack();
                animator.SetBool("isMoving", false);
            }
        }
    }

    void Approach()
    {
        transform.Translate(enemySpeed * Time.deltaTime * Vector3.forward);  
    }

    void Attack()
    {
        if (readyToShoot)
        {
            //Vector3 shootDirection = (player.transform.position - transform.position).normalized;
            //GameObject instantiatedBall = Instantiate(GenerateBall(), transform.position + offset, transform.rotation);
            //Rigidbody ballRb = instantiatedBall.GetComponent<Rigidbody>();
            //ballRb.AddForce(shootDirection * ballSpeed, ForceMode.Impulse);
            Instantiate(GenerateBall(), transform.position - offset, transform.rotation);
            readyToShoot = false;
            animator.SetTrigger("Attack");
            enemyAudio.PlayOneShot(attackSound);
            StartCoroutine(Reload());
        }
    }

    public void Death()
    {
        isAlive = false;
        enemyRb.velocity = Vector3.zero;
        enemyRb.angularVelocity = Vector3.zero;
        enemyRb.constraints = RigidbodyConstraints.FreezeRotation;
        animator.SetTrigger("Die");
        deathParticles.Play();
        enemyAudio.PlayOneShot(deathSound);
        StartCoroutine(Dying());
    }

    private GameObject GenerateBall()
    {
        GameObject randomBall = ballPrefab;
        randomBall.GetComponent<Renderer>().sharedMaterial.color = GenerateColor();
        return randomBall;
    }

    private Color GenerateColor()
    {
        Color randomColor;
        float ballIndex = UnityEngine.Random.Range(0, 3);

        if (ballIndex == 2)
        {
            randomColor = Color.green;
        }

        else if (ballIndex == 1)
        {
            randomColor = Color.red;
        }

        else
        {
            randomColor = Color.blue;
        }

        return randomColor;
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(5);
        readyToShoot = true;
    }

    IEnumerator Dying()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}