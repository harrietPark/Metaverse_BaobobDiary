using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.DistanceReticles
{
    public class ChangeReticleIconGrabbed : InteractorReticle<ReticleDataIcon>
    {
        [SerializeField, Interface(typeof(IDistanceInteractor))]
        private UnityEngine.Object _distanceInteractor;
        private IDistanceInteractor DistanceInteractor { get; set; }

        [SerializeField]
        private MeshRenderer _renderer;

        [SerializeField]
        private Transform _centerEye;

        [SerializeField]
        private Texture _defaultIcon;
        [SerializeField]
        private Texture _changedIcon;
        public Texture DefaultIcon
        {
            get
            {
                return _defaultIcon;
            }
            set
            {
                _defaultIcon = value;
            }
        }

        public Texture ChangedIcon
        {
            get
            {
                return _changedIcon;
            }
            set
            {
                _changedIcon = value;
            }
        }

        [SerializeField]
        private bool _constantScreenSize;
        public bool ConstantScreenSize
        {
            get
            {
                return _constantScreenSize;
            }
            set
            {
                _constantScreenSize = value;
            }
        }

        [SerializeField]
        private float _iconScaleFactor = 1.0f; // Adjust this value to scale the icon

        private Vector3 _originalScale;

        protected override IInteractorView Interactor { get; set; }
        protected override Component InteractableComponent => DistanceInteractor.DistanceInteractable as Component;

        #region Editor events
        protected virtual void OnValidate()
        {
            if (_renderer != null)
            {
                _renderer.sharedMaterial.mainTexture = _defaultIcon;
            }
        }
        #endregion

        protected virtual void Awake()
        {
            DistanceInteractor = _distanceInteractor as IDistanceInteractor;
            Interactor = DistanceInteractor;
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            this.AssertField(_renderer, nameof(_renderer));
            this.AssertField(_centerEye, nameof(_centerEye));
            _originalScale = this.transform.localScale;
            this.EndStart(ref _started);
        }

        protected override void Draw(ReticleDataIcon dataIcon)
        {
            if (dataIcon != null && dataIcon.CustomIcon != null)
            {
                _renderer.material.mainTexture = dataIcon.CustomIcon;
            }
            else
            {
                _renderer.material.mainTexture = _defaultIcon;
            }

            if (!_constantScreenSize)
            {
                _renderer.transform.localScale = _originalScale * dataIcon.GetTargetSize().magnitude * _iconScaleFactor;
            }
            _renderer.enabled = true;
        }

        protected override void Align(ReticleDataIcon data)
        {
            this.transform.position = data.ProcessHitPoint(DistanceInteractor.HitPoint);

            if (_renderer.enabled)
            {
                Vector3 dirToTarget = (_centerEye.position - transform.position).normalized;
                transform.LookAt(transform.position - dirToTarget, Vector3.up);

                if (_constantScreenSize)
                {
                    float distance = Vector3.Distance(transform.position, _centerEye.position);
                    _renderer.transform.localScale = _originalScale * distance * _iconScaleFactor;
                }
            }
        }

        protected override void Hide()
        {
            _renderer.enabled = false;
        }

        public void SetIconState(bool isPinched)
        {
            if (_renderer != null)
            {
                _renderer.sharedMaterial.mainTexture = isPinched ? _changedIcon : _defaultIcon;
            }
        }

        #region Inject
        public void InjectAllReticleIconDrawer(IDistanceInteractor distanceInteractor,
            Transform centerEye, MeshRenderer renderer, Texture defaultIcon, Texture changedIcon, float iconScaleFactor)
        {
            InjectDistanceInteractor(distanceInteractor);
            InjectCenterEye(centerEye);
            InjectRenderer(renderer);
            InjectDefaultIcon(defaultIcon);
            InjectChangedIcon(changedIcon);
            InjectIconScaleFactor(iconScaleFactor);
        }

        public void InjectDistanceInteractor(IDistanceInteractor distanceInteractor)
        {
            _distanceInteractor = distanceInteractor as UnityEngine.Object;
            DistanceInteractor = distanceInteractor;
            Interactor = distanceInteractor;
        }

        public void InjectCenterEye(Transform centerEye)
        {
            _centerEye = centerEye;
        }

        public void InjectRenderer(MeshRenderer renderer)
        {
            _renderer = renderer;
        }

        public void InjectDefaultIcon(Texture defaultIcon)
        {
            _defaultIcon = defaultIcon;
        }

        public void InjectChangedIcon(Texture changedIcon)
        {
            _changedIcon = changedIcon;
        }

        public void InjectIconScaleFactor(float iconScaleFactor)
        {
            _iconScaleFactor = iconScaleFactor;
        }
        #endregion
    }
}