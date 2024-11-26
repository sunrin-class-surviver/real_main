using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Script : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float moveInterval = 0.1f;
    public float waitBeforeFall = 1.5f;
    public float bulletSpeedMin = 3f;
    public float bulletSpeedMax = 7f;
    public int bulletCount = 20;
    public float screenMargin = 1f;

    private bool isGenerating = false;
    private Vector3 horizontalStartPosition;
    private ObjectPool bulletPool;
    private List<GameObject> currentBullets = new List<GameObject>();

    void Start()
    {
        horizontalStartPosition = spawnPoint.position;
        bulletPool = new ObjectPool(bulletPrefab, bulletCount);
        StartCoroutine(GenerateHorizontalLine());
    }

    IEnumerator GenerateHorizontalLine()
    {
        currentBullets.Clear();

        for (int i = 0; i < 18; i++)
        {
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = horizontalStartPosition + Vector3.right * i;
            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        yield return new WaitForSeconds(waitBeforeFall);

        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBullet(bullet, Vector3.down * Random.Range(bulletSpeedMin, bulletSpeedMax), "vertical"));
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(GenerateVerticalLine());
    }

    IEnumerator GenerateVerticalLine()
    {
        currentBullets.Clear();
        Vector3 rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rightEdge.x -= screenMargin;
        rightEdge.z = 0;

        for (int i = 0; i < 13; i++)
        {
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = new Vector3(
                rightEdge.x,
                rightEdge.y - (i * 0.8f),
                0f
            );
            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBullet(bullet, Vector3.left * Random.Range(bulletSpeedMin, bulletSpeedMax), "horizontal"));
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(GenerateBottomLine());
    }

    IEnumerator GenerateBottomLine()
    {
        currentBullets.Clear();
        Vector3 bottomPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        bottomPosition.y = -4.5f;
        bottomPosition.z = 0;

        for (int i = 0; i < 18; i++)
        {
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = new Vector3(bottomPosition.x + (i * 1.0f), bottomPosition.y, 0f);
            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBullet(bullet, Vector3.up * Random.Range(bulletSpeedMin, bulletSpeedMax), "vertical"));
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(GenerateVerticalLeftLine());
    }

    IEnumerator GenerateVerticalLeftLine()
    {
        if (isGenerating) yield break;
        isGenerating = true;

        currentBullets.Clear();
        Vector3 leftEdge = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        leftEdge.x += screenMargin;
        leftEdge.z = 0;

        for (int i = 0; i < 13; i++)
        {
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = new Vector3(
                leftEdge.x,
                leftEdge.y - (i * 0.8f),
                0f
            );
            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBullet(bullet, Vector3.right * Random.Range(bulletSpeedMin, bulletSpeedMax), "horizontal"));
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(GenerateHorizontalLine());
    }

    IEnumerator MoveBullet(GameObject bullet, Vector3 velocity, string direction)
    {
        while (true)
        {
            bullet.transform.position += velocity * Time.deltaTime;

            if ((direction == "vertical" && Mathf.Abs(bullet.transform.position.y) > 10f) ||
                (direction == "horizontal" && Mathf.Abs(bullet.transform.position.x) > 10f))
            {
                bulletPool.ReturnObject(bullet);
                yield break;
            }

            yield return null;
        }
    }
}

public class ObjectPool
{
    private GameObject prefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Object.Instantiate(prefab);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}