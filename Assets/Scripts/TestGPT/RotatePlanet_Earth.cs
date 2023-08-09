using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlanet_Earth : MonoBehaviour
{
    //private float degreePerSecond = 20f;

    public Transform hand;  //��Ʈ�ѷ��� Transform
    public Transform planet;    //�༺�� Transform

    private bool isRotating = false;
    private Vector3 initialHandPosition;
    private Quaternion initialRotation;
    private float rotationSpeed = 0.6f;   //������ �ӵ�

    void Update()
    {
        //transform.Rotate(Vector3.up * Time.deltaTime * degreePerSecond);  //ȥ�ڼ� �����ϰ� �ϴ� �ڵ�
        planet.position = new Vector3(-10.71f, -11.91f, 16.03f);   //�༺ ��ǥ ����

        if (isRotating)
        {
            RotateObject();
        }
    }

    public void StartRotating()
    {
        isRotating = true;
        initialHandPosition = hand.position;
        initialRotation = transform.rotation;
    }

    public void StopRotating()
    {
        isRotating = false;
    }

    private void RotateObject()
    {
        Vector3 currentHandPosition = hand.position;
        Vector3 direction = (currentHandPosition - initialHandPosition).normalized;

        // ���� ������ ���⿡ ���� ȸ�� ���� ���
        float horizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float verticalAngle = Mathf.Asin(direction.y) * Mathf.Rad2Deg;

        // ȸ�� ������ �����Ͽ� ��ü�� ȸ��
        transform.rotation = initialRotation * Quaternion.Euler(verticalAngle * rotationSpeed, -horizontalAngle * rotationSpeed, 0f);
    }
}
