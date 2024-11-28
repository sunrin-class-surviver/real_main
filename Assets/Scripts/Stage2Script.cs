using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Script : MonoBehaviour
{

    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float moveInterval = 0.1f;

    private Vector3 horizontalStartPosition;
    private List<GameObject> currentBullets = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        horizontalStartPosition = spawnPoint.position;
        StartCoroutine(GenerateHorizontalLine());
    }

     IEnumerator GenerateHorizontalLine()
    {
        currentBullets.Clear();

        for (int i = 0; i < 18; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = horizontalStartPosition + Vector3.right * i;
            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        foreach (GameObject bullet in currentBullets)
        {
            // 각 총알에 랜덤 지연 시간 추가
            float randomDelay = Random.Range(0.0f, 1.0f);
            StartCoroutine(DropBulletDown(bullet, randomDelay));
        }

        yield return new WaitForSeconds(2f);
        //StartCoroutine(GenerateVerticalLine());
    }

    IEnumerator DropBulletDown(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody2D>();

        rb.gravityScale = 1f;
        rb.velocity = new Vector2(0f, -5f);

        while (true)
        {
            if (bullet.transform.position.y < -10f)
            {
                Destroy(bullet);
                yield break;
            }

            yield return null;
        }
    }
}
