using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace GranularFlow.Main
{
    public class GranularFlow : MonoBehaviour, IGranularFlow
    {
        [SerializeField] private int _num;
        [SerializeField] private float3 _gravity;
        [SerializeField] private float _airDensity;
        [SerializeField] private float _springConstant;
        [SerializeField] private float _viscosity;
        [SerializeField] private float _timeStep;

        private Granule[] _granular;
        private float3[] _forces;
        private Matrix4x4[] _matrices;

        public Matrix4x4[] Matrices { get => _matrices; }

        private void Awake()
        {
            _granular = new Granule[_num];
            _forces = new float3[_num];
            _matrices = new Matrix4x4[_num];

            for (int i = 0; i < _num; i++)
            {
                float3 center = (float3)Camera.main.gameObject.transform.position + new float3(0,
                    Camera.main.orthographicSize - 1f,
                    (Camera.main.farClipPlane - Camera.main.nearClipPlane) / 2f);
                float3 size = new float3(1.7f, 0.2f, 0.01f);
                float3 position = center + new float3(
                    UnityEngine.Random.Range(-size.x / 2, size.x / 2),
                    UnityEngine.Random.Range(-size.y / 2, size.y / 2),
                    UnityEngine.Random.Range(-size.z / 2, size.z / 2));

                _granular[i].Radius = 0.05f;
                _granular[i].Density = 20f;
                _granular[i].Position = position;
                _matrices[i].SetTRS(_granular[i].Position, Quaternion.identity, (float3)_granular[i].Radius * 2);
            }
        }

        private void Update()
        {
            UpdateForce();
            UpdatePosition();
        }

        private void UpdateForce()
        {
            for (int i = 0; i < _num; i++)
            {
                float volume = 4f / 3f * PI * _granular[i].Radius * _granular[i].Radius * _granular[i].Radius;
                _forces[i] = _gravity * volume * (_granular[i].Density - _airDensity);
                for (int j = 0; j < _num; j++)
                {
                    if (i == j) continue;
                    float3 vec = _granular[j].Position - _granular[i].Position;
                    float3 len = length(vec);
                    _forces[i] += vec / len * -_springConstant * max(_granular[i].Radius + _granular[j].Radius - len, 0);
                }
            }
        }

        private void UpdatePosition()
        {
            for (int i = 0; i < _num; i++)
            {
                for (int j = 0; j < _num; j++)
                {
                    if (i == j) continue;
                    float3 vec = _granular[i].Position - _granular[j].Position;
                    float len = length(vec);
                    float3x3 I = new float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
                    float3x3 rr = DirectProduct(vec, vec);

                    float3x3 J = (I + rr / (len * len) + 2f / 3f * pow(_granular[j].Radius / len, 2) * (I - 3 * rr / (len * len))) / len;
                    float3 u = mul(J, _forces[j]) * (6 * PI * _viscosity * _granular[i].Radius) / (8 * PI * _viscosity);
                    _forces[i] += u;
                }

                _granular[i].Velocity = _forces[i] / (6 * PI * _viscosity * _granular[i].Radius);
                _granular[i].Position += _granular[i].Velocity * _timeStep;
                _matrices[i].SetTRS(_granular[i].Position, Quaternion.identity, (float3)_granular[i].Radius * 2);
            }
        }

        float3x3 DirectProduct(float3 a, float3 b)
        {
            return float3x3(a.x * b.x, a.x * b.y, a.x * b.z, a.y * b.x, a.y * b.y, a.y * b.z, a.z * b.x, a.z * b.y, a.z * b.z);
        }

        private struct Granule
        {
            public float Radius;
            public float Density;
            public float3 Velocity;
            public float3 Position;
        }
    }
}