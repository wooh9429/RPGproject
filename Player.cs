using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    JoyStickControl joyStick;

    public Animator anim;

    Rigidbody rigid;
    Quaternion dodgeVec;

    bool wDown;
    bool jDown;
    bool isJump;
    bool isDodge;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool isSwap;

    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    int equipWeaponIndex = -1;
    public float jumpPower;

    GameObject nearObj;
    GameObject equipWeapon;

    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;

    void Awake()
    {
        joyStick = GameObject.Find("JoyStick").GetComponentInChildren<JoyStickControl>();
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
        Move();
        Jump();
        Dodge();
        Swap();
        Interaction();
    }

    void Move()
    {
        anim.SetBool("isWalk", joyStick.inputVector != Vector3.zero);
        anim.SetBool("isRun", wDown);

        if(isSwap)
        {
            joyStick.moveSpeed = 0;
        }
        else
        {
            joyStick.moveSpeed = 3.5f;
        }

        if(isDodge)
        {
            joyStick.player.transform.rotation = dodgeVec; // Dodge하는 순간 방향 고정
        }
            
        
    }

    void GetInput()
    {
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");

        //if(hasWeapons[0])
            sDown1 = Input.GetButtonDown("Swap1");

        //if(hasWeapons[1])
            sDown2 = Input.GetButtonDown("Swap2");

        //if (hasWeapons[2])
            sDown3 = Input.GetButtonDown("Swap3");
    }

    void Jump()
    {
        if(jDown && joyStick.inputVector == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDown && joyStick.inputVector != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = joyStick.player.transform.rotation;  // Dodge하는 순간 방향 고정
            joyStick.moveSpeed *= 2;    // Dodge시 스피드 2배
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        joyStick.moveSpeed *= 0.5f; // Dodge 후 스피드 원상복귀
        isDodge = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;

        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;


        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
            {
                equipWeapon.SetActive(false);
            }

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if(iDown && nearObj != null && !isJump && !isDodge)
        {
            if(nearObj.tag == "Weapon")
            {
                Item item = nearObj.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObj);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;

                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;

                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;

                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }

            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObj = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObj = null;
        }
    }
}
