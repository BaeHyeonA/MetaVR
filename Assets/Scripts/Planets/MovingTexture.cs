using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTexture : MonoBehaviour
{

    // ������ ���ϴ� �༺�� ������
    // 1. �༺�� �����ϴ� ��ó�� ���̵��� Plane�� �ؽ�ó �����̱�
    // 2. ���ּ�(Landing Plane)�� �ڿ������� ���ִ� �� ó�� ���̱� ���� �༺ Plane�� ���Ʒ��� �����̱�

    private Renderer m_renderer;

    public float movingSpeed = 0.02f;
    public float offset = 0.1f;

    public float shakingSpeed = 1.5f;
    private float lerpTime = 0;
    private float yPos = 0;

    Vector2 offsetVec = Vector2.zero;
    
    void Start()
    {
        m_renderer = GetComponent<Renderer>();

        //yPos = transform.position.y;
    }


    void Update()
    {
        offset = movingSpeed * Time.time;
        m_renderer.material.mainTextureOffset = new Vector2(0, -offset);


        lerpTime += Time.deltaTime * shakingSpeed;
        yPos = Mathf.Sin(lerpTime) * 0.01f;
        transform.position += new Vector3(0, yPos, 0);
    }
}
