namespace SABI
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class PLOT_RandomTransformDeviation : PLOT
    {
        [SerializeField]
        private Vector3 positionRange,
            rotationRange,
            scaleRange;

        public override void Execute()
        {
            // ---------------------------------------------------------------------------------------------

            float posX = Random.Range(
                transform.position.x - positionRange.x,
                transform.position.x + positionRange.x
            );
            float posY = Random.Range(
                transform.position.y - positionRange.y,
                transform.position.y + positionRange.y
            );
            float posZ = Random.Range(
                transform.position.z - positionRange.z,
                transform.position.z + positionRange.z
            );

            // ---------------------------------------------------------------------------------------------

            float rotX = Random.Range(
                transform.eulerAngles.x - rotationRange.x,
                transform.eulerAngles.x + rotationRange.x
            );
            float rotY = Random.Range(
                transform.eulerAngles.y - rotationRange.y,
                transform.eulerAngles.y + rotationRange.y
            );
            float rotZ = Random.Range(
                transform.eulerAngles.z - rotationRange.z,
                transform.eulerAngles.z + rotationRange.z
            );

            // ---------------------------------------------------------------------------------------------

            float scaleX = Random.Range(
                transform.localScale.x - scaleRange.x,
                transform.localScale.x + scaleRange.x
            );
            float scaleY = Random.Range(
                transform.localScale.y - scaleRange.y,
                transform.localScale.y + scaleRange.y
            );
            float scaleZ = Random.Range(
                transform.localScale.z - scaleRange.z,
                transform.localScale.z + scaleRange.z
            );

            // ---------------------------------------------------------------------------------------------

            transform.position = new Vector3(posX, posY, posZ);
            transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, rotZ));
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT_RandomTransformDeviation))]
    public class PLOT_RandomTransformDeviationEditor : PLOTEditor { }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
