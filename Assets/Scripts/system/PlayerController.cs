using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    public float moveSpeed = 5f, runSpeed = 8f;
    private float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon;

    private Camera cam;

    public float jumpForce = 12f, gravityMod = 2.5f;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;


    public float muzzleDisplayTime;
    private float muzzleCounter;


    public Animator anim;
    public GameObject playerModel;
    public Transform modelGunPoint, gunHolder;

    public Material[] allSkins;

    public float adsSpeed = 5f;
    public Transform adsOutPoint, adsInPoint;

    public AudioSource footstepSlow, footstepFast;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;


        //SwitchGun();
        // photonView.RPC("SetGun", RpcTarget.All, selectedGun);


        //Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;

        

        playerModel.GetComponent<Renderer>().material = allSkins[photonView.Owner.ActorNumber % allSkins.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

            verticalRotStore += mouseInput.y;
            verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

            if (invertLook)
            {
                viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }
            else
            {
                viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }

            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            if (Input.GetKey(KeyCode.LeftShift))
            {
                activeMoveSpeed = runSpeed;

                if(!footstepFast.isPlaying && moveDir != Vector3.zero)
                {
                    footstepFast.Play();
                    footstepSlow.Stop();
                }
            }
            else
            {
                activeMoveSpeed = moveSpeed;

                if (!footstepSlow.isPlaying && moveDir != Vector3.zero)
                {
                    footstepFast.Stop();
                    footstepSlow.Play();
                }
            }

            if(moveDir == Vector3.zero || !isGrounded)
            {
                footstepSlow.Stop();
                footstepFast.Stop();
            }

            float yVel = movement.y;
            movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
            movement.y = yVel;

            if (charCon.isGrounded)
            {
                movement.y = 0f;
            }

            isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                movement.y = jumpForce;
            }

            movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;

            charCon.Move(movement * Time.deltaTime);

            
            anim.SetBool("grounded", isGrounded);
            anim.SetFloat("speed", moveDir.magnitude);



            // if(Input.GetMouseButton(1))
            // {
            //     cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, allGuns[selectedGun].adsZoom, adsSpeed * Time.deltaTime);
            //     gunHolder.position = Vector3.Lerp(gunHolder.position, adsInPoint.position, adsSpeed * Time.deltaTime);
            // } else
            // {
            //     cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, adsSpeed * Time.deltaTime);
            //     gunHolder.position = Vector3.Lerp(gunHolder.position, adsOutPoint.position, adsSpeed * Time.deltaTime);
            // }



            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     Cursor.lockState = CursorLockMode.None;
                
            // }
            // else if (Cursor.lockState == CursorLockMode.None)
            // {
            //     if (Input.GetMouseButtonDown(0) && UIController.instance.curState == "")
            //     {
            //         Cursor.lockState = CursorLockMode.Locked;
            //     }
            // }
        }
    }

    private void Shoot()
    {
        return;
        // Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        // ray.origin = cam.transform.position;

        // if(Physics.Raycast(ray, out RaycastHit hit))
        // {
        //     //Debug.Log("We hit " + hit.collider.gameObject.name);

        //     if (hit.collider.gameObject.tag == "Player")
        //     {
        //         Debug.Log("Hit " + hit.collider.gameObject.GetPhotonView().Owner.NickName);

        //         PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);

        //         hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);
        //     }
        //     else
        //     {

        //         GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));

        //         Destroy(bulletImpactObject, 10f);
        //     }
        // }

        // shotCounter = allGuns[selectedGun].timeBetweenShots;


        // heatCounter += allGuns[selectedGun].heatPerShot;
        // if(heatCounter >= maxHeat)
        // {
        //     heatCounter = maxHeat;

        //     overHeated = true;

        //     UIController.instance.overheatedMessage.gameObject.SetActive(true);
        // }

        // allGuns[selectedGun].muzzleFlash.SetActive(true);
        // muzzleCounter = muzzleDisplayTime;

        // allGuns[selectedGun].shotSound.Stop();
        // allGuns[selectedGun].shotSound.Play();
    }

    // [PunRPC]
    // public void DealDamage(string damager, int damageAmount, int actor)
    // {
    //     TakeDamage(damager, damageAmount, actor);
    // }

    // public void TakeDamage(string damager, int damageAmount, int actor)
    // {
    //     if (photonView.IsMine)
    //     {
    //         //Debug.Log(photonView.Owner.NickName + " has been hit by " + damager);

    //         currentHealth -= damageAmount;

    //         if (currentHealth <= 0)
    //         {
    //             currentHealth = 0;

    //             PlayerSpawner.instance.Die(damager);

    //             MatchManager.instance.UpdateStatsSend(actor, 0, 1);
    //         }


    //         UIController.instance.healthSlider.value = currentHealth;
    //     }
    // }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            if (MatchManager.instance.state == MatchManager.GameState.Playing)
            {
                cam.transform.position = viewPoint.position;
                cam.transform.rotation = viewPoint.rotation;
            } else
            {
                cam.transform.position = MatchManager.instance.mapCamPoint.position;
                cam.transform.rotation = MatchManager.instance.mapCamPoint.rotation;
            }
        }
    }

    // void SwitchGun()
    // {
    //     foreach(Gun gun in allGuns)
    //     {
    //         gun.gameObject.SetActive(false);
    //     }

    //     allGuns[selectedGun].gameObject.SetActive(true);

    //     allGuns[selectedGun].muzzleFlash.SetActive(false);
    // }

    // [PunRPC]
    // public void SetGun(int gunToSwitchTo)
    // {
    //     if(gunToSwitchTo < allGuns.Length)
    //     {
    //         selectedGun = gunToSwitchTo;
    //         SwitchGun();
    //     }
    // }
}
