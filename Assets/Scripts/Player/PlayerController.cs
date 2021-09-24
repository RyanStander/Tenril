// GENERATED AUTOMATICALLY FROM 'Assets/PlayerController.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerController : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerController"",
    ""maps"": [
        {
            ""name"": ""PlayerMovement"",
            ""id"": ""191d3e36-d213-45df-b518-f7e681e38d27"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f2b13905-b8d1-41dc-8177-33b6ac69eb03"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""743ef377-52dc-43b5-a7ea-bea35033cac2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""0c21c37e-ae07-4c39-863d-ee969794c197"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Button"",
                    ""id"": ""fd581256-d941-4346-bf56-cc9776dcb36e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""90e61187-3613-4766-b2fa-24a573833840"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""9973bdd8-78f4-4245-aad1-ad95e9ccaf8c"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""308c9d0a-4620-43af-9c41-549dea381f21"",
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
                    ""id"": ""8f8fc72b-d290-4ad3-958e-7d4ac4b9db17"",
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
                    ""id"": ""3043cc47-5b7a-435b-82ad-b23291411d88"",
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
                    ""id"": ""712c6980-4680-4169-89ea-608184aa5ba5"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Joystick"",
                    ""id"": ""551cd107-8534-4b3c-a97e-b267e182c86f"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""97faede1-5f70-4db4-aa45-889c744dc7ab"",
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
                    ""id"": ""3f503252-9342-492c-88a1-1afbd63ac136"",
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
                    ""id"": ""13504137-3561-4ff4-a161-29879401ad33"",
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
                    ""id"": ""8b49d7b5-5f3b-4989-b880-5d04d694ba87"",
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
                    ""id"": ""f92da61f-5a95-4955-b3f6-6accfd8c166b"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0d1edb2-3460-4938-9431-a7934c9c395c"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""054a81ac-5753-4bec-8a4a-2e3f8d19f914"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""21813f2e-97f9-4726-9077-48da78357fe6"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9fcda652-e62b-4b21-9552-c3ef87005ac5"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bcf45bf8-0b31-4605-9bf0-478dbe047954"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6447e6b3-af8f-4c23-b31f-06a5d6f462c1"",
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
                    ""id"": ""d62c1127-06b5-4073-82d1-24d1ace4ad5d"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerActions"",
            ""id"": ""2ba48359-9a20-465a-b09a-52d4a05e94a2"",
            ""actions"": [
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""b50a2899-aa54-4dcc-8a74-348bff051dcf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Switch Item Left"",
                    ""type"": ""Button"",
                    ""id"": ""947af87e-17d6-468a-bbec-5add577f77b9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Switch Item Right"",
                    ""type"": ""Button"",
                    ""id"": ""d63691ae-8454-4c4a-a6be-7baa56c8bdd7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Use Item"",
                    ""type"": ""Button"",
                    ""id"": ""49bdab1f-1af3-47c7-9e81-28dc1b343b35"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Draw/Sheath"",
                    ""type"": ""Button"",
                    ""id"": ""419cf071-0b9a-4af9-8dd5-eac3f014a0ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""OpenMenu"",
                    ""type"": ""Button"",
                    ""id"": ""fc8d335f-ee25-4e36-b80b-63fd4754558e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Open Map"",
                    ""type"": ""Button"",
                    ""id"": ""4884b49b-d099-451c-aef5-aad04330b0fc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a9dbdcbf-dad6-4336-ba7a-698814b170ff"",
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
                    ""id"": ""d3b87d78-7f56-49fc-ae6f-1042207dcb29"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e588b49a-058f-4f9f-9c9a-899b8bf7dc14"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Switch Item Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1f99b778-beb4-48ad-94cf-cee33b6243da"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Switch Item Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6cf8eb5e-ac03-4ba8-8959-338409d0639a"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Switch Item Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d92de44-3b65-4837-9249-8d5d432f30d8"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Switch Item Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b7a71b18-656c-4dc2-b488-0886bab8529f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Use Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8807a6a3-accc-433e-b193-be07b90681ef"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Use Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be34ab1b-c6f8-4589-85f3-f8e194e30ae9"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Draw/Sheath"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27dc3ea5-fed8-4ed8-926a-4f2257976389"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Draw/Sheath"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""645e67d2-f59b-491e-bcf2-c514aa6e712e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7db5ff34-6d94-4760-baf5-61c8cade51ce"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7f974cb-8b2a-4e7f-87fe-b3e646bd9554"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Open Map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""12769a54-1b3b-41a2-9877-cb0d2fead28c"",
                    ""path"": ""<DualShockGamepad>/touchpadButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Open Map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerCombat"",
            ""id"": ""7132d7da-70bf-47d8-96a3-d57d1565d520"",
            ""actions"": [
                {
                    ""name"": ""Strong Attack"",
                    ""type"": ""Button"",
                    ""id"": ""f575bb06-d02e-4207-b207-c0bbdb2c6e4a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Weak Attack"",
                    ""type"": ""Button"",
                    ""id"": ""e2b81b94-34b3-4809-909e-6f1e472b503c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Special Attack"",
                    ""type"": ""Button"",
                    ""id"": ""67f09c39-592a-4bc8-897b-001bb0859f70"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Block"",
                    ""type"": ""Button"",
                    ""id"": ""2bd44701-e0f5-499f-b699-4f45510e2d7f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Parry"",
                    ""type"": ""Button"",
                    ""id"": ""be13b4e3-bc8f-48be-833b-a129703f1afe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spellcasting Mode"",
                    ""type"": ""Button"",
                    ""id"": ""677417f5-f6d4-46fb-8324-24793e976145"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Lock On"",
                    ""type"": ""Button"",
                    ""id"": ""fef3721c-68df-48fc-b82d-4a1e90edba9c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f60c3aeb-b79d-43e6-b96c-7958d1bb5dac"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Weak Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a4e66611-0d05-40c2-ad9f-90a4d1dcdd9c"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Weak Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3113ad3c-bac0-48c3-a779-f9109cc1d0ca"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc2ec09d-54c2-4c4a-901b-1c60cfc623b2"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ddb1e77-bdae-4748-ae1e-19302df40bd5"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Parry"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1be2536-ee22-4af9-9d35-b2d76882f7ec"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Parry"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c69e2d27-eee3-4c0c-ac1d-2b17f5d24509"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spellcasting Mode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""421eadd2-9a7b-4d63-82e1-fc68a6bd3ecd"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Spellcasting Mode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0ba5f54f-061e-4f62-840e-910f39de7dac"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Strong Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e64555c-5feb-4772-be5d-32f5d970258a"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Strong Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eb2e3008-273c-4183-9a7f-c84a587809dd"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Special Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b97218e-5ff1-4156-81f4-d397bb04dd31"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Special Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""57f18b97-b726-4ecb-910a-79cc927fea84"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Lock On"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9cd73e8d-c822-485c-9366-cdd8b3ba144f"",
                    ""path"": ""<Gamepad>/rightStickPress"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Lock On"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerSpellcasting"",
            ""id"": ""d107cc5c-f5c3-431f-bf2a-235880598233"",
            ""actions"": [
                {
                    ""name"": ""Spell 1"",
                    ""type"": ""Button"",
                    ""id"": ""378b686d-68c3-4c32-a18d-89b871af1d8e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spell 2"",
                    ""type"": ""Button"",
                    ""id"": ""bc10c364-caf3-4fb2-8f08-784e8cdaf872"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spell 3"",
                    ""type"": ""Button"",
                    ""id"": ""7341e672-0bf5-43df-a460-11dbfe9de15d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spell 4"",
                    ""type"": ""Button"",
                    ""id"": ""830a0400-9e80-4052-bafb-faa99f3e6a2e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spell 5"",
                    ""type"": ""Button"",
                    ""id"": ""ef68da6d-b373-40d7-a077-0d336b921427"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spell 6"",
                    ""type"": ""Button"",
                    ""id"": ""853c886b-43fb-48c3-b058-b5fa333c996c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spell 7"",
                    ""type"": ""Button"",
                    ""id"": ""be8108cf-c4a4-4b04-ace9-4aedc1ae05af"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spell 8"",
                    ""type"": ""Button"",
                    ""id"": ""b6e8da20-4140-48a7-8aab-5f5808719fa0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""defcd523-fbe9-4f8e-abee-558f52c0dda2"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spell 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8076952-59f6-417d-8da8-2ac2af0f5f78"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Spell 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""20225ce4-2be4-41e8-8031-4938eabd89ca"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spell 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a748fa62-68e2-4945-9d5a-591a9390aa48"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Spell 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f0f68cd-86b9-4314-821d-3dc5a0719842"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spell 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f48fb43-5a1c-42a8-b891-9373d67abb95"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Spell 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""41c342f0-d06f-4b94-956f-0788490647ae"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spell 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9ef7d141-7e6a-4d59-aaf0-5fc3575b8842"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Spell 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""36db3353-379c-472d-8f82-323032e027dd"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spell 5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0416386d-f3db-4a43-9ade-ea81de74f0cd"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Spell 5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e6d67fe8-93b8-43b0-be4c-929e30c4cdf8"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spell 6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e994aaeb-1d64-4b2d-bddb-36a1077f1602"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Spell 6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d14dbf5-1f8c-4233-a7cd-aa573d4ead2b"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spell 7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a82c663-7a7e-4c00-b440-d1b59cedd416"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Spell 7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea43cd76-b851-4949-aad8-3a7c6e77e608"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Spell 8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1bc4be01-f3bd-43a2-933c-95e71dcc167f"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Spell 8"",
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
        // PlayerMovement
        m_PlayerMovement = asset.FindActionMap("PlayerMovement", throwIfNotFound: true);
        m_PlayerMovement_Movement = m_PlayerMovement.FindAction("Movement", throwIfNotFound: true);
        m_PlayerMovement_Look = m_PlayerMovement.FindAction("Look", throwIfNotFound: true);
        m_PlayerMovement_Sprint = m_PlayerMovement.FindAction("Sprint", throwIfNotFound: true);
        m_PlayerMovement_Dodge = m_PlayerMovement.FindAction("Dodge", throwIfNotFound: true);
        m_PlayerMovement_Jump = m_PlayerMovement.FindAction("Jump", throwIfNotFound: true);
        // PlayerActions
        m_PlayerActions = asset.FindActionMap("PlayerActions", throwIfNotFound: true);
        m_PlayerActions_Interact = m_PlayerActions.FindAction("Interact", throwIfNotFound: true);
        m_PlayerActions_SwitchItemLeft = m_PlayerActions.FindAction("Switch Item Left", throwIfNotFound: true);
        m_PlayerActions_SwitchItemRight = m_PlayerActions.FindAction("Switch Item Right", throwIfNotFound: true);
        m_PlayerActions_UseItem = m_PlayerActions.FindAction("Use Item", throwIfNotFound: true);
        m_PlayerActions_DrawSheath = m_PlayerActions.FindAction("Draw/Sheath", throwIfNotFound: true);
        m_PlayerActions_OpenMenu = m_PlayerActions.FindAction("OpenMenu", throwIfNotFound: true);
        m_PlayerActions_OpenMap = m_PlayerActions.FindAction("Open Map", throwIfNotFound: true);
        // PlayerCombat
        m_PlayerCombat = asset.FindActionMap("PlayerCombat", throwIfNotFound: true);
        m_PlayerCombat_StrongAttack = m_PlayerCombat.FindAction("Strong Attack", throwIfNotFound: true);
        m_PlayerCombat_WeakAttack = m_PlayerCombat.FindAction("Weak Attack", throwIfNotFound: true);
        m_PlayerCombat_SpecialAttack = m_PlayerCombat.FindAction("Special Attack", throwIfNotFound: true);
        m_PlayerCombat_Block = m_PlayerCombat.FindAction("Block", throwIfNotFound: true);
        m_PlayerCombat_Parry = m_PlayerCombat.FindAction("Parry", throwIfNotFound: true);
        m_PlayerCombat_SpellcastingMode = m_PlayerCombat.FindAction("Spellcasting Mode", throwIfNotFound: true);
        m_PlayerCombat_LockOn = m_PlayerCombat.FindAction("Lock On", throwIfNotFound: true);
        // PlayerSpellcasting
        m_PlayerSpellcasting = asset.FindActionMap("PlayerSpellcasting", throwIfNotFound: true);
        m_PlayerSpellcasting_Spell1 = m_PlayerSpellcasting.FindAction("Spell 1", throwIfNotFound: true);
        m_PlayerSpellcasting_Spell2 = m_PlayerSpellcasting.FindAction("Spell 2", throwIfNotFound: true);
        m_PlayerSpellcasting_Spell3 = m_PlayerSpellcasting.FindAction("Spell 3", throwIfNotFound: true);
        m_PlayerSpellcasting_Spell4 = m_PlayerSpellcasting.FindAction("Spell 4", throwIfNotFound: true);
        m_PlayerSpellcasting_Spell5 = m_PlayerSpellcasting.FindAction("Spell 5", throwIfNotFound: true);
        m_PlayerSpellcasting_Spell6 = m_PlayerSpellcasting.FindAction("Spell 6", throwIfNotFound: true);
        m_PlayerSpellcasting_Spell7 = m_PlayerSpellcasting.FindAction("Spell 7", throwIfNotFound: true);
        m_PlayerSpellcasting_Spell8 = m_PlayerSpellcasting.FindAction("Spell 8", throwIfNotFound: true);
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

    // PlayerMovement
    private readonly InputActionMap m_PlayerMovement;
    private IPlayerMovementActions m_PlayerMovementActionsCallbackInterface;
    private readonly InputAction m_PlayerMovement_Movement;
    private readonly InputAction m_PlayerMovement_Look;
    private readonly InputAction m_PlayerMovement_Sprint;
    private readonly InputAction m_PlayerMovement_Dodge;
    private readonly InputAction m_PlayerMovement_Jump;
    public struct PlayerMovementActions
    {
        private @PlayerController m_Wrapper;
        public PlayerMovementActions(@PlayerController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PlayerMovement_Movement;
        public InputAction @Look => m_Wrapper.m_PlayerMovement_Look;
        public InputAction @Sprint => m_Wrapper.m_PlayerMovement_Sprint;
        public InputAction @Dodge => m_Wrapper.m_PlayerMovement_Dodge;
        public InputAction @Jump => m_Wrapper.m_PlayerMovement_Jump;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMovementActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMovementActions instance)
        {
            if (m_Wrapper.m_PlayerMovementActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMovement;
                @Look.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnLook;
                @Sprint.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSprint;
                @Dodge.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDodge;
                @Dodge.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDodge;
                @Dodge.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDodge;
                @Jump.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_PlayerMovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Dodge.started += instance.OnDodge;
                @Dodge.performed += instance.OnDodge;
                @Dodge.canceled += instance.OnDodge;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public PlayerMovementActions @PlayerMovement => new PlayerMovementActions(this);

    // PlayerActions
    private readonly InputActionMap m_PlayerActions;
    private IPlayerActionsActions m_PlayerActionsActionsCallbackInterface;
    private readonly InputAction m_PlayerActions_Interact;
    private readonly InputAction m_PlayerActions_SwitchItemLeft;
    private readonly InputAction m_PlayerActions_SwitchItemRight;
    private readonly InputAction m_PlayerActions_UseItem;
    private readonly InputAction m_PlayerActions_DrawSheath;
    private readonly InputAction m_PlayerActions_OpenMenu;
    private readonly InputAction m_PlayerActions_OpenMap;
    public struct PlayerActionsActions
    {
        private @PlayerController m_Wrapper;
        public PlayerActionsActions(@PlayerController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Interact => m_Wrapper.m_PlayerActions_Interact;
        public InputAction @SwitchItemLeft => m_Wrapper.m_PlayerActions_SwitchItemLeft;
        public InputAction @SwitchItemRight => m_Wrapper.m_PlayerActions_SwitchItemRight;
        public InputAction @UseItem => m_Wrapper.m_PlayerActions_UseItem;
        public InputAction @DrawSheath => m_Wrapper.m_PlayerActions_DrawSheath;
        public InputAction @OpenMenu => m_Wrapper.m_PlayerActions_OpenMenu;
        public InputAction @OpenMap => m_Wrapper.m_PlayerActions_OpenMap;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActionsActions instance)
        {
            if (m_Wrapper.m_PlayerActionsActionsCallbackInterface != null)
            {
                @Interact.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnInteract;
                @SwitchItemLeft.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnSwitchItemLeft;
                @SwitchItemLeft.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnSwitchItemLeft;
                @SwitchItemLeft.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnSwitchItemLeft;
                @SwitchItemRight.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnSwitchItemRight;
                @SwitchItemRight.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnSwitchItemRight;
                @SwitchItemRight.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnSwitchItemRight;
                @UseItem.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnUseItem;
                @UseItem.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnUseItem;
                @UseItem.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnUseItem;
                @DrawSheath.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnDrawSheath;
                @DrawSheath.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnDrawSheath;
                @DrawSheath.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnDrawSheath;
                @OpenMenu.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnOpenMenu;
                @OpenMenu.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnOpenMenu;
                @OpenMap.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnOpenMap;
                @OpenMap.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnOpenMap;
                @OpenMap.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnOpenMap;
            }
            m_Wrapper.m_PlayerActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @SwitchItemLeft.started += instance.OnSwitchItemLeft;
                @SwitchItemLeft.performed += instance.OnSwitchItemLeft;
                @SwitchItemLeft.canceled += instance.OnSwitchItemLeft;
                @SwitchItemRight.started += instance.OnSwitchItemRight;
                @SwitchItemRight.performed += instance.OnSwitchItemRight;
                @SwitchItemRight.canceled += instance.OnSwitchItemRight;
                @UseItem.started += instance.OnUseItem;
                @UseItem.performed += instance.OnUseItem;
                @UseItem.canceled += instance.OnUseItem;
                @DrawSheath.started += instance.OnDrawSheath;
                @DrawSheath.performed += instance.OnDrawSheath;
                @DrawSheath.canceled += instance.OnDrawSheath;
                @OpenMenu.started += instance.OnOpenMenu;
                @OpenMenu.performed += instance.OnOpenMenu;
                @OpenMenu.canceled += instance.OnOpenMenu;
                @OpenMap.started += instance.OnOpenMap;
                @OpenMap.performed += instance.OnOpenMap;
                @OpenMap.canceled += instance.OnOpenMap;
            }
        }
    }
    public PlayerActionsActions @PlayerActions => new PlayerActionsActions(this);

    // PlayerCombat
    private readonly InputActionMap m_PlayerCombat;
    private IPlayerCombatActions m_PlayerCombatActionsCallbackInterface;
    private readonly InputAction m_PlayerCombat_StrongAttack;
    private readonly InputAction m_PlayerCombat_WeakAttack;
    private readonly InputAction m_PlayerCombat_SpecialAttack;
    private readonly InputAction m_PlayerCombat_Block;
    private readonly InputAction m_PlayerCombat_Parry;
    private readonly InputAction m_PlayerCombat_SpellcastingMode;
    private readonly InputAction m_PlayerCombat_LockOn;
    public struct PlayerCombatActions
    {
        private @PlayerController m_Wrapper;
        public PlayerCombatActions(@PlayerController wrapper) { m_Wrapper = wrapper; }
        public InputAction @StrongAttack => m_Wrapper.m_PlayerCombat_StrongAttack;
        public InputAction @WeakAttack => m_Wrapper.m_PlayerCombat_WeakAttack;
        public InputAction @SpecialAttack => m_Wrapper.m_PlayerCombat_SpecialAttack;
        public InputAction @Block => m_Wrapper.m_PlayerCombat_Block;
        public InputAction @Parry => m_Wrapper.m_PlayerCombat_Parry;
        public InputAction @SpellcastingMode => m_Wrapper.m_PlayerCombat_SpellcastingMode;
        public InputAction @LockOn => m_Wrapper.m_PlayerCombat_LockOn;
        public InputActionMap Get() { return m_Wrapper.m_PlayerCombat; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerCombatActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerCombatActions instance)
        {
            if (m_Wrapper.m_PlayerCombatActionsCallbackInterface != null)
            {
                @StrongAttack.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnStrongAttack;
                @StrongAttack.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnStrongAttack;
                @StrongAttack.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnStrongAttack;
                @WeakAttack.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnWeakAttack;
                @WeakAttack.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnWeakAttack;
                @WeakAttack.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnWeakAttack;
                @SpecialAttack.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnSpecialAttack;
                @SpecialAttack.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnSpecialAttack;
                @SpecialAttack.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnSpecialAttack;
                @Block.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnBlock;
                @Block.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnBlock;
                @Block.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnBlock;
                @Parry.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnParry;
                @Parry.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnParry;
                @Parry.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnParry;
                @SpellcastingMode.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnSpellcastingMode;
                @SpellcastingMode.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnSpellcastingMode;
                @SpellcastingMode.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnSpellcastingMode;
                @LockOn.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnLockOn;
                @LockOn.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnLockOn;
                @LockOn.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnLockOn;
            }
            m_Wrapper.m_PlayerCombatActionsCallbackInterface = instance;
            if (instance != null)
            {
                @StrongAttack.started += instance.OnStrongAttack;
                @StrongAttack.performed += instance.OnStrongAttack;
                @StrongAttack.canceled += instance.OnStrongAttack;
                @WeakAttack.started += instance.OnWeakAttack;
                @WeakAttack.performed += instance.OnWeakAttack;
                @WeakAttack.canceled += instance.OnWeakAttack;
                @SpecialAttack.started += instance.OnSpecialAttack;
                @SpecialAttack.performed += instance.OnSpecialAttack;
                @SpecialAttack.canceled += instance.OnSpecialAttack;
                @Block.started += instance.OnBlock;
                @Block.performed += instance.OnBlock;
                @Block.canceled += instance.OnBlock;
                @Parry.started += instance.OnParry;
                @Parry.performed += instance.OnParry;
                @Parry.canceled += instance.OnParry;
                @SpellcastingMode.started += instance.OnSpellcastingMode;
                @SpellcastingMode.performed += instance.OnSpellcastingMode;
                @SpellcastingMode.canceled += instance.OnSpellcastingMode;
                @LockOn.started += instance.OnLockOn;
                @LockOn.performed += instance.OnLockOn;
                @LockOn.canceled += instance.OnLockOn;
            }
        }
    }
    public PlayerCombatActions @PlayerCombat => new PlayerCombatActions(this);

    // PlayerSpellcasting
    private readonly InputActionMap m_PlayerSpellcasting;
    private IPlayerSpellcastingActions m_PlayerSpellcastingActionsCallbackInterface;
    private readonly InputAction m_PlayerSpellcasting_Spell1;
    private readonly InputAction m_PlayerSpellcasting_Spell2;
    private readonly InputAction m_PlayerSpellcasting_Spell3;
    private readonly InputAction m_PlayerSpellcasting_Spell4;
    private readonly InputAction m_PlayerSpellcasting_Spell5;
    private readonly InputAction m_PlayerSpellcasting_Spell6;
    private readonly InputAction m_PlayerSpellcasting_Spell7;
    private readonly InputAction m_PlayerSpellcasting_Spell8;
    public struct PlayerSpellcastingActions
    {
        private @PlayerController m_Wrapper;
        public PlayerSpellcastingActions(@PlayerController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Spell1 => m_Wrapper.m_PlayerSpellcasting_Spell1;
        public InputAction @Spell2 => m_Wrapper.m_PlayerSpellcasting_Spell2;
        public InputAction @Spell3 => m_Wrapper.m_PlayerSpellcasting_Spell3;
        public InputAction @Spell4 => m_Wrapper.m_PlayerSpellcasting_Spell4;
        public InputAction @Spell5 => m_Wrapper.m_PlayerSpellcasting_Spell5;
        public InputAction @Spell6 => m_Wrapper.m_PlayerSpellcasting_Spell6;
        public InputAction @Spell7 => m_Wrapper.m_PlayerSpellcasting_Spell7;
        public InputAction @Spell8 => m_Wrapper.m_PlayerSpellcasting_Spell8;
        public InputActionMap Get() { return m_Wrapper.m_PlayerSpellcasting; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerSpellcastingActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerSpellcastingActions instance)
        {
            if (m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface != null)
            {
                @Spell1.started -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell1;
                @Spell1.performed -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell1;
                @Spell1.canceled -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell1;
                @Spell2.started -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell2;
                @Spell2.performed -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell2;
                @Spell2.canceled -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell2;
                @Spell3.started -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell3;
                @Spell3.performed -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell3;
                @Spell3.canceled -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell3;
                @Spell4.started -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell4;
                @Spell4.performed -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell4;
                @Spell4.canceled -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell4;
                @Spell5.started -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell5;
                @Spell5.performed -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell5;
                @Spell5.canceled -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell5;
                @Spell6.started -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell6;
                @Spell6.performed -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell6;
                @Spell6.canceled -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell6;
                @Spell7.started -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell7;
                @Spell7.performed -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell7;
                @Spell7.canceled -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell7;
                @Spell8.started -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell8;
                @Spell8.performed -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell8;
                @Spell8.canceled -= m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface.OnSpell8;
            }
            m_Wrapper.m_PlayerSpellcastingActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Spell1.started += instance.OnSpell1;
                @Spell1.performed += instance.OnSpell1;
                @Spell1.canceled += instance.OnSpell1;
                @Spell2.started += instance.OnSpell2;
                @Spell2.performed += instance.OnSpell2;
                @Spell2.canceled += instance.OnSpell2;
                @Spell3.started += instance.OnSpell3;
                @Spell3.performed += instance.OnSpell3;
                @Spell3.canceled += instance.OnSpell3;
                @Spell4.started += instance.OnSpell4;
                @Spell4.performed += instance.OnSpell4;
                @Spell4.canceled += instance.OnSpell4;
                @Spell5.started += instance.OnSpell5;
                @Spell5.performed += instance.OnSpell5;
                @Spell5.canceled += instance.OnSpell5;
                @Spell6.started += instance.OnSpell6;
                @Spell6.performed += instance.OnSpell6;
                @Spell6.canceled += instance.OnSpell6;
                @Spell7.started += instance.OnSpell7;
                @Spell7.performed += instance.OnSpell7;
                @Spell7.canceled += instance.OnSpell7;
                @Spell8.started += instance.OnSpell8;
                @Spell8.performed += instance.OnSpell8;
                @Spell8.canceled += instance.OnSpell8;
            }
        }
    }
    public PlayerSpellcastingActions @PlayerSpellcasting => new PlayerSpellcastingActions(this);
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
    public interface IPlayerMovementActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnDodge(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
    public interface IPlayerActionsActions
    {
        void OnInteract(InputAction.CallbackContext context);
        void OnSwitchItemLeft(InputAction.CallbackContext context);
        void OnSwitchItemRight(InputAction.CallbackContext context);
        void OnUseItem(InputAction.CallbackContext context);
        void OnDrawSheath(InputAction.CallbackContext context);
        void OnOpenMenu(InputAction.CallbackContext context);
        void OnOpenMap(InputAction.CallbackContext context);
    }
    public interface IPlayerCombatActions
    {
        void OnStrongAttack(InputAction.CallbackContext context);
        void OnWeakAttack(InputAction.CallbackContext context);
        void OnSpecialAttack(InputAction.CallbackContext context);
        void OnBlock(InputAction.CallbackContext context);
        void OnParry(InputAction.CallbackContext context);
        void OnSpellcastingMode(InputAction.CallbackContext context);
        void OnLockOn(InputAction.CallbackContext context);
    }
    public interface IPlayerSpellcastingActions
    {
        void OnSpell1(InputAction.CallbackContext context);
        void OnSpell2(InputAction.CallbackContext context);
        void OnSpell3(InputAction.CallbackContext context);
        void OnSpell4(InputAction.CallbackContext context);
        void OnSpell5(InputAction.CallbackContext context);
        void OnSpell6(InputAction.CallbackContext context);
        void OnSpell7(InputAction.CallbackContext context);
        void OnSpell8(InputAction.CallbackContext context);
    }
}
