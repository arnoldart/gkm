using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FootIKController : MonoBehaviour
{
    [System.Serializable]
    public class FootSettings
    {
        public Transform footBone;
        public Transform target;
        public Transform hint;
        public Vector3 rayOffset;
        public float rayDistance = 1f;
        public LayerMask groundLayer;
        [HideInInspector] public float currentHeight;
        [HideInInspector] public Vector3 smoothedPosition;
    }

    [Header("Settings")]
    [SerializeField] private float footElevation = 0.1f;
    [SerializeField] private float positionSmoothSpeed = 10f;
    [SerializeField] private float heightAdjustSmoothSpeed = 5f;
    [SerializeField] private float maxFootDistance = 0.5f;
    [SerializeField] private Vector2 footOffset = new Vector2(0.1f, 0.1f);

    [Header("References")]
    [SerializeField] private FootSettings leftFoot;
    [SerializeField] private FootSettings rightFoot;
    [SerializeField] private TwoBoneIKConstraint leftFootIK;
    [SerializeField] private TwoBoneIKConstraint rightFootIK;
    [SerializeField] private Transform characterRoot;

    private Vector3 leftFootInitialLocalPos;
    private Vector3 rightFootInitialLocalPos;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Simpan posisi awal untuk referensi
        leftFootInitialLocalPos = leftFoot.target.localPosition;
        rightFootInitialLocalPos = rightFoot.target.localPosition;
    }

    void Update()
    {
        UpdateFootPosition(leftFoot);
        UpdateFootPosition(rightFoot);
        UpdateIKWeights();
    }

    void UpdateFootPosition(FootSettings foot)
    {
        // Hitung posisi raycast berdasarkan posisi kaki di animasi
        Vector3 rayOrigin = foot.footBone.position + characterRoot.TransformVector(foot.rayOffset);
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, foot.rayDistance, foot.groundLayer))
        {
            // Hitung posisi target dengan offset
            Vector3 targetPosition = hit.point + 
                                   characterRoot.up * footElevation + 
                                   characterRoot.TransformDirection(new Vector3(footOffset.x, 0, footOffset.y));

            // Smoothing posisi
            foot.smoothedPosition = Vector3.Lerp(
                foot.smoothedPosition, 
                targetPosition, 
                Time.deltaTime * positionSmoothSpeed
            );

            // Smoothing ketinggian
            foot.currentHeight = Mathf.Lerp(
                foot.currentHeight, 
                hit.point.y, 
                Time.deltaTime * heightAdjustSmoothSpeed
            );

            // Terapkan posisi target
            foot.target.position = foot.smoothedPosition;
        }

        // Debug ray
        Debug.DrawRay(rayOrigin, Vector3.down * foot.rayDistance, Color.red);
    }

    void UpdateIKWeights()
    {
        // Hitung jarak antara kaki dan target
        float leftDistance = Vector3.Distance(leftFoot.footBone.position, leftFoot.target.position);
        float rightDistance = Vector3.Distance(rightFoot.footBone.position, rightFoot.target.position);

        // Update weight berdasarkan jarak
        leftFootIK.weight = Mathf.Clamp01(1 - (leftDistance / maxFootDistance));
        rightFootIK.weight = Mathf.Clamp01(1 - (rightDistance / maxFootDistance));
    }

    void OnAnimatorIK()
    {
        // Atur posisi hint untuk menghindari knee flipping
        if (leftFoot.hint != null)
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1);
            animator.SetIKHintPosition(AvatarIKHint.LeftKnee, leftFoot.hint.position);

        if (rightFoot.hint != null)
            animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1);
            animator.SetIKHintPosition(AvatarIKHint.RightKnee, rightFoot.hint.position);
    }
}
