using System.Collections;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
	[SerializeField]
	private int m_count = 10;
	[SerializeField]
	private float m_amount = 0.5f;

	private bool m_isRunning = false;

	public void Shake()
	{
		if (!m_isRunning)
			StartCoroutine(ShakeCoroutine());
	}

	private IEnumerator ShakeCoroutine()
	{
		m_isRunning = true;

		Vector3 originalPositon = transform.localPosition;
		Quaternion originRotation = transform.localRotation;

		Vector3 randomVector3 = Vector3.zero;

		int count = 0;

		while (m_count > count)
		{
			count++;

			float x = Random.Range(-1f, 1f) * (m_amount / count);
			float y = Random.Range(-1f, 1f) * (m_amount / count);
			float z = Random.Range(-1f, 1f) * (m_amount / count);

			randomVector3.x = x;
			randomVector3.y = y;
			randomVector3.z = z;

			transform.localPosition = Vector3.Lerp(transform.localPosition, originalPositon + randomVector3, 0.5f);
			transform.rotation = Quaternion.Euler(transform.localRotation.eulerAngles + randomVector3);

			yield return new WaitForSeconds(0.1f);
		}

		transform.localPosition = originalPositon;
		transform.localRotation = originRotation;
		m_isRunning = false;
	}
}
