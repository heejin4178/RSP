using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Projectiles : MonoBehaviour
{
	public GameObject HitPrefab = null;

	[SerializeField] private float speed = 10f;

	private Rigidbody m_rigidbody = null;

	void Start()
	{
		m_rigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		if (speed != 0 && m_rigidbody != null)
			m_rigidbody.position += transform.forward * speed * Time.deltaTime;
	}

    void OnCollisionEnter(Collision _colision)
    {
		if(_colision.gameObject.tag == "Projectiles")
			return;

		Camera.main.GetComponent<ShakeCamera>().Shake();

		speed = 0;
		GetComponent<Rigidbody>().isKinematic = true;

		ContactPoint contact = _colision.contacts[0];
		Quaternion rotation = Quaternion.identity;
		Vector3 positon = contact.point;

		rotation = Quaternion.LookRotation(contact.normal);

		if (HitPrefab != null)
		{
			GameObject hitEffect = Instantiate(HitPrefab, positon, rotation) as GameObject;

			ParticleSystem particleSystem = hitEffect.GetComponent<ParticleSystem>();

			if (particleSystem != null)
			{
				Destroy(hitEffect, particleSystem.main.duration);
			}
		}

		Destroy(gameObject);
	}
}
