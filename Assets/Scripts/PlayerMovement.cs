using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float sprint = 2f;
    public int maxStamina = 2500;
    public int stamina = 2500;
    public float gravity = 9.8f;
    public GameObject flashlight;
    public bool on = false;
    public bool canMove = true;
    public Text staminaText;
    public Texture flashon, flashoff;
    public RawImage flashsprite;
    public Slider staminaBar;

    FMOD.Studio.EventInstance Footsteps;
    FMOD.Studio.EventDescription Speed;
    FMOD.Studio.PARAMETER_DESCRIPTION spd;
    FMOD.Studio.PARAMETER_ID SID;

    FMOD.Studio.PLAYBACK_STATE pbs;
    // Update is called once per frame

    private void Start()
    {
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
        Footsteps = FMODUnity.RuntimeManager.CreateInstance("event:/player/footsteps");

        Speed = FMODUnity.RuntimeManager.GetEventDescription("event:/player/footsteps");
        Speed.getParameterDescriptionByName("Speed", out spd);
        SID = spd.id;
    }

    void Update()
    {
        if (canMove)
        {
            //staminaText.text = "Remaining stamina: " + stamina.ToString();
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(Footsteps, GetComponent<Transform>(), GetComponent<Rigidbody>());
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = 0f;
            playsound(x, z);
            if (!controller.isGrounded)
            {
                y = -gravity;
            }

            Vector3 move = transform.right * x + transform.up * y + transform.forward * z;
            if (Input.GetKey(KeyCode.LeftShift) && stamina >0)
            {
                Footsteps.setParameterByID(SID, 1f);
                controller.Move(move * speed * sprint * Time.deltaTime);
                stamina -= 1;
            }
            else
            {
                Footsteps.setParameterByID(SID, 0f);
                controller.Move(move * speed * Time.deltaTime);
                
            }
            if (!Input.GetKey(KeyCode.LeftShift) && stamina < maxStamina)
            {
                stamina += 1;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                on = !on;
                flashlight.SetActive(on);
                if (on)
                {
                    flashsprite.texture = flashon;
                }
                else
                {
                    flashsprite.texture = flashoff;
                }
            }
        }
        staminaBar.value = stamina;
    }
    public void playsound(float x, float z)
    {
        Footsteps.getPlaybackState(out pbs);
        if (x != 0f || z != 0f)
        {
            if (pbs != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                Footsteps.start();
            }
        }
        else
        {
            Footsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
    private void OnDestroy()
    {
        Footsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
