﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Helpers;

namespace HoneySelectVR
{
    public class TransientHead : ProtectedBehaviour
    {
        private List<Renderer> rendererList = new List<Renderer>();
        private bool hidden = false;
        private Transform root;

        private Renderer[] m_tongues;
        private CharInfo avatar;
        private Transform headTransform;
        private Transform eyesTransform;
        
        public Transform Eyes { get
            {
                return eyesTransform;
            }
        }

        public bool Visible
        {
            get
            {
                return !hidden;
            }
            set
            {
                if (value)
                {
                    Console.WriteLine("SHOW");
                }
                else
                {
                    Console.WriteLine("HIDE");
                }
                SetVisibility(value);
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            avatar = GetComponent<CharInfo>();
            Reinitialize();
        }

        public void Reinitialize()
        {
            headTransform = GetHead(avatar);
            eyesTransform = GetEyes(avatar);

            root = avatar.objRoot.transform;
            m_tongues = root.GetComponentsInChildren<SkinnedMeshRenderer>().Where(renderer => renderer.name.StartsWith("cm_O_tang") || renderer.name == "cf_O_tang").Where(tongue => tongue.enabled).ToArray();

        }

        public static Transform GetHead(CharInfo human)
        {
            return human.chaBody.objHead.GetComponentsInParent<Transform>().First(t => t.name.StartsWith("c") && t.name.Contains("J_Head"));
        }


        public static Transform GetEyes(CharInfo human)
        {
            var eyes = human.chaBody.objHeadBone.transform.Descendants().FirstOrDefault(t => t.name.StartsWith("c") && t.name.EndsWith("J_FaceUp_tz"));
            if (!eyes)
            {
                VRLog.Info("Creating eyes");
                eyes = new GameObject("cf_J_FaceUp_tz").transform;
                eyes.SetParent(GetHead(human), false);
                eyes.transform.localPosition = new Vector3(0, 0.07f, 0.05f);
            } else
            {
                VRLog.Info("FOund eyes");
            }
            return eyes;
        }

        void SetVisibility(bool visible)
        {
            if (visible)
            {
                if (hidden)
                {
                    // enable
                    //Console.WriteLine("Enabling {0} renderers", rendererList.Count);
                    foreach (var renderer in rendererList)
                    {
                        if (renderer)
                        {
                            renderer.enabled = true;
                        }
                    }
                    foreach (var renderer in m_tongues)
                    {
                        if (renderer)
                        {
                            renderer.enabled = true;
                        }
                    }

                }
            }
            else
            {
                if (!hidden)
                {
                    m_tongues = root.GetComponentsInChildren<SkinnedMeshRenderer>().Where(renderer => renderer.name.StartsWith("cm_O_tang") || renderer.name == "cf_O_tang").Where(tongue => tongue.enabled).ToArray();

                    // disable
                    rendererList.Clear();
                    foreach (var renderer in headTransform.GetComponentsInChildren<Renderer>().Where(renderer => renderer.enabled))
                    {
                        rendererList.Add(renderer);
                        renderer.enabled = false;
                    }

                    foreach (var renderer in m_tongues)
                    {
                        renderer.enabled = false;
                    }
                }
            }

            hidden = !visible;
        }
    }
}
