using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlanetInteraction : MonoBehaviour
{ 
    public GameObject player;
    public Vector3 offset; //�༺�� ī�޶� ���� ����

    public GameObject panel;

    private Button btn1;
    private Button btn2;

    private bool moveCam = false;

    private void Start()
    {
        btn1 = panel.transform.GetChild(1).GetComponent<Button>();
        btn2 = panel.transform.GetChild(2).GetComponent<Button>();

        btn1.onClick.AddListener(changePlanetScene);
        btn2.onClick.AddListener(returnPosition);
    }

    public void changePlanetScene()
    {
        if (moveCam) SceneManager.LoadScene(this.name);
    }

    public void returnPosition()
    {
        if (moveCam) player.transform.position = new Vector3(0, 4, -27);
        moveCam = false;
        panel.SetActive(false);
    }

    public void FixCameraToPlanet()
    {
        moveCam = true;

        //UIȰ��ȭ �� Ŭ���� �༺ �̸����� ����
        panel.SetActive(true);
        panel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.name.Substring(1);
       
    }

    private void Update()
    {
        //Ŭ���� �༺���� �̵�
        if (moveCam)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, transform.position + offset, 0.1f);
            panel.transform.position = player.transform.position + new Vector3(0, 5, 10);
        }
       
    }
}
