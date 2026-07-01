using System.Collections.Generic;
using UnityEngine;

public class MovingObjectSpawner : MonoBehaviour {
    private float speed;
    private float direction;



    private bool isRotated = false;
    private float length;
    private float respawnDistance;

    GameObject currentSpawn;


    [SerializeField]private float minSpeed;
    [SerializeField]private float maxSpeed;

    [SerializeField]private float minDistance;
    [SerializeField]private float maxDistance;

    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private GameObject tile;

    void Start() {

        // Determine random movement values once on startup

        RandomizeDirection();
        speed = Random.Range(minSpeed, maxSpeed);

        respawnDistance = Random.Range(minDistance, maxDistance);

        FirstSpawn();
    }

    private void RandomizeDirection() {
        direction = Random.Range(0, 2) * 2 - 1; // Get either -1 or 1

        if (direction > 0)
        {
            isRotated = true;
        }

        transform.localPosition = new Vector3(transform.localPosition.x * -direction, transform.localPosition.y, transform.localPosition.z);
    }

    private void Update() {
        if (GameManager.instance.isPaused) { return; }
        SpawnNewObject();
    }

    private void SpawnNewObject() {
        Vector3 objectPos = currentSpawn.transform.localPosition;
        Vector3 spawnerPos = transform.localPosition;
        
        if(Mathf.Abs(spawnerPos.x - objectPos.x) > respawnDistance) {

            currentSpawn = Instantiate(objectPrefab, transform.position, GetRotation(), transform);

            Renderer tileRenderer = GetComponentInParent<Renderer>();
            if (tileRenderer != null && !tileRenderer.enabled)
            {
                foreach (Renderer r in currentSpawn.GetComponentsInChildren<Renderer>())
                {
                    r.enabled = false;
                }
            }

            // Give new object speed and direction
            currentSpawn.GetComponent<MovingObject>().Initialise(speed, direction);
            
        }
    }

    // Required to make car models face the correct direction
    private Quaternion GetRotation()
    {
        Vector3 rotation = new Vector3(-transform.eulerAngles.x,  transform.eulerAngles.y + 180, transform.eulerAngles.z);

        if (isRotated)
        {
            rotation.x = transform.eulerAngles.x;
            rotation.y = 180f;
        }

        return Quaternion.Euler(rotation);
    }

    private void FirstSpawn() {
        currentSpawn = Instantiate(objectPrefab, transform.position, GetRotation(), transform);

        // Disable renderers if preloaded
        Renderer tileRenderer = GetComponentInParent<Renderer>();
        if (tileRenderer != null && !tileRenderer.enabled)
        {
            foreach (Renderer r in currentSpawn.GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
        }

        MovingObject movingObject = currentSpawn.GetComponent<MovingObject>();

        movingObject.Initialise(speed, direction);

    }
}
