using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public GameObject player;
    public float sensitivity;
    public float senseChangeRate;
    public Quaternion tilt;

    private float xRotation;
    private InputAction look;
    private InputAction sense;

    // Start is called before the first frame update
    void Start()
    {
        look = player.GetComponent<PlayerInput>().actions.FindAction("Look");
        sense = player.GetComponent<PlayerInput>().actions.FindAction("Sensitivity");
        xRotation = 0;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookInput = look.ReadValue<Vector2>() * sensitivity * Time.deltaTime;

        player.transform.Rotate(Vector3.up * lookInput.x);

        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f) * tilt;

        float senseChange = sense.ReadValue<float>() * senseChangeRate * Time.deltaTime;
        sensitivity = Mathf.Clamp(sensitivity + senseChange, 5f, 50f);
    }
}
