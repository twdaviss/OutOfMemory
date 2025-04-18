using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class InputManager
{
    public static PlayerControls playerControls;

    public static event Action rebindCompleted;
    public static event Action rebindCanceled;
    public static event Action disableOverlay;
    public static event Action<InputAction, int> rebindStarted;
    public static event Action<bool, string, string> compositeBeingRebound;

    //[SerializeField] private Camera activeCamera;

    public static Vector3 mouseScreenPosition;
    public static Vector3 moveDirection;
    public static Vector2 aimDirection;

    public delegate void OnPaused();
    public static event OnPaused onPaused;

    public delegate void OnInteract();
    public static event OnInteract onInteract;

    public delegate void OnContinue();
    public static event OnContinue onContinue;

    public delegate void OnScrapShot();
    public static event OnScrapShot onScrapShot;

    public delegate void OnMelee();
    public static event OnMelee onMelee;

    public delegate void OnGrappleStart();
    public static event OnGrappleStart onGrappleStart;

    public delegate void OnGrappleStop();
    public static event OnGrappleStop onGrappleStop;

    public delegate void OnSprint();
    public static event OnSprint onSprint;

    public delegate void OnMagnetize();
    public static event OnMagnetize onMagnetize;

    public delegate void OnMagnetizeScrap();
    public static event OnMagnetize onMagnetizeScrap;

    public static void Initialize()
    {
        Subscribe();
    }

    public static void PauseGame(InputAction.CallbackContext context)
    {
        onPaused?.Invoke();
        if (GameManager.Instance.IsOptionsMenuEnabled())
        {
            GameManager.Instance.DisableOptionsMenu();
            return;
        }
        else if (GameManager.Instance.IsTutorialEnabled())
        {
            GameManager.Instance.DisableTutorial();
            return;
        }
        if (GameManager.Instance.IsPauseMenuEnabled())
        {
            playerControls.Menu.Disable();
            playerControls.Gameplay.Enable();
            GameManager.Instance.DisableMenus();
        }
        else
        {
            playerControls.Menu.Enable();
            playerControls.Gameplay.Disable();
            moveDirection = Vector2.zero;
            GameManager.Instance.EnablePauseMenu();
        }
    }

    public static void ContinuePressed()
    {
        onContinue?.Invoke();
    }

    public static void UnPauseGame()
    {
        playerControls.Menu.Disable();
        playerControls.Gameplay.Enable();
    }

    public static Vector2 GetAimDirection(Vector3 position)
    {
        if(aimDirection != null && aimDirection != Vector2.zero)
        {
            Debug.Log("Stick Direction: " + aimDirection.ToString());
            return aimDirection.normalized;
        }

        Plane plane = new Plane(Vector3.back, 0.0f);
        Ray mouseRay = Camera.main.ScreenPointToRay(InputManager.mouseScreenPosition);
        Vector3 hitPoint = Vector3.zero;
        float enter;
        if (plane.Raycast(mouseRay, out enter))
        {
            //Get the point that is clicked
            hitPoint = mouseRay.GetPoint(enter);
        }

        Vector2 mouseDirection = ((Vector2)hitPoint - (Vector2)position).normalized;
        return mouseDirection;
    }

    public static void InteractPressed()
    {
        onInteract?.Invoke();
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }
    private void Sprint()
    {
        onSprint?.Invoke();
    }

    private static void ScrapShot()
    {
        onScrapShot?.Invoke();
    }

    private static void GrappleStart()
    {
        onGrappleStart?.Invoke();
    }
    private static void GrappleStop()
    {
        onGrappleStop?.Invoke();
    }
    private static void Melee()
    {
        onMelee?.Invoke();
    }

    private static void Magnetize()
    {
        onMagnetize?.Invoke();
    }

    public static void MagnetizeScrap()
    {
        onMagnetizeScrap?.Invoke();
    }

    public static void StartRebind(string actionName, int bindingIndex, TextMeshProUGUI statusText, bool excludeMouse)
    {
        InputAction action = playerControls.asset.FindAction(actionName);
        if(action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if(firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, firstPartIndex, statusText, true, excludeMouse);
            }
        }
        else
        {
            DoRebind(action, bindingIndex, statusText, false, excludeMouse);
        }
    }

    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TextMeshProUGUI statusText, bool allCompositeParts, bool excludeMouse)
    {
        if(actionToRebind == null || bindingIndex < 0)
        {
            return;
        }

        if (actionToRebind.bindings[bindingIndex].isPartOfComposite)
        {
            statusText.text = $"Binding '{actionToRebind.bindings[bindingIndex].name}'. ";
            compositeBeingRebound?.Invoke(true, statusText.text, actionToRebind.name);

        }
        else
        {
            statusText.text = "Press a " + actionToRebind.expectedControlType;
            compositeBeingRebound?.Invoke(false, "", actionToRebind.name);
        }

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);
        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();
            disableOverlay?.Invoke();
            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                { 
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse); 
                }
                else
                {
                    compositeBeingRebound?.Invoke(false, "", actionToRebind.name);
                }
            }

            SaveBindingOverride(actionToRebind);
            rebindCompleted?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            //actionToRebind.Enable();
            operation.Dispose();
            disableOverlay?.Invoke();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
        {
            rebind.WithControlsExcluding("Mouse");
        }

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start(); //actually starts the rebinding
    }

    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        InputAction action = playerControls.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for(int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(string actionName)
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        InputAction action = playerControls.asset.FindAction(actionName);

        for(int i = 0; i < action.bindings.Count; i++)
        {
            string path = PlayerPrefs.GetString(action.actionMap + action.name + i);

            if (!string.IsNullOrEmpty(path))
            {
                action.ApplyBindingOverride(i, path);
            }
        }
    }

    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = playerControls.asset.FindAction(actionName);
        if(action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }

        SaveBindingOverride(action);
    }

    ~InputManager()
    {
        GameManager.onUnPaused -= UnPauseGame;
        playerControls.Gameplay.Pause.performed -= PauseGame;
        playerControls.Menu.Pause.performed -= PauseGame;
        playerControls.Gameplay.Mouse.performed -= ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.performed -= ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Scrap.performed -= ctx => ScrapShot();
        playerControls.Gameplay.Magnetize.performed -= ctx => Magnetize();
        playerControls.Gameplay.Spin.performed -= ctx => Melee();
        playerControls.Gameplay.Grapple.performed -= ctx => GrappleStart();
        playerControls.Gameplay.Grapple.canceled -= ctx => GrappleStop();
        playerControls.Gameplay.Aim.performed -= ctx => aimDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Interact.performed -= ctx => InteractPressed();
        playerControls.Dialogue.Continue.performed -= ctx => ContinuePressed();
    }

    private static void Subscribe()
    {
        playerControls.Gameplay.Enable();
        GameManager.onUnPaused += UnPauseGame;
        playerControls.Gameplay.Pause.performed += PauseGame;
        playerControls.Menu.Pause.performed += PauseGame;

        playerControls.Gameplay.Mouse.performed += ctx => mouseScreenPosition = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Scrap.performed += ctx => ScrapShot();
        playerControls.Gameplay.Magnetize.performed += ctx => Magnetize();
        playerControls.Gameplay.Spin.performed += ctx => Melee();
        playerControls.Gameplay.Grapple.performed += ctx => GrappleStart();
        playerControls.Gameplay.Grapple.canceled += ctx => GrappleStop();
        playerControls.Gameplay.Aim.performed += ctx => aimDirection = ctx.ReadValue<Vector2>();
        playerControls.Gameplay.Interact.performed += ctx => InteractPressed();
        playerControls.Dialogue.Continue.performed += ctx => ContinuePressed();
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Ray mouseRay = Camera.main.ScreenPointToRay(mouseScreenPosition);
    //    Gizmos.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * 30);
    //}
}
