using UnityEngine;

/// <summary>
/// Controls and fires a beam gameobject from a cannon object, dealing damage to a damagable target on impact.
/// </summary>
public class CannonController : MonoBehaviour {
    
    // The gameobject representing the cannon beam.
    public GameObject beam;
    // The effect triggered upon the beam impact.
    public GameObject impactEffect;
    // The layers the beam can impact.
    public LayerMask impactLayers;
    // The maximum range the beam can be fired.
    public float beamMaxRange;
    // The amount of damage the beam deals.
    public float damage;
    // The rate at which the beam can deal damage.
    public float beamRate;

    // The counter for ticking the rate at which the beam is fired.
    private float beamCounter;
    // The tracker for locking onto a given target.
    private CameraTracking tracker;
    // The target that is currently locked onto by the cannon.
    private Transform target;

	// Use this for initialization
	void Start () {
        tracker = Camera.main.gameObject.GetComponent<CameraTracking>();
        beam.SetActive(false);
        beamCounter = beamRate;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        target = tracker.getTargetTransform();
        if (target != null)
        {
            if (tracker.IsLookingAtTarget())
            {
                AimCannon();
            }
            else
            {
                ResetCannon();
            }
        }
        else
        {
            ResetCannon();
        }
        BeamTick();
        if (Input.GetButton("Fire") && beamCounter == beamRate)
        {
            FireBeam();
            beamCounter = 0;
        }
        else
        {
            beam.SetActive(false);
        }
    }

    /// <summary>
    /// Ticks the beam counter for firing at a constant rate.
    /// </summary>
    private void BeamTick()
    {
        if(beamCounter < beamRate)
        {
            beamCounter += Time.fixedDeltaTime;
            if(beamCounter > beamRate)
            {
                beamCounter = beamRate;
            }
        }
    }

    /// <summary>
    /// Faces the cannon to look towards the target.
    /// </summary>
    private void AimCannon()
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - this.transform.position);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, lookRotation, 2.0f);
    }

    /// <summary>
    /// Resets the cannon to it's default position.
    /// </summary>
    private void ResetCannon()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.zero), 0.2f);
    }

    /// <summary>
    /// Fire's the beam in the direction the cannon is facing while determining it's length and the nearest impact.
    /// </summary>
    private void FireBeam()
    {
        RaycastHit[] hits = Physics.RaycastAll(this.transform.position, this.transform.forward, beamMaxRange, impactLayers);
        if(hits.Length == 0)
        {
            SetBeamLength(beamMaxRange);
        } else
        {
            RaycastHit hit = new RaycastHit();
            float closestImpactDistance = GetClosestImpact(hits, ref hit);

            beam.SetActive(true);
            SetBeamLength(closestImpactDistance);
            BeamImpactEffect();
            BeamDamage(hit);
        }
    }

    /// <summary>
    /// Sets the beam length to a desired length, not exceeding the beam's maximum length.
    /// </summary>
    /// <param name="targetLength">The desired length of the beam.</param>
    private void SetBeamLength(float targetLength)
    {
        float beamLength = targetLength;
        if (beamLength > beamMaxRange)
        {
            beamLength = beamMaxRange;
        }
        beam.transform.localPosition = new Vector3(0, 0, beamLength / 2.0f);
        impactEffect.transform.localPosition = new Vector3(0, 0, beamLength);
        beam.transform.localScale = new Vector3(beam.transform.localScale.x, beamLength / 2.0f, beam.transform.localScale.z);
    }

    /// <summary>
    /// Finds the beam's closest impact using a Raycast fired in the direction the cannon is facing, returning the distance to the closest impact.
    /// </summary>
    /// <param name="hits">The list of RaycastHit.</param>
    /// <param name="hit">The hit being considered as the closest hit.</param>
    /// <returns>The distance to the closest impact.</returns>
    private float GetClosestImpact(RaycastHit[] hits, ref RaycastHit hit)
    {
        float closestImpactDistance = -1.0f;

        for (int i = 0; i < hits.Length; i++)
        {
            if (!this.transform.IsChildOf(hits[i].transform))
            {
                if (closestImpactDistance < 0)
                {
                    closestImpactDistance = hits[i].distance;
                    hit = hits[i];
                }
                else
                {
                    float impactCurrentDistance = hits[i].distance;
                    if (impactCurrentDistance < closestImpactDistance)
                    {
                        closestImpactDistance = impactCurrentDistance;
                        hit = hits[i];
                    }
                }
            }
        }

        return closestImpactDistance;
    }

    /// <summary>
    /// Emits a particle effect on the beam's impact.
    /// </summary>
    private void BeamImpactEffect()
    {
        impactEffect.GetComponent<ParticleSystem>().Emit(50);
    }

    /// <summary>
    /// Determines if the impact made is damagable. If so, the beam deals damage to the gameobject impacted.
    /// </summary>
    /// <param name="hit">The gameobject being considered for damage.</param>
    private void BeamDamage(RaycastHit hit)
    {
        IDamageable damageable = (IDamageable)hit.transform.GetComponent(typeof(IDamageable));
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
}
