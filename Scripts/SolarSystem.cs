using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SolarSystem : MonoBehaviour
{
    public DataManager dataManager;

    public GameObject[] celestialObjs;

    public Rigidbody[] planets;
    public Rigidbody[] meteors;
    public Rigidbody[] cormets;
    public Transform cameraTrans;

    public bool[] controls;

    bool[] isRegist = new bool[11];

    int[] revDistance = { 0, 4, 7, 10, 15, 40, 50, 75, 100 };
    float[] revDegree = new float[9];
    float[] revolutionSpeed = { 0, 1.5f, 1.16f, 1, 0.8f, 0.43f, 0.32f, 0.2f, 0.18f };
    float[] rotationSpeed = { 0.04f, 0.01f, -0.004f, 1, 1, 2.5f, 2.3f, 1.4f, 1.5f };

    float meteorDistance = 30;
    float[] meteorDegree = new float[22];

    float cometDistance = 25;
    float[] cometDegree = new float[8];
    


    Vector3 dir;

    [SerializeField]
    int totalSpeed;

    int cameraSpeed;
    bool isControl;
    float rH, rV = 0;

    private void Awake()
    {
        dir = cameraTrans.position;

        for(int i = 0; i < 11; i++)
        {
            isRegist[i] = dataManager.playerData.isRegist[i];
        }
    }
    private void Start()
    {
        for(int i =0; i < celestialObjs.Length; i++)
        {
            celestialObjs[i].SetActive(isRegist[i]);
        }

        cameraTrans.position = new Vector3(-10, 7, -15);
        rH = 48;
        rV = 20;

        float mDegreeSetter = 0;
        float cDegreeSetter = 0;

        for (int i = 0; i < 22; i++)
        {
            meteorDegree[i] = mDegreeSetter;
            mDegreeSetter += 0.3f;
        }

        for (int i = 0; i < 8; i++)
        {
            cometDegree[i] = cDegreeSetter;
            cDegreeSetter += 0.9f;
        }
    }

    private void Update()
    {
        CameraMove();
        Revolution();
        Rotation();
    }

    /////////아래는 카메라 조작////////////
    void CameraMove()
    {
        if (!isControl) { cameraSpeed = 0; }
        else
        {
            if (controls[0]) { dir = Vector3.right; cameraSpeed = 10; }
            if (controls[1]) { dir = Vector3.left; cameraSpeed = 10; }
            if (controls[2]) { dir = Vector3.forward; cameraSpeed = 10; }
            if (controls[3]) { dir = Vector3.back; cameraSpeed = 10; }
            if (controls[4]) { rH += 1; }
            if (controls[5]) { rH -= 1; }
            if (controls[6]) { rV -= 1; }
            if (controls[7]) { rV += 1; }

            cameraTrans.rotation = Quaternion.Euler(rV, rH, 0);

            cameraTrans.Translate(dir * cameraSpeed * Time.deltaTime);
        }
    }

    public void JoyPanel(int type)
    {
        for (int i = 0; i < 8; i++)
        {
            controls[i] = i == type;
        }
    }

    public void JoyDown()
    {
        isControl = true;
    }
    public void JoyUp()
    {
        isControl = false;
    }
    ////////////////여기까지////////////////////

    /////////아래는 공전 및 자전 루틴////////////
    void Revolution()
    {
        for(int i = 0; i < 9; i++)
        {
            if (i == 7)
            {
                    planets[i].transform.rotation = Quaternion.Euler(
                    totalSpeed * 10 * rotationSpeed[i], 
                    Quaternion.LookRotation(
                    SetCoordinate(
                    CoordinateX(revDistance[i], revDegree[i]),
                    CoordinateZ(revDistance[i], revDegree[i])).normalized).eulerAngles.y
                    , 0);

                rotationSpeed[i] += Time.deltaTime;
            }
            else
                planets[i].transform.Rotate(0, totalSpeed * 10 * rotationSpeed[i] * Time.deltaTime, 0);
        }

        for (int i = 0; i < 22; i++)
        {
            meteors[i].transform.rotation =
                 Quaternion.LookRotation(
                    SetCoordinate(
                    CoordinateX(meteorDistance, meteorDegree[i]),
                    CoordinateZ(meteorDistance, meteorDegree[i])).normalized);
        }

        for(int i = 0; i < 8; i++)
        {
            cormets[i].transform.rotation =
                 Quaternion.LookRotation(
                    SetCoordinate(
                    CoordinateX(cometDistance, cometDegree[i]),
                    CoordinateZ(cometDistance, cometDegree[i])).normalized);
        }
    }

    void Rotation()
    {
        for (int i = 0; i < 9; i++)
        {
            planets[i].transform.position =
                SetCoordinate(
                    CoordinateX(revDistance[i], revDegree[i]), 
                    CoordinateZ(revDistance[i], revDegree[i]));

            revDegree[i] -= revolutionSpeed[i] * Time.deltaTime * 0.1f;
        }

        for(int i = 0; i < 22; i++)
        {
            meteors[i].transform.position =
                 SetCoordinate(
                    CoordinateX(meteorDistance, meteorDegree[i]), 
                    CoordinateZ(meteorDistance, meteorDegree[i]));

            meteorDegree[i] -= 2.3f * Time.deltaTime * 0.1f;
        }

        for (int i = 0; i < 8; i++)
        {
            cormets[i].transform.position =
                 SetCoordinate(
                    CoordinateX(cometDistance, cometDegree[i]),
                    CoordinateZ(cometDistance, cometDegree[i]));

            cometDegree[i] -= 5f * Time.deltaTime * 0.1f;
        }
    }

    Vector3 SetCoordinate(float X, float Z)
    {
        return new Vector3(X, 0, Z);
    }
    float CoordinateX(float distance, float degree)
    {
        return distance * Mathf.Cos(degree);
    }

    float CoordinateZ(float distance, float degree)
    {
        return distance * Mathf.Sin(degree);
    }
    ///////////////////여기까지////////////////////


    public void SceneToMain()
    {
        dataManager.Save();
        SceneManager.LoadScene(2);
    }
}
