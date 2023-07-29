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
    }


    void Update()
    {
        transform.Rotate(0, selfRotateSpeed * Time.deltaTime, 0); //����

        rotateAngle += Time.deltaTime * orbitRotateSpeed;
        if(rotateAngle < 360)
        {
            float xPos = rotateRadius * Mathf.Sin(rotateAngle);
            float zPos = rotateRadius * Mathf.Cos(rotateAngle);

            //if (gameObject.name == "Moon")

            transform.localPosition = new Vector3(xPos, 0, zPos);
        }
        
    }
}
