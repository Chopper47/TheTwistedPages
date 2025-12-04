using UnityEngine;

public class Axe : Trap
{
    private Vector3 direction = Vector3.zero;

    private void Awake()
    {
        transform.Rotate(Vector3.up, -90f);
    }

    private void Update()
    {
        transform.Translate(direction * Time.deltaTime);
    }

    public void SetDir(Vector3 newDirection)
    {
        direction = newDirection;
    }

}
