using UnityEngine;

/// <summary>
/// Komponen untuk mengelola sistem senjata pemain.
/// </summary>
public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField]
    private int weaponDamage = 20;

    [SerializeField]
    private float fireRate = 0.5f;

    [SerializeField]
    private float raycastMaxDistance = 100f;

    [SerializeField]
    private LayerMask raycastLayerMask = -1;

    private Camera playerCamera;
    private float lastFireTime = 0f;

    public bool CanFire => Time.time >= lastFireTime + fireRate;

    public void Initialize(Camera camera)
    {
        playerCamera = camera;

        // Initialize raycast layer mask if not set
        if (raycastLayerMask == -1)
        {
            raycastLayerMask = Physics.DefaultRaycastLayers;
        }
    }

    /// <summary>
    /// Menembakkan raycast dari tengah kamera/layar.
    /// </summary>
    /// <returns>True jika berhasil menembak, false jika belum bisa menembak (cooldown)</returns>
    public bool FireWeapon()
    {
        if (!CanFire)
            return false;

        ShootRaycastFromCamera();
        lastFireTime = Time.time;
        return true;
    }

    /// <summary>
    /// Menembakkan raycast dari tengah kamera/layar.
    /// </summary>
    private void ShootRaycastFromCamera()
    {
        if (playerCamera == null)
            return;

        // Mendapatkan posisi tengah layar (0.5, 0.5) dalam normalized viewport coordinates
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Visualisasi raycast dengan debug line
        Debug.DrawRay(ray.origin, ray.direction * raycastMaxDistance, Color.red, 0.5f);

        // Melakukan raycast dan mencatat hit result
        if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastMaxDistance, raycastLayerMask))
        {
            ProcessHit(hitInfo, ray);
        }
    }

    /// <summary>
    /// Memproses hasil hit dari raycast.
    /// </summary>
    private void ProcessHit(RaycastHit hitInfo, Ray ray)
    {
        // Debug informasi hit
        Debug.Log(
            $"Raycast hit: {hitInfo.collider.gameObject.name} at distance {hitInfo.distance}"
        );

        // Opsional: Visualisasi titik hit
        Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 0.5f);

        // Periksa apakah objek yang terkena memiliki HealthSystem
        HealthSystem targetHealth = GetHealthSystem(hitInfo.collider);

        if (targetHealth != null)
        {
            int damageDealt = targetHealth.TakeDamage(weaponDamage, this.gameObject);
            Debug.Log($"Hit dealt {damageDealt} damage to {hitInfo.collider.gameObject.name}");
        }
        else
        {
            CheckForDamager(hitInfo.collider);
        }
    }

    /// <summary>
    /// Mencari HealthSystem pada collider atau parent-nya.
    /// </summary>
    private HealthSystem GetHealthSystem(Collider collider)
    {
        // Coba langsung pada collider
        HealthSystem health = collider.GetComponent<HealthSystem>();
        if (health != null)
            return health;

        // Coba pada parent
        return collider.GetComponentInParent<HealthSystem>();
    }

    /// <summary>
    /// Memeriksa apakah ada komponen Damager pada collider.
    /// </summary>
    private void CheckForDamager(Collider collider)
    {
        Damager damager = collider.GetComponent<Damager>();
        if (damager != null)
        {
            Debug.Log($"Hit a Damager on {collider.gameObject.name}");
        }
        else
        {
            Debug.Log($"Hit object has no health system: {collider.gameObject.name}");
        }
    }
}
