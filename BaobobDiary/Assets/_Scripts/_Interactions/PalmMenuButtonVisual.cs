using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction
{
    public class PalmMenuButtonVisual : MonoBehaviour
    {
        [SerializeField] Color _defaultBackColor;
        [SerializeField] Color _defaultFrontColor;

        [SerializeField] Color _selectedBackColor;
        [SerializeField] Color _selectedFrontColor;

        [SerializeField] Image buttonImage;
        [SerializeField] RoundedBoxProperties frontPanel;
        [SerializeField] RoundedBoxProperties backPanel;
    void Start()
        {
            frontPanel.Color = _defaultFrontColor;
            backPanel.BorderColor = _defaultBackColor;
            buttonImage.color = _defaultBackColor;
        }

        public void PalmMenuButtonSelectedVisual()
        {
            frontPanel.Color = _selectedFrontColor;
            backPanel.BorderColor = _selectedBackColor;
            buttonImage.color = _selectedBackColor;
        }

        public void PalmMenuButtonDeselectedVisual()
        {
            frontPanel.Color = _defaultFrontColor;
            backPanel.BorderColor = _defaultBackColor;
            buttonImage.color = _defaultBackColor;
        }
    }

}
