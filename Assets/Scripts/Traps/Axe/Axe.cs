using UnityEngine;

public class Axe : Trap
{
    private Vector3 direction;

    private void Update()
    {
        transform.Translate(direction * Time.deltaTime);
    }

    public void SetDir(Vector3 newDirection)
    {
        direction = newDirection;
    }
}
