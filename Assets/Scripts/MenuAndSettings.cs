using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuAndSettings : MonoBehaviour
{
    private bool paused;

    public GameObject pauseMenu;
    public Settings settings;
    public InputActionAsset input;
    private InputAction pause;
    private bool pauseDown;
    public static MenuAndSettings instance;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);

        pause = input["Player/Pause"];
        pause.Enable();
        Debug.Log(pause);

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(pause.ReadValue<float>() > 0.5f)
        {
            Debug.Log("Pause");
            if(!pauseDown)
            {
                Debug.Log("Down");
                TogglePause();
                pauseDown = true;
            }
        }
        else
        {
            pauseDown = false;
        }
    }

    public void TogglePause()
    {
        if(paused)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            paused = false;
        }
        else
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            paused = true;
        }
    }
}
