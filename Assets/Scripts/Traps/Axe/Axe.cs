using UnityEngine;

public class Axe : Trap
{
    private Vector3 direction = Vector3.zero;

    private void Awake()
    {
        transform.Rotate(Vector3.forward, -90f);
    }

    private void Update()
    {
        transform.position += direction * Time.deltaTime;
    }

    public void SetDir(Vector3 newDirection)
    {
        direction = new Vector3(newDirection.x,0f, newDirection.z);
    }

}
