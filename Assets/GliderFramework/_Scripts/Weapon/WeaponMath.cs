using UnityEngine;

namespace WeaponMath
{
    public static class Math
    {
        public static float VectorToRotation(Vector2 vector) => - Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;

        public static Vector2 ProjectVectorOntoVector(Vector2 projectingVector, Vector2 ontoVector)
        {
            return ontoVector.magnitude > 0 ? Vector2.Dot(ontoVector, projectingVector) * ontoVector / ontoVector.sqrMagnitude : Vector2.zero;
        }

        public static Vector2 GetDirectionVectorToTarget(Vector2 sourcePosition, Vector2 targetPosition)
        {
            return (targetPosition - sourcePosition).normalized;
        }

        public static Vector2 ApplyAccuracyCoefficientToVector(Vector2 vector, float accuracyCoefficient)
        {
            return new Vector2(vector.y * Random.Range(-1 + accuracyCoefficient, 1 - accuracyCoefficient),
             vector.x * Random.Range(-1 + accuracyCoefficient, 1 - accuracyCoefficient)) / 2f;
        }
    }
}
