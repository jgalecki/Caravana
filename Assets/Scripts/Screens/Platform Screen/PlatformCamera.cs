using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Platform_Screen
{
    class PlatformCamera : MonoBehaviour
    {
        public GameObject FollowThis;
        public Vector3 _offset;
        //public Vector3 UIViewportOffset;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            var follow = FollowThis.transform.position;
            transform.position = new Vector3(follow.x, follow.y, transform.position.z);
        }

        // LateUpdate is called after Update each frame
        void LateUpdate()
        {
            //transform.position = FollowThis.transform.position;
        }
    }
}
