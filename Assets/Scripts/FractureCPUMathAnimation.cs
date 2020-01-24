using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureCPUMathAnimation : MonoBehaviour
{
    [SerializeField]
    private Transform[] fracturePieces;

    private Vector3[] initialLocalPos;
    private Quaternion[] initialLocalRot;

    [SerializeField]
    [Range (0, 1)]
    private float _FractureAmount;
    [SerializeField]
    private float _TranslateAmount;
    [SerializeField]
    private Vector3 _RotateAmountEuler;


    private void Start()
    {
        initialLocalPos = new Vector3[fracturePieces.Length];
        initialLocalRot = new Quaternion[fracturePieces.Length];

        for (int i = 0; i < fracturePieces.Length; i++)
        {
            initialLocalPos[i] = fracturePieces[i].localPosition;
            initialLocalRot[i] = fracturePieces[i].localRotation;
        }
    }

    private void Update()
    {
        for (int i = 0; i < fracturePieces.Length; i++)
        {
            Vector3 directionFromCenter = (initialLocalPos[i]).normalized;

            fracturePieces[i].localPosition = initialLocalPos[i] + directionFromCenter * _FractureAmount * _TranslateAmount;

            float scale = 1f - _FractureAmount;
            fracturePieces[i].localScale = new Vector3(scale, scale, scale);

            fracturePieces[i].localRotation = initialLocalRot[i] * Quaternion.Euler(_RotateAmountEuler * _FractureAmount);
        }
    }
}
