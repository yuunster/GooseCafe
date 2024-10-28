using UnityEngine;

public static class Extensions
{
    private static LayerMask layerMask = LayerMask.GetMask("Default");
    public static bool Raycast(this Rigidbody rigidbody, Vector3 direction)
    {
        if (rigidbody.isKinematic) return false;

        direction = direction.normalized;

        float width = 0.4f;
        float distance = 0.25f;
        RaycastHit hitInfo;

        if (direction.Equals(Vector3.down)) {
            width = 0.8f;
            distance = 0.25f;

            return Physics.BoxCast(rigidbody.position, new Vector3(width, width, width), direction.normalized, Quaternion.identity, distance, layerMask);
        }

        return Physics.SphereCast(rigidbody.position, width, direction.normalized, out hitInfo, distance, layerMask);
    }
}