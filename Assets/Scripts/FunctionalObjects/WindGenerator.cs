using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public sealed class WindGenerator : ElecticGenerator
{
    [SerializeField] private Transform _rotationVisualMined;
    [SerializeField] private Vector3 _vectorRotationVisual;

    protected new void Start()
    {
        base.Start();
    }

    private void LateUpdate()
    {
        if (IsMined && IsHasProductCopacity()) 
            _rotationVisualMined.Rotate(_vectorRotationVisual);

    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
