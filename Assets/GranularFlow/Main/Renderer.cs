using System;
using System.Collections;
using System.Collections.Generic;
using GranularFlow.Tools;
using UnityEngine;

namespace GranularFlow.Main
{
    public class Renderer : MonoBehaviour
    {
        [SerializeField] private Mesh _mesh;
        [SerializeField] private Material _material;

        private IGranularFlow _granularFlow;

        private void Awake()
        {
            _granularFlow = InterfaceUtility.FindObjectOfInterface<IGranularFlow>();
        }

        private void Update()
        {
            Graphics.DrawMeshInstanced(_mesh, 0, _material, _granularFlow.Matrices);
        }
    }
}