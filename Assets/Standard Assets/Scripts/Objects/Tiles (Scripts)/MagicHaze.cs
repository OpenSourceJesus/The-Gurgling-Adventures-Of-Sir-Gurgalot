using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAoKR
{
    public class MagicHaze : MonoBehaviour
    {
        public virtual void OnTriggerEnter2D (Collider2D other)
        {
        	MagicScepter.instance.enabled = true;
        }

        public virtual void OnTriggerExit2D (Collider2D other)
        {
            MagicScepter.instance.enabled = false;
            MagicScepter.instance.CancelRecording ();
        }
    }
}