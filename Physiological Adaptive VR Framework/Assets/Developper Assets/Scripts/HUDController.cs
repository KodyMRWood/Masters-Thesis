using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Thesis.HUD
{
    public class HUDController : MonoBehaviour
    {
        //Singleton?
        public static HUDController hudText;//**
        //public static HUDController counterText;

        //Properties
        [SerializeField]private TextMeshProUGUI text;
        public TextMeshProUGUI Text
        {
            get => text;
            set => text = value;
        }

        [SerializeField] private TextMeshProUGUI counter;
        public TextMeshProUGUI Counter
        {
            get => counter;
            set => counter = value;
        }

        private void Awake()
        {
            hudText = this;//**
            //text = this.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetHUDText (string newText)
        {
            Text.text = newText; 
        }

        public void SetCounterText(string newText)
        {
            Counter.text = newText;
        }
    }
}
