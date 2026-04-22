using UnityEngine;

namespace SABI
{
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Returns a new Quaternion with the Euler X angle set to the specified degrees,
        /// preserving the current Y and Z Euler angles.
        /// </summary>
        public static Quaternion WithEulerX(this Quaternion q, float xDegrees)
        {
            Vector3 currentEuler = q.eulerAngles;
            return Quaternion.Euler(xDegrees, currentEuler.y, currentEuler.z);
        }

        /// <summary>
        /// Returns a new Quaternion with the Euler Y angle set to the specified degrees,
        /// preserving the current X and Z Euler angles.
        /// </summary>
        public static Quaternion WithEulerY(this Quaternion q, float yDegrees)
        {
            Vector3 currentEuler = q.eulerAngles;
            return Quaternion.Euler(currentEuler.x, yDegrees, currentEuler.z);
        }

        /// <summary>
        /// Returns a new Quaternion with the Euler Z angle set to the specified degrees,
        /// preserving the current X and Y Euler angles.
        /// </summary>
        public static Quaternion WithEulerZ(this Quaternion q, float zDegrees)
        {
            Vector3 currentEuler = q.eulerAngles;
            return Quaternion.Euler(currentEuler.x, currentEuler.y, zDegrees);
        }
    }
}
