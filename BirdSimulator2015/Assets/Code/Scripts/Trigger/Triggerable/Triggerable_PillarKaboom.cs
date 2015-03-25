﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerable_PillarKaboom : BaseTriggerable<BaseTriggerable>
{
    public GameObject Cube;
    public GameObject[] Pillars;
    public GameObject Kabooms;

    private bool triggered = false;

    void Start()
    {
    }

    void Update()
    {
    }

    public override void Trigger(Collider c, GameObject g)
    {
        if (triggered)
            return;

        triggered = true;

        Camera.main.GetComponent<BirdSimulator2015.Code.Scripts.Cam.TPRadialCamera>().TargetRadius = 200f;
        //Camera.main.GetComponent<TPChaseCamera>().TargetRadius = 200f;
        int children = Kabooms.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            Kabooms.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }

        this.GetComponent<AudioSource>().Play();

        StartCoroutine(coRumble());
        StartCoroutine(coFadeCube());
        StartCoroutine(coMoveShit());
        StartCoroutine(coSaturate());
        StartCoroutine(coFadeOut());

        GameObject.FindWithTag(Registry.Tag.AudioController).GetComponent<AudioController>().PlayTrack(AudioController.BGMTrack.Ending);

        base.Trigger(c, g);
    }

    IEnumerator coRumble()
    {
        yield return new WaitForSeconds(0f);
        GameController.Player.GetComponent<PlayerRumble>().BumbleRumble(0.1f, 0f, 1f);

        yield return new WaitForSeconds(0.1f);
        GameController.Player.GetComponent<PlayerRumble>().BumbleRumble(0.1f, 1f, 0f);

        yield return new WaitForSeconds(0.5f);
        GameController.Player.GetComponent<PlayerRumble>().BumbleRumble(1f, 0.5f, 0.5f);

        yield return new WaitForSeconds(1f);
        GameController.Player.GetComponent<PlayerRumble>().BumbleRumble(1f, 0.3f, 0.3f);

        yield return new WaitForSeconds(1f);
        GameController.Player.GetComponent<PlayerRumble>().BumbleRumble(1f, 0.1f, 0.1f);

        while (Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation < 1.5f)
        {
            Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    IEnumerator coFadeCube()
    {
        Renderer renderer = Cube.GetComponentInChildren<Renderer>();
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * 0.35f;
            renderer.material.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
            yield return null;
        }

        yield return null;
    }

    IEnumerator coMoveShit()
    {
        float t = 10f;

        Vector3[] directions = new Vector3[Pillars.Length];

        for (int i = 0; i < directions.Length; i++)
        {
            float forward = Random.Range(-1f, 1f);
            float right = Random.Range(-1f, 1f);
            float up = Random.Range(-1f, 1f);
            directions[i] = (Vector3.forward * forward) + (Vector3.right * right) + (Vector3.up * up);
        }


        while (t > 0)
        {
            t -= Time.deltaTime;

            int i = 0;
            foreach (GameObject g in Pillars)
            {
                g.transform.Translate(directions[i] * Time.deltaTime * 15f);
                i++;
            }
            if (t < 9)
            {
                Time.timeScale = 0.3f;
                GameController.Player.GetComponent<PlayerState>().SpeedyModeSpeed = 35f;
            }

            yield return null;
        }
        

        yield return null;
    }

    IEnumerator coSaturate()
    {
        yield return new WaitForSeconds(5f);
        while (Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation < 1.5f)
        {
            Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ColorCorrectionCurves>().saturation += Time.deltaTime * 0.5f;
            yield return null;
        }
        yield return null;
    }

    IEnumerator coFadeOut()
    {
        yield return new WaitForSeconds(5f);
        GameController.EndingPopup.FadeIn();
        yield return null;
    }
}
