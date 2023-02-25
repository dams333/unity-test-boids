using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoidController : MonoBehaviour
{
	public Boid prefab;
	public PlayerInput boidActions;

	[Range(0, 2000)]
	int startBoids = 500;

	List<Boid> boids = new List<Boid>();

    // Start is called before the first frame update
    void Start()
    {
		for (int i = 0; i < startBoids; i++)
		{
			int x = Random.Range(Screen.width / 100, Screen.width - Screen.width / 100);
			int y = Random.Range(Screen.height / 100, Screen.height - Screen.height / 100);
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, -Camera.main.transform.position.z));
			worldPos.z = 0;
			SpawnBoid(worldPos);
		}
    }

    // Update is called once per frame
    void Update()
    {
        if(boidActions.actions["SpawnTrigger"].triggered)
		{
			Vector2 mousePos = boidActions.actions["SpawnPosition"].ReadValue<Vector2>();
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
			worldPos.z = 0;
			Collider[] colliders = Physics.OverlapSphere(worldPos, 1);
			foreach (Collider collider in colliders)
			{
				Boid boid = collider.GetComponent<Boid>();
				if (boid != null)
				{
					foreach (Boid b in boids)
					{
						b.SetSelected(false);
						if (b == boid)
						{
							b.SetSelected(true);
						}
					}
					break;
				}
			}
		}

		foreach (Boid boid in boids)
		{
			boid.UpdateBoid();
		}

		Boid selected = null;
		foreach (Boid boid in boids)
		{
			if (boid.IsSelected())
			{
				boid.SetColor(new Color(0, 0, 255, 255));
				selected = boid;
			}
			else
			{
				boid.SetColor(new Color(0, 0, 0, 255));
			}
		}
		if (selected != null)
		{
			foreach (Boid boid in selected.avoidBoids)
			{
				boid.SetColor(new Color(255, 0, 0, 255));
			}
			foreach (Boid boid in selected.alignBoids)
			{
				boid.SetColor(new Color(255, 255, 0, 255));
			}
			foreach (Boid boid in selected.approachBoids)
			{
				boid.SetColor(new Color(0, 255, 0, 255));
			}
		}
    }

	void SpawnBoid(Vector3 position)
	{
		Boid boid = Instantiate(prefab, position, Quaternion.identity);
		boids.Add(boid);
	}
}
