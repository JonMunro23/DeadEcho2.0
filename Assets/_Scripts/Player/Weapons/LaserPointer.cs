using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    LineRenderer laserLine;
    [SerializeField] Transform bulletSpawnTransform;
    [SerializeField] GameObject laserPointPrefab;
    GameObject spawnedLaserPoint;

    private void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    //private void Start()
    //{
    //    laserLine.enabled = false;
    //}

    void Update()
    {
        if (laserLine.enabled)
        {
            UpdateLaserPos();

            RaycastHit hit;
            if (Physics.Raycast(bulletSpawnTransform.position, bulletSpawnTransform.forward, out hit, Mathf.Infinity))
            {
                Vector3 laserPointSpawnLocation = hit.point + (hit.normal * .01f);
                Quaternion laserPointSpawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                if(spawnedLaserPoint == null)
                {
                    spawnedLaserPoint = Instantiate(laserPointPrefab, laserPointSpawnLocation, laserPointSpawnRotation);
                }

                spawnedLaserPoint.transform.localPosition = laserPointSpawnLocation;
                spawnedLaserPoint.transform.localRotation = laserPointSpawnRotation;

            }
            else
            {
                if (spawnedLaserPoint != null)
                    RemoveLaserPoint();
            }
        }
    }

    public void RemoveLaser()
    {
        laserLine.enabled = false;

        if (spawnedLaserPoint != null)
            RemoveLaserPoint();
    }

    void RemoveLaserPoint()
    {
        Destroy(spawnedLaserPoint.gameObject);
    }

    public void SpawnLaser(Transform _bulletSpawnTransform)
    {
        laserLine.enabled = true;
        bulletSpawnTransform = _bulletSpawnTransform;

        laserLine.SetPosition(0, transform.position);
        laserLine.SetPosition(1, _bulletSpawnTransform.position + _bulletSpawnTransform.forward * 1);
    }

    void UpdateLaserPos()
    {
        laserLine.SetPosition(0, transform.position);
        laserLine.SetPosition(1, bulletSpawnTransform.position + bulletSpawnTransform.forward * 1);
    }
}
