using System.Collections;
using UnityEngine;

public class AxeThrow : MonoBehaviour
{
    [SerializeField] float timeBetweenThrows = 5f;
    [SerializeField] int throwChance = 30;

    [SerializeField] LayerMask layerMask;

    [SerializeField] float throwForce = 40f;
    [SerializeField] GameObject axe;

    private Vector3 sphere;

    void Start()
    {
        StartCoroutine(ThrowPossibility());
    }

    void Update()
    {
        
    }

    IEnumerator ThrowPossibility()
    {
        yield return new WaitForSeconds(timeBetweenThrows);

        int throwChance = Random.Range(1, 101);

        if(throwChance <= this.throwChance) 
        {
            Throw();
        }

        StartCoroutine(ThrowPossibility());
    }

    void Throw()
    {
        Vector3 dir = new Vector3(Random.Range(-1f,1f),0, Random.Range(-1f, 1f));

        dir.Normalize();
        RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up*0.7f,dir,out hit, 100f, layerMask))
        {
            sphere = hit.point;
            GameObject gO = Instantiate(axe, hit.point,Quaternion.identity,null);

            gO.GetComponent<Axe>().SetDir((transform.position - hit.point).normalized * throwForce);

            //gO.GetComponent<Rigidbody>().AddForce((transform.position - hit.point).normalized * throwForce);
        }
    }

    private void OnDrawGizmos()
    {
        if(sphere != Vector3.zero)
        {
            Gizmos.DrawSphere(sphere,1f);
        }
    }
}
