using System.Collections;
using System.Linq;
using UnityEngine;

public class Axe : Trap
{
    [SerializeField] private float showTime = 0.4f;

    private Vector3 direction = Vector3.zero;
    private Vector3 opositeWallPoint = Vector3.zero;
    private Vector3 startingPoint = Vector3.zero;

    private bool move = false;

    [SerializeField] LayerMask wall;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] GameObject trajectory;
    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    public AudioSource Main;
    private void Awake()
    {
        transform.Rotate(Vector3.forward, -90f);
        Main=GetComponent<AudioSource>();
        /*AM=FindFirstObjectByType<AudioManager>();
        AM.Play("AxeThrow", Main);*/
        //El sonido no queda bien, empieza a hacer overlap porque dura más que el CD entre hachas
    }

    private void Update()
    {
        if (move)
        {
            transform.position += direction * Time.deltaTime;
        }
    }

    public void SetDir(Vector3 newDirection)
    {
        direction = new Vector3(newDirection.x, 0f, newDirection.z);

        trajectory.transform.parent = null;

        startingPoint = transform.position;

        if (Physics.Raycast(transform.position - Vector3.up * 0.7f, direction, out RaycastHit hit, 100f, wall))
        {
            opositeWallPoint = hit.point;
        }

        Vector3 halfOffset = (opositeWallPoint - startingPoint) / 2;
        trajectory.transform.position = transform.position + halfOffset;
        trajectory.transform.LookAt(opositeWallPoint);
        trajectory.transform.localScale = new Vector3(trajectory.transform.localScale.x, trajectory.transform.localScale.y, halfOffset.magnitude * 2.25f);

        StartCoroutine(StartMoving());
    }

    IEnumerator StartMoving()
    {
        GetComponent<MeshRenderer>().enabled = false;
        trailRenderer.enabled = false;
        yield return new WaitForSeconds(showTime);
        GetComponent<MeshRenderer>().enabled = true;
        trailRenderer.enabled = true;
        Destroy(trajectory);
        move = true;
    }
}
