using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
	Transform transform;

	[Range(1, 15)]
	float speed = 3;

	[Range(0.1f, 10)]
	public float avoidDistance = 0.5f;
	[Range(0, 1)]
	public float avoidWeight = 0.7f;

	[Range(0.1f, 10)]
	public float alignDistance = 3;
	[Range(0, 1)]
	public float alignWeight = 0.6f;

	[Range(0.1f, 10)]
	public float approachDistance = 5;
	[Range(0, 1)]
	public float approachWeight = 0.08f;

	[HideInInspector]
	public Vector2 velocity;

	bool selected = false;

	Material material;

	[HideInInspector]
	public List<Boid> avoidBoids = new List<Boid>();
	[HideInInspector]
	public List<Boid> alignBoids = new List<Boid>();
	[HideInInspector]
	public List<Boid> approachBoids = new List<Boid>();

	void Awake()
	{
		transform = GetComponent<Transform>();
		material = transform.Find("Body").GetComponent<MeshRenderer>().material;
		velocity = Random.insideUnitCircle.normalized;
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void UpdateBoid()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		if(screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
		{
			if (screenPos.x < 0)
			{
				screenPos.x = Screen.width;
			}
			else if (screenPos.x > Screen.width)
			{
				screenPos.x = 0;
			}

			if (screenPos.y < 0)
			{
				screenPos.y = Screen.height;
			}
			else if (screenPos.y > Screen.height)
			{
				screenPos.y = 0;
			}
		}
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));
		worldPos.z = 0;
		transform.position = worldPos;

		avoidBoids.Clear();
		alignBoids.Clear();
		approachBoids.Clear();

		Collider[] colliders = Physics.OverlapSphere(transform.position, approachDistance);
		foreach (Collider collider in colliders)
		{
			Boid boid = collider.GetComponent<Boid>();
			if (boid != null && boid != this)
			{
				float distance = Vector2.Distance(transform.position, boid.transform.position);
				if (distance < avoidDistance)
				{
					avoidBoids.Add(boid);
				}
				else if (distance < alignDistance)
				{
					alignBoids.Add(boid);
				}
				else if (distance < approachDistance)
				{
					approachBoids.Add(boid);
				}
			}
		}


		if (avoidBoids.Count > 0)
		{
			Vector2 avoid = Vector2.zero;
			foreach (Boid boid in avoidBoids)
			{
				avoid += (Vector2)(transform.position - boid.transform.position);
			}
			avoid /= avoidBoids.Count;
			avoid *= avoidWeight;
			velocity += avoid;
		}
		else
		{
			if (alignBoids.Count > 0)
			{
				Vector2 align = Vector2.zero;
				foreach (Boid boid in alignBoids)
				{
					align += boid.velocity;
				}
				align /= alignBoids.Count;
				align *= alignWeight;
				velocity += align;
			}

			if (approachBoids.Count > 0)
			{
				Vector2 approach = Vector2.zero;
				foreach (Boid boid in approachBoids)
				{
					approach += (Vector2)(boid.transform.position - transform.position);
				}
				approach /= approachBoids.Count;
				approach *= approachWeight;
				velocity += approach;
			}
		}
	}
    
    void FixedUpdate()
    {
		velocity = velocity.normalized;
		transform.position += new Vector3(velocity.x, velocity.y, 0) * speed * Time.deltaTime;
    }

	public void SetSelected(bool selected)
	{
		this.selected = selected;
	}

	public bool IsSelected()
	{
		return selected;
	}

	public void SetColor(Color color)
	{
		material.color = color;
	}
}
