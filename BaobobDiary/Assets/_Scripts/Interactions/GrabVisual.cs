using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.DistanceReticles
{
    public class GrabVisual : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHand))]
        private UnityEngine.Object _handObject;
        private IHand _hand;

        [SerializeField]
        private DistanceHandGrabInteractor _distanceHandGrabInteractor;

        [SerializeField]
        private GameObject _handCircle;

        [SerializeField]
        private TubeRenderer _tubeRenderer;

        [SerializeField]
        private float _handCircleMinScale = 0.6f;

        [SerializeField]
        private float _handCircleMaxScale = 1f;

        [SerializeField]
        private Color _lineColor = Color.white;

        [SerializeField]
        private ChangeReticleIconGrabbed _grabbedObjectReticle;

        private void Awake()
        {
            _hand = _handObject as IHand;
        }

        private void Start()
        {
            Debug.Assert(_hand != null, "Hand object is not assigned or doesn't implement IHand.");
            Debug.Assert(_distanceHandGrabInteractor != null, "Distance Hand Grab Interactor is not assigned.");
            Debug.Assert(_handCircle != null, "Hand Circle is not assigned.");
            Debug.Assert(_tubeRenderer != null, "Tube Renderer is not assigned.");

            _tubeRenderer.Gradient = new Gradient
            {
                colorKeys = new GradientColorKey[] { new GradientColorKey(_lineColor, 0f), new GradientColorKey(_lineColor, 1f) }
            };
        }

        private void Update()
        {
            if (!_hand.IsTrackedDataValid || _distanceHandGrabInteractor.State == InteractorState.Disabled)
            {
                _handCircle.SetActive(false);
                _tubeRenderer.Hide();
                if (_grabbedObjectReticle != null)
                {
                    _grabbedObjectReticle.SetIconState(false);
                }
                return;
            }

            if (_distanceHandGrabInteractor.State == InteractorState.Select)
            {
                HandlePinchAndGrab();

            }
            else
            {
                _handCircle.SetActive(false);
                if (_grabbedObjectReticle != null)
                {
                    _grabbedObjectReticle.SetIconState(false);
                }
                _tubeRenderer.Hide();
                
            }
        }

        private void HandlePinchAndGrab()
        {
            if (!_hand.GetJointPose(HandJointId.HandIndex3, out var poseIndex3) || !_hand.GetJointPose(HandJointId.HandThumb3, out var poseThumb3))
                return;

            Vector3 midIndexThumb = Vector3.Lerp(poseThumb3.position, poseIndex3.position, 0.5f);

            

            var selectedInteractable = _distanceHandGrabInteractor.SelectedInteractable;
            if (selectedInteractable != null)
            {
                // Draw Line
                _tubeRenderer.enabled = true;
                UpdateTubeRenderer(midIndexThumb, selectedInteractable.transform.position);

                if (_grabbedObjectReticle != null)
                {
                    _grabbedObjectReticle.SetIconState(true);
                }

            }
            else
            {
                _tubeRenderer.Hide();

                if (_grabbedObjectReticle != null)
                {
                    _grabbedObjectReticle.SetIconState(false);
                }

            }
            
        }


        private void UpdateTubeRenderer(Vector3 startPoint, Vector3 endPoint)
        {
            TubePoint[] points = new TubePoint[]
            {
                new TubePoint { position = startPoint, rotation = Quaternion.identity, relativeLength = 0f },
                new TubePoint { position = endPoint, rotation = Quaternion.identity, relativeLength = 1f }
            };
            _tubeRenderer.RenderTube(points, Space.World);
        }
    }
}