using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingArea : MonoBehaviour
{
    private Vector3 _mousePosition;
    private bool _isOccupied = false;
    public int towerType;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(_mousePosition.x, _mousePosition.y, 0);

        SelectArea(towerType);
	}

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag != "Tower")
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, (float)93 / 255, (float)93 / 255, 0.5f);
        _isOccupied = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color((float)64 / 255, (float)170 / 255, (float)231 / 255, 0.5f);
        _isOccupied = false;
    }

    public void SelectArea(int type)
    {
        GameObject character = GameObject.FindGameObjectWithTag("Character");
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!_isOccupied)
            {
                character.GetComponent<CharacterCtrl>().buildPosition = new Vector3(transform.position.x - 3f, transform.position.y, 0);
                character.GetComponent<CharacterCtrl>().cState = CharacterCtrl.CharacterState.BuildMove;
                Destroy(gameObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            character.GetComponent<CharacterCtrl>().EditStone(30);
            character.GetComponent<CharacterCtrl>().EditWood(20);
            character.GetComponent<CharacterCtrl>().cState = CharacterCtrl.CharacterState.Idle;
            character.SendMessage("UpdateResourcesText");
            Destroy(gameObject);
        }
    }
}
