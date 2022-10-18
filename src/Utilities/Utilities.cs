using UnityEngine;

using MelonLoader;

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using SLZ.AI;
using SLZ.Combat;

namespace NEP.Hitmarkers.Utilities
{
    public static class Utilities
    {
        public static string textureDir = MelonLoader.MelonUtils.UserDataDirectory + "/Hitmarkers/Textures/";

        public static Texture2D LoadFromFile(string src)
        {
            Texture2D texture = new Texture2D(2, 2);

            if (File.Exists(src))
            {
                byte[] data = File.ReadAllBytes(src);
                ImageConversion.LoadImage(texture, data, false);

                return texture;
            }

            return null;
        }
    }

    internal static class HealthPatch
    {
        public static Action<int, Attack> OnTakeDamage;

        private static HealthPatchDelegate _original;
        private delegate void HealthPatchDelegate(IntPtr instance, int m, IntPtr attack, IntPtr method);

        public static unsafe void Patch()
        {
            HealthPatchDelegate patch = Patch;

            string nativeInfoName = "NativeMethodInfoPtr_TakeDamage_Public_Single_Int32_Attack_0";
            var tgtPtr = *(IntPtr*)(IntPtr)typeof(PuppetMasta.SubBehaviourHealth).GetField(nativeInfoName, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var dstPtr = patch.Method.MethodHandle.GetFunctionPointer();

            MelonUtils.NativeHookAttach((IntPtr)(&tgtPtr), dstPtr);
            _original = Marshal.GetDelegateForFunctionPointer<HealthPatchDelegate>(tgtPtr);
        }

        public static void Patch(IntPtr instance, int m, IntPtr attack, IntPtr method)
        {
            unsafe
            {
                try
                {
                    Attack_ refAttack = *(Attack_*)attack;

                    Attack _attack = new Attack()
                    {
                        damage = refAttack.damage,
                        normal = refAttack.normal,
                        origin = refAttack.origin,
                        backFacing = refAttack.backFacing == 1 ? true : false,
                        OrderInPool = refAttack.OrderInPool,
                        collider = refAttack.Collider,
                        proxy = refAttack.Proxy
                    };

                    OnTakeDamage?.Invoke(m, _attack);
                }
                catch(Exception e)
                {
                    MelonLogger.Error(e);
                }
                
            }

            _original(instance, m, attack, method);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Attack_
        {
            public float damage;
            public Vector3 normal;
            public Vector3 origin;
            public Vector3 force;
            public byte backFacing;
            public int OrderInPool;
            public IntPtr collider;
            public IntPtr proxy;

            public Collider Collider
            {
                get => new Collider(collider);
                set => collider = value.Pointer;
            }

            public TriggerRefProxy Proxy
            {
                get => new TriggerRefProxy(proxy);
                set => proxy = value.Pointer;
            }
        }
    }
}
