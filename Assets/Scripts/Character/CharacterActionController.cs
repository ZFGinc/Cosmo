using UnityEngine;

public class CharacterActionController : MonoBehaviour, IActionController
{
    [SerializeField] private Transform _pickUpCheckerPivot;
    [SerializeField] private float _radiusChechActionObjects;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_pickUpCheckerPivot.position, _radiusChechActionObjects);
    }

    private T TryGetActionObject<T>() where T : IActionObejctBase
    {
        Collider[] hitColliders = Physics.OverlapSphere(_pickUpCheckerPivot.position, _radiusChechActionObjects);
        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out T pickableObject))
            {
                return pickableObject;
            }
        }

        return default(T);
    }

    private void TryPushObject()
    {
        IPushObject action = TryGetActionObject<IPushObject>();
        if (action == null) return;

        action.Action(transform);
    }

    public void Action()
    {
        IActionObject action = TryGetActionObject<IActionObject>();
        if (action == null)
        {
            TryPushObject();
            return;
        }

        action.Action();    
    }
}
