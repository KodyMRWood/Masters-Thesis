using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Thesis.HUD
{
    public class HUDController : MonoBehaviour
    {
        //Singleton?
        public static HUDController manager;//**

        //Properties
        public TextMeshProUGUI text;
        public TextMeshProUGUI Text
        {
            get => text;
            set => text = value;
        }

        private void Awake()
        {
            manager = this;//**
            text = this.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetHUDText (string newText)
        {
            Text.text = newText; 
        }
    }
}
