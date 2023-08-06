using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinPlanet : MonoBehaviour
{
    public float selfRotateSpeed; //���� ���ӵ�

    public float orbitRotateSpeed; //���� ���ӵ�
    private float rotateRadius; //ȸ�� ������
    private float rotateAngle; //ȸ�� ����


    void Start()
    {
        rotateRadius = transform.localPosition.x; //����(�¾�)���κ����� �Ÿ�

        //���� ��� �����κ����� �Ÿ�
        if (this.name == "10Moon") 
            rotateRadius = transform.localPosition.x - GameObject.Find("3Earth").transform.localPosition.x;
    }


    void Update()
    {
        transform.Rotate(0, selfRotateSpeed * Time.deltaTime, 0); //����

        rotateAngle += Time.deltaTime * orbitRotateSpeed; // ����
        if(rotateAngle < 360)
        {
            float xPos;
            float zPos;
            if (gameObject.name != "10Moon")
            {
                xPos = rotateRadius * Mathf.Sin(rotateAngle);
                zPos = rotateRadius * Mathf.Cos(rotateAngle);
            }
            else //���� ��� ���� ���� ����(�¾�)�� ���� ������ �Ÿ��� ������
            {
                xPos = rotateRadius * Mathf.Sin(rotateAngle) + GameObject.Find("3Earth").transform.localPosition.x;
                zPos = rotateRadius * Mathf.Cos(rotateAngle) + GameObject.Find("3Earth").transform.localPosition.z;
            }

            transform.localPosition = new Vector3(xPos, 0, zPos);
        }
        
    }
}
