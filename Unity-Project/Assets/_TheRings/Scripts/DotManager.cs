using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SgLib;

struct Line
{
    public Vector2 Position { get; set; }

    public float Angle { get; set; }

    public Color Color { get; set; }
}

public class DotManager : MonoBehaviour
{

    public GameManager gameManager;
    public UIManager uIManager;
    public CameraController cameraControler;
    public GameObject[] dots;
    public AnimationClip bigRingScale;
    public AnimationClip comboText;
    [HideInInspector]
    public int dotIndex;
    [HideInInspector]
    public bool finishCheckAfterDestroy;

    private List<GameObject> listDestroyRing = new List<GameObject>();
    private List<Line> listLine = new List<Line>();
    private int previousCombo;
    // Use this for initialization

    void Awake()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].GetComponent<DotController>().dotIndex = i;
            dots[i].GetComponent<DotController>().CheckRing();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (gameManager.finishDrop) //Finish drop -> check all ring at that dot, caculate combo and destroy rings
        {
            gameManager.finishDrop = false;
            dots[dotIndex].GetComponent<DotController>().CheckRing();
            CaculateCombo();
            StartCoroutine(DestroyRings());                   
        }
    }

    //Find all destroyed ring and destroy it, caculate score base on number of destroyed ring
    IEnumerator DestroyRings()
    {       
        //Find all destroyed ring add ad to the list
        for (int i = 0; i < dots.Length; i++)
        {
            for (int j = 0; j < dots[i].transform.childCount; j++)
            {
                if (dots[i].transform.GetChild(j).GetComponent<RingController>().destroyed)
                {
                    listDestroyRing.Add(dots[i].transform.GetChild(j).gameObject);
                }
            }
        }

        yield return null;

        //Caculate score and change ring time
        int scoreAdded = gameManager.combo * listDestroyRing.Count;
        if (gameManager.combo > 1)
        {
            cameraControler.ShakeCamera();
            uIManager.ShowComboText(comboText.length);
            Vector2 comboExplodePosition = Camera.main.ScreenToWorldPoint(uIManager.txtCombo.transform.position);
            ParticleSystem comboExplode = Instantiate(gameManager.comboExplode, comboExplodePosition, Quaternion.identity) as ParticleSystem;
            comboExplode.Play();
            Destroy(comboExplode.gameObject, 1f);
            StartCoroutine(PlayComboSound(gameManager.combo));
        }

        if (scoreAdded > 0)
        {
            ScoreManager.Instance.AddScore(scoreAdded);
            SoundManager.Instance.PlaySound(SoundManager.Instance.lineDestroy);
        }

        if (listDestroyRing.Count >= 5)
        {
            float changeRingTimeAdded = Mathf.Ceil(listDestroyRing.Count / 5f);
            CoinManager.Instance.AddCoins((int)changeRingTimeAdded);
        }

        CreateAndDestroyLine();

        //Destroy rings
        for (int i = 0; i < listDestroyRing.Count; i++)
        {
            ParticleSystem particle = Instantiate(gameManager.ringExplode, listDestroyRing[i].transform.position, Quaternion.identity) as ParticleSystem;
            #if UNITY_5_5_OR_NEWER
            var tmp = particle.main;
            tmp.startColor = gameManager.ringColors[listDestroyRing[i].GetComponent<RingController>().colorIndex];
            #else
            particle.startColor = gameManager.ringColors[listDestroyRing[i].GetComponent<RingController>().colorIndex];
            #endif
            particle.Play();
            Destroy(particle.gameObject, 2f);
            listDestroyRing[i].GetComponent<Animator>().enabled = true;
            Destroy(listDestroyRing[i], bigRingScale.length);
        }

        yield return (listDestroyRing.Count > 0) ? (new WaitForSeconds(bigRingScale.length)) : null;
        //float waitTime = (listDestroyRing.Count > 0) ? bigRingScale.length : 0.02f;
        listDestroyRing.Clear();

        //Check all to update bool variable of ring
        //yield return new WaitForSeconds(waitTime);
        CheckAllDot();
    }

    IEnumerator PlayComboSound(int combo)
    {
        for (int i = 0; i < gameManager.combo; i++)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.lineDestroy);
            yield return new WaitForSeconds(0.3f);
        }
    }

    //Check all dot
    void CheckAllDot()
    {
        for (int i = 0; i < dots.Length; i++)
        {          
            dots[i].GetComponent<DotController>().CheckRing();
        }
        finishCheckAfterDestroy = true;
    }


    //Caculate combo base on dot sequence
    void CaculateCombo()
    {
        if (dotIndex == 0) //Check line: 0_1_2, 0_4_8. 0_5_6
        {
            previousCombo = gameManager.combo;

            gameManager.combo += (ComboLine(0, 1, 2) + ComboLine(0, 4, 8) + ComboLine(0, 5, 6) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
        else if (dotIndex == 1) //Check line: 0_1_2, 1_4_7
        {
            previousCombo = gameManager.combo;

            gameManager.combo += (ComboLine(0, 1, 2) + ComboLine(1, 4, 7) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
        else if (dotIndex == 2) //Check line: 2_1_0, 2_3_8, 2_4_6
        {
            previousCombo = gameManager.combo;

            gameManager.combo += (ComboLine(0, 1, 2) + ComboLine(2, 3, 8) + ComboLine(2, 4, 6) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
        else if (dotIndex == 3) //Check line 2_3_8, 3_4_5
        {
            previousCombo = gameManager.combo;
            gameManager.combo += (ComboLine(2, 3, 8) + ComboLine(3, 4, 5) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
        else if (dotIndex == 4) //Check line: 1_4_7, 0_4_8, 2_4_6, 3_4_5
        {
            previousCombo = gameManager.combo;
            gameManager.combo += (ComboLine(1, 4, 7) + ComboLine(0, 4, 8) + ComboLine(2, 4, 6) + ComboLine(3, 4, 5) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
        else if (dotIndex == 5) //Check line: 0_5_6, 3_4_5
        {
            previousCombo = gameManager.combo;
            gameManager.combo += (ComboLine(0, 5, 6) + ComboLine(3, 4, 5) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
        else if (dotIndex == 6) //Check line: 0_5_6, 2_4_6, 6_7_8
        {
            previousCombo = gameManager.combo;
            gameManager.combo += (ComboLine(0, 5, 6) + ComboLine(6, 7, 8) + ComboLine(2, 4, 6) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
        else if (dotIndex == 7) //Check line: 6_7_8, 1_4_7
        {
            previousCombo = gameManager.combo;
            gameManager.combo += (ComboLine(6, 7, 8) + ComboLine(1, 4, 7) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
        else //Check line: 2_3_8, 6_7_8, 0_4_8
        {
            previousCombo = gameManager.combo;
            gameManager.combo += (ComboLine(2, 3, 8) + ComboLine(6, 7, 8) + ComboLine(0, 4, 8) + ComboDotIndex(dotIndex));

            if (gameManager.combo == previousCombo)
            {
                gameManager.combo = 0;
            }
        }
    }

    int ComboLine(int firstDot, int secondDot, int thirdDot)
    {
        int combo = 0;
        bool hasDestroy = false;
        //Check first dot
        for (int i = 0; i < dots[firstDot].transform.childCount; i++)
        {
            int colorIndexOfChild = dots[firstDot].transform.GetChild(i).GetComponent<RingController>().colorIndex;
            //Check second dot
            for (int j = 0; j < dots[secondDot].transform.childCount; j++)
            {
                RingController secondDotRingControlller = dots[secondDot].transform.GetChild(j).GetComponent<RingController>();
                if (secondDotRingControlller.colorIndex == colorIndexOfChild)
                {
                    //Check third dot
                    for (int k = 0; k < dots[thirdDot].transform.childCount; k++)
                    {
                        RingController thirdDotRingController = dots[thirdDot].transform.GetChild(k).GetComponent<RingController>();
                        if (thirdDotRingController.colorIndex == colorIndexOfChild)
                        {
                            dots[thirdDot].transform.GetChild(k).GetComponent<RingController>().destroyed = true;
                            dots[secondDot].transform.GetChild(j).GetComponent<RingController>().destroyed = true;
                            dots[firstDot].transform.GetChild(i).GetComponent<RingController>().destroyed = true;
                            AddForListLine(firstDot, secondDot, colorIndexOfChild);
                            hasDestroy = true;                            
                        }
                    }
                }
            }
        }
        if (hasDestroy)
        {
            combo = 1;
        }
        return combo;
    }

    int ComboDotIndex(int dotIndex)
    {
        bool hasDestroy = false;
        int combo = 0;

        if (dots[dotIndex].GetComponent<DotController>().isSameColor)
        {
            hasDestroy = true;
            for (int i = 0; i < dots[dotIndex].transform.childCount; i++)
            {
                dots[dotIndex].transform.GetChild(i).GetComponent<RingController>().destroyed = true;
            }
        }

        if (hasDestroy)
        {
            combo = 1;
        }

        return combo;
    }


    void AddForListLine(int firstDot, int secondDot, int colorIndex)
    {
        Line newLine = new Line();
        newLine.Position = dots[secondDot].transform.position;
        newLine.Color = gameManager.ringColors[colorIndex];
        if (secondDot == 5 || secondDot == 3)
        {            
            newLine.Angle = 0;
        }
        else if (secondDot == 1 || secondDot == 7)
        {          
            newLine.Angle = 90;
        }
        else if (secondDot == 4)
        {
            if (firstDot == 1)
            {             
                newLine.Angle = 0;
            }
            else if (firstDot == 3)
            {
                newLine.Angle = 90;
            }
            else if (firstDot == 2)
            {
                newLine.Angle = 135;
            }
            else
            {
                newLine.Angle = 45;
            }
        }

        if (!listLine.Contains(newLine))
        {
            listLine.Add(newLine);
        }
    }

    void CreateAndDestroyLine()
    {
        for (int i = 0; i < listLine.Count; i++)
        {
            GameObject line = Instantiate(gameManager.line, listLine[i].Position, Quaternion.Euler(0, 0, listLine[i].Angle)) as GameObject;
            line.GetComponent<SpriteRenderer>().color = listLine[i].Color;
            line.GetComponent<Animator>().enabled = true;
            Destroy(line, bigRingScale.length);
        }

        listLine.Clear();
    }
}