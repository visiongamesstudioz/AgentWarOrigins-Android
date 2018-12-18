using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterScroller : MonoBehaviour
{

    private Player[] m_Players;
    public GameObject leftButton;
    public GameObject rightButton;
    public float changePosX;
    public static int index = 0;
    private int currentIndex;
    private ShowOutfits m_Showoutfits;
    private ShowCharacters _mCharactersDetails;
    void Awake()
    {
        m_Showoutfits = GetComponent<ShowOutfits>();
        
        _mCharactersDetails = GetComponent<ShowCharacters>();
    }

    void Start()
    {
        m_Players = DataManager.Instance.Players;

        if (index <= 0)
        {
            index = 0;
            leftButton.SetActive(false);

        }
    }

    public void OnLeftClickedsScroller()
    {
        if (index <= 0)
        {
            index = 0;
            leftButton.SetActive(false);

        }
        else {
            index -= 1;

            leftButton.SetActive(true);
            rightButton.SetActive(true);
            Camera.main.GetComponent<CameraMovement>().MoveCamera(-changePosX);
            //float targetPosX = transform.position.x + changePosX;
            //Vector3 targetPosition = new Vector3(targetPosX, transform.position.y, transform.position.z);
            //StartCoroutine(ChangeGoTargetPosition(targetPosition));
         //  DisableOtherObjects();
            EnableCurrentObject();
            if (index <= 0)
            {
                index = 0;
                leftButton.SetActive(false);

            }
        }
        _mCharactersDetails.DisplayCurrentCharacterDetails();
        m_Showoutfits.ShowPlayerOutfits();
    }
    public void OnRightClickedScroller()
    {

        if (index == m_Players.Length-1)
        {

            rightButton.SetActive(false);

        }
        else {
            index += 1;
            leftButton.SetActive(true);
            rightButton.SetActive(true);
            Camera.main.GetComponent<CameraMovement>().MoveCamera(changePosX);
            if (index == m_Players.Length - 1)
            {

                rightButton.SetActive(false);

            }
            // DisableOtherObjects();
            EnableCurrentObject();

        }
        _mCharactersDetails.DisplayCurrentCharacterDetails();

        m_Showoutfits.ShowPlayerOutfits();

    }

    void DisableOtherObjects() {

        for (int i = 0; i < m_Players.Length; i++) {

            if (i == index) {
                continue;
            }
            m_Players[i].gameObject.SetActive(false);

        }

    }
    void EnableCurrentObject() {

        for (int i = 0; i < m_Players.Length; i++)
        {

            if (i == index)
            {
                m_Players[i].gameObject.SetActive(true);
                
            }

        }


    }
    public int GetCurrentPositionSelected() {

        return index;

    }

    public void ResetScrollerPosition()
    {
        index = 0;
        //show changed outfit
        if (index <= 0)
        {
            index = 0;
            leftButton.SetActive(false);

        }
        m_Players = DataManager.Instance.Players;

        if (m_Players.Length > 1)
        {
            rightButton.SetActive(true);
        }
        else
        {
            rightButton.SetActive(false);
        }

        _mCharactersDetails.DisplayCurrentCharacterDetails();
        m_Showoutfits.ShowPlayerOutfits();

    }

    private void OnDisable()
    {
        index = 0;
    }

}
