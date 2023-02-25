// GENERATED AUTOMATICALLY FROM 'Assets/Script/Utils/Import/InputActions/PlayerInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""9fac386a-e300-4402-9a30-7c6bb5ed80cc"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""3e663530-0ba8-4032-a960-c7f19879cdfd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""210ec766-58c8-409f-b7f7-8541dff5a646"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""df265a42-cc61-4e89-a167-d114218e4c90"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bde10122-ba8b-4898-bfad-8f97f4f90f90"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc660456-81fd-4542-930a-9ac94f9f1372"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""03234c4c-c00f-4671-97cf-e71a0479ec2b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e2a7b64c-af1a-4ad4-9ffa-ed8cc531e4e1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""57608719-1822-4279-bd88-073af30ada4c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""47f445cd-5a38-4b27-a966-b5faa8da7c0a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8b753a4c-77cb-4c72-93ba-2af3f7289eda"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""a126c9cc-2a57-4004-9fbc-cd79c07be480"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f62536b5-0cb3-427c-ba85-b61f4bab7cd8"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f3a66ccc-1b18-4a8c-8661-fd71c1395701"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c547a6f1-08a2-40f7-8721-5e5a1bdc95d0"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4d88a030-18ef-4dcf-b862-fc5dd39c1086"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""262b730b-334f-46c2-b1a5-2bb077882beb"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Archery"",
            ""id"": ""4003dde2-97b9-4c7a-be84-62de659cce13"",
            ""actions"": [
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""48b9b0b5-7260-4bc1-b8a0-975e979b4c6b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""e04735fc-5910-48df-be5b-c965a8ad599c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""be0c62fd-5905-47e6-a6d7-e4e7c4fd61a3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86b6f906-461d-49fb-afe6-f94f0ead9162"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menus"",
            ""id"": ""f0b57d89-8d0a-49cc-bccc-312a98f9f337"",
            ""actions"": [
                {
                    ""name"": ""Debug"",
                    ""type"": ""Button"",
                    ""id"": ""e400963d-2c6f-49b4-b961-3af0b0ac5a3d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""23f01d82-a34d-4222-a5be-08ed03d2b05f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""b019acbc-0bf5-416d-9a2a-8f20c3bad5a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""Scale(factor=10)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""a5f8ea89-12d2-4968-b870-6792bd15dd3c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Previous"",
                    ""type"": ""Button"",
                    ""id"": ""5ebed10c-0a6b-4b2d-a250-fa5f1aceeb26"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Next"",
                    ""type"": ""Button"",
                    ""id"": ""45520ad9-679a-4df8-9296-9d2e1cfa4277"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9a9f3da7-0982-49a0-bbaf-85f3af7cee8a"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Debug"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4a5aea4d-fe8a-43c1-bdd1-954ee8ffc27d"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Debug"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd671c08-fd61-485e-81bf-568602b2cb7b"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Debug"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03f7633f-76ac-4bad-a955-b039cd24e10d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Next"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38f686ff-2d71-477e-80fb-0f874c01d24f"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Next"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13a0e117-5054-4f80-83b9-0170805fce49"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Previous"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c86af621-075a-4cf9-8e9f-ab5e4ef791b2"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Previous"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""389fd6c4-126e-4b06-88db-6342da9fa8e6"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8069dc11-d45e-4bea-a9a5-a09a27cae43a"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f9f7a4fc-2c06-463f-ac9e-f16ac9e711df"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6218d96e-515e-43f2-819b-91dc206338c4"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86136246-6e25-4fba-9baa-c7c8eca22c1c"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d9505a1-7dc7-4b6f-8651-a7ed69f97eb3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Quit = m_Player.FindAction("Quit", throwIfNotFound: true);
        // Archery
        m_Archery = asset.FindActionMap("Archery", throwIfNotFound: true);
        m_Archery_Reload = m_Archery.FindAction("Reload", throwIfNotFound: true);
        m_Archery_Shoot = m_Archery.FindAction("Shoot", throwIfNotFound: true);
        // Menus
        m_Menus = asset.FindActionMap("Menus", throwIfNotFound: true);
        m_Menus_Debug = m_Menus.FindAction("Debug", throwIfNotFound: true);
        m_Menus_Right = m_Menus.FindAction("Right", throwIfNotFound: true);
        m_Menus_Left = m_Menus.FindAction("Left", throwIfNotFound: true);
        m_Menus_Interact = m_Menus.FindAction("Interact", throwIfNotFound: true);
        m_Menus_Previous = m_Menus.FindAction("Previous", throwIfNotFound: true);
        m_Menus_Next = m_Menus.FindAction("Next", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Quit;
    public struct PlayerActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Quit => m_Wrapper.m_Player_Quit;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Quit.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
                @Quit.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
                @Quit.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQuit;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Quit.started += instance.OnQuit;
                @Quit.performed += instance.OnQuit;
                @Quit.canceled += instance.OnQuit;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Archery
    private readonly InputActionMap m_Archery;
    private IArcheryActions m_ArcheryActionsCallbackInterface;
    private readonly InputAction m_Archery_Reload;
    private readonly InputAction m_Archery_Shoot;
    public struct ArcheryActions
    {
        private @PlayerInputActions m_Wrapper;
        public ArcheryActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Reload => m_Wrapper.m_Archery_Reload;
        public InputAction @Shoot => m_Wrapper.m_Archery_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_Archery; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ArcheryActions set) { return set.Get(); }
        public void SetCallbacks(IArcheryActions instance)
        {
            if (m_Wrapper.m_ArcheryActionsCallbackInterface != null)
            {
                @Reload.started -= m_Wrapper.m_ArcheryActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_ArcheryActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_ArcheryActionsCallbackInterface.OnReload;
                @Shoot.started -= m_Wrapper.m_ArcheryActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_ArcheryActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_ArcheryActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_ArcheryActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
            }
        }
    }
    public ArcheryActions @Archery => new ArcheryActions(this);

    // Menus
    private readonly InputActionMap m_Menus;
    private IMenusActions m_MenusActionsCallbackInterface;
    private readonly InputAction m_Menus_Debug;
    private readonly InputAction m_Menus_Right;
    private readonly InputAction m_Menus_Left;
    private readonly InputAction m_Menus_Interact;
    private readonly InputAction m_Menus_Previous;
    private readonly InputAction m_Menus_Next;
    public struct MenusActions
    {
        private @PlayerInputActions m_Wrapper;
        public MenusActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Debug => m_Wrapper.m_Menus_Debug;
        public InputAction @Right => m_Wrapper.m_Menus_Right;
        public InputAction @Left => m_Wrapper.m_Menus_Left;
        public InputAction @Interact => m_Wrapper.m_Menus_Interact;
        public InputAction @Previous => m_Wrapper.m_Menus_Previous;
        public InputAction @Next => m_Wrapper.m_Menus_Next;
        public InputActionMap Get() { return m_Wrapper.m_Menus; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenusActions set) { return set.Get(); }
        public void SetCallbacks(IMenusActions instance)
        {
            if (m_Wrapper.m_MenusActionsCallbackInterface != null)
            {
                @Debug.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnDebug;
                @Debug.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnDebug;
                @Debug.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnDebug;
                @Right.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnRight;
                @Left.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnLeft;
                @Interact.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnInteract;
                @Previous.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnPrevious;
                @Previous.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnPrevious;
                @Previous.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnPrevious;
                @Next.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnNext;
                @Next.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnNext;
                @Next.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnNext;
            }
            m_Wrapper.m_MenusActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Debug.started += instance.OnDebug;
                @Debug.performed += instance.OnDebug;
                @Debug.canceled += instance.OnDebug;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Previous.started += instance.OnPrevious;
                @Previous.performed += instance.OnPrevious;
                @Previous.canceled += instance.OnPrevious;
                @Next.started += instance.OnNext;
                @Next.performed += instance.OnNext;
                @Next.canceled += instance.OnNext;
            }
        }
    }
    public MenusActions @Menus => new MenusActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnQuit(InputAction.CallbackContext context);
    }
    public interface IArcheryActions
    {
        void OnReload(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
    public interface IMenusActions
    {
        void OnDebug(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnLeft(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnPrevious(InputAction.CallbackContext context);
        void OnNext(InputAction.CallbackContext context);
    }
}
