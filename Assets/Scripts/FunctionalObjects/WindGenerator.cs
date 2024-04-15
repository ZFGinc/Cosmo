using UnityEngine;

[RequireComponent(typeof(PickableObject))]
public class WindGenerator : ElecticGenerator
{
    [SerializeField] private Transform _rotationVisualMined;
    [SerializeField] private Vector3 _vectorRotationVisual;

    protected new void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (IsWorking && IsHasProductCopacity()) 
            _rotationVisualMined.Rotate(_vectorRotationVisual);
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
