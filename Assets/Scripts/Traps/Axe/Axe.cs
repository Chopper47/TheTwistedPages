using UnityEngine;

public class Axe : Trap
{
    private Vector3 direction = Vector3.zero;

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
        transform.position += direction * Time.deltaTime;
    }

    public void SetDir(Vector3 newDirection)
    {
        direction = new Vector3(newDirection.x,0f, newDirection.z);
    }

}
