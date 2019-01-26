﻿using UnityEngine;
using UnityEngine.UI;
using CoreGame;

#pragma warning disable 0649

/// <summary>
/// Clase que controla al jugador y su interacción con el grid de zanahorias
/// </summary>
public class Player : MonoBehaviour {
    //Player
    Rigidbody2D playerRigidBody;
    SpriteRenderer playerSpriteRenderer;
    Animator playerAnimator;
    [SerializeField]
    float moveSpeed;
    float move;
    int playerPosX, playerPosY;

    //Carrot grid
    [SerializeField]
    GameObject carrotGridObject;
    //CarrotGrid carrotGrid;
    
    //Input
    [SerializeField]
    Sprite A_Button, B_Button, X_Button, Y_Button;
    [SerializeField]
    GameObject currentInputObject;
    SpriteRenderer currentInputSR;
    string currentInputString;
    int currentInputPulls;
    int randomInput;
    string[] inputsArray;
    Sprite[] spriteButtonsArray;

    //Collision
    bool collisionStay;
    string collisionName;

    //UI
    [SerializeField]
    Text moneyText;

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip audioCoin, audioBoo, audioPull;

    [SerializeField]
    GameObject mobileInputsObject;
    //MobileInputs mobileInputs;

    private void Awake() {
        playerAnimator = GetComponent<Animator>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        //carrotGrid = carrotGridObject.GetComponent<CarrotGrid>();
        currentInputSR = currentInputObject.GetComponent<SpriteRenderer>();

        //mobileInputs = mobileInputsObject.GetComponent<MobileInputs>();
    }

    // Use this for initialization
    void Start() {
        currentInputString = "A_Button";
        currentInputPulls = 0;
        inputsArray = new string[4] { "A_Button", "B_Button", "X_Button", "Y_Button" };
        spriteButtonsArray = new Sprite[4] { A_Button, B_Button, X_Button, Y_Button };
    }

    private void FixedUpdate() {
        /*//Caminar
        //Vector2 leftJoystick = Controls.LeftJoystick();
        Vector2 leftJoystick = mobileInputs.Direction;
        playerRigidBody.velocity = new Vector2(leftJoystick.x, leftJoystick.y) * moveSpeed;
        playerAnimator.SetFloat("Velocity", Mathf.Abs(leftJoystick.x) + Mathf.Abs(leftJoystick.y));
        //Flipear el sprite
        if (leftJoystick.x > 0) {
            playerSpriteRenderer.flipX = false;
        }
        else if (leftJoystick.x < 0) {
            playerSpriteRenderer.flipX = true;
        }*/
    }

    // Update is called once per frame
    void Update() {
        /*//El jugador esta colisionando con algo
        if (collisionStay) {
            //Es una zanahoria
            if (collisionName.StartsWith("Carrot") || collisionName.StartsWith("Golden")) {
                //El input es el correcto
                if (   Input.GetButtonUp(currentInputString) 
                    || (currentInputString == "A_Button" && mobileInputs.AButton) 
                    || (currentInputString == "B_Button" && mobileInputs.BButton)
                    || (currentInputString == "X_Button" && mobileInputs.XButton)
                    || (currentInputString == "Y_Button" && mobileInputs.YButton) ) {
                    Debug.Log("Input correcto!");
                    //Empezar a sacar la zanahoria
                    if (currentInputPulls == 0) audioSource.PlayOneShot(audioPull);
                    playerAnimator.SetTrigger("StartPull");
                    carrotGrid.PullCarrot(playerPosX, playerPosY, currentInputPulls++);
                    carrotGrid.PlayerIsPullingGoldenCarrot(collisionName.StartsWith("Golden") ? true : false);
                    //Termino de sacarla
                    if (currentInputPulls == 5) {
                        audioSource.PlayOneShot(audioCoin);
                        //Es normal
                        if (collisionName.StartsWith("Carrot")) {
                            carrotGrid.DestroyCarrot(playerPosX, playerPosY, false);
                            MoneyManager.AddActivityMoney(20f);
                        }
                        //Es de oro
                        else {
                            carrotGrid.DestroyCarrot(playerPosX, playerPosY, true);
                            MoneyManager.AddActivityMoney(50f);
                        }
                    }
                }
                //Verificar que no se haya equivocado de input
                if (checkIncorrectInput()) {
                    //Se equivoco, regresar la zanahoria a su estado inicial
                    carrotGrid.ReturnCarrotToOriginalPosition(playerPosX, playerPosY);
                }
            }
            else {
                //Es una ardilla
                if (collisionName.StartsWith("S")) {
                    //Ahuyentarla
                    if (   Input.GetButtonUp(currentInputString)
                        || (currentInputString == "A_Button" && mobileInputs.AButton)
                        || (currentInputString == "B_Button" && mobileInputs.BButton)
                        || (currentInputString == "X_Button" && mobileInputs.XButton)
                        || (currentInputString == "Y_Button" && mobileInputs.YButton)) {
                        playerAnimator.SetTrigger("Boo");
                        carrotGrid.SquirrelScared();
                        audioSource.PlayOneShot(audioBoo);
                    }   
                }
            }
        }*/
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.name.StartsWith("L")) {
            playerSpriteRenderer.sortingLayerName = other.name.Substring(9) + "_Player";
        }
        else {
            collisionStay = true;
            collisionName = other.name;
            //Si la colisión es un hoyo, apagar el input
            if (collisionName.StartsWith("H")) {
                currentInputObject.SetActive(false);
            }
            //Si la colsión no es un hoyo, poner un input al azar
            if (!collisionName.StartsWith("H")) {
                randomInput = (int)Mathf.Round(Random.Range(0, 4));
                currentInputString = inputsArray[randomInput];
                currentInputSR.sprite = spriteButtonsArray[randomInput];
                currentInputObject.SetActive(true);
            }
            //Si la colisión no es una ardilla, decirle al grid la posición del jugador
            if (!collisionName.StartsWith("S")) {
                playerPosX = int.Parse(collisionName.Substring(7, 1));
                playerPosY = int.Parse(collisionName.Substring(9));
                //carrotGrid.PlayerPosition(playerPosX, playerPosY);
                //Si es una zanahoria, poner su sprite encima del sprite del jugador
                if (collisionName.StartsWith("C") || collisionName.StartsWith("G")) {
                    //carrotGrid.SortLayer("_Carrot_pulled");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!other.name.StartsWith("L")) {
            currentInputObject.SetActive(false);
            collisionStay = false;
            //Reinicia el contador de jaladas
            currentInputPulls = 0;
            //Si es una zanahoria, regresarla a su posición inicial
            if (collisionName.StartsWith("Carrot") || collisionName.StartsWith("Golden")) {
                playerAnimator.SetTrigger("ExitCarrot");
                /*carrotGrid.ReturnCarrotToOriginalPosition(playerPosX, playerPosY);
                carrotGrid.SortLayer("_Carrot");
                carrotGrid.PlayerIsPullingGoldenCarrot(false);*/
            }
        }
    }
    /// <summary>
    /// Detecta si el jugador falló el input
    /// </summary>
    /// <returns>True: incorrecto, False: correcto</returns>
    bool checkIncorrectInput() {
        for (int i = 0; i < 4; i++) {
            if (!currentInputString.Equals(inputsArray[i]) && Input.GetButtonUp(inputsArray[i])) {
                currentInputPulls = 0;
                return true;
            }
        }
        return false;
    }

}
