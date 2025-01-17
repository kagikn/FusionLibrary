﻿using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using System;

namespace FusionLibrary.Memory
{
    public struct WheelDimensions
    {
        public float TyreRadius;
        public float RimRadius;
        public float TyreWidth;
    };

    public unsafe static class VehicleControl
    {
        private static readonly int handbrakeOffset;
        private static readonly int handlingOffset;

        private static readonly int wheelsPtrOffset;

        private static readonly int wheelSteeringAngleOffset;
        private static readonly int wheelAngularVelocityOffset;
        private static readonly int wheelSuspensionCompressionOffset;
        private static readonly int wheelAngleOffset;

        private static readonly int deluxoTransformationOffset;
        private static readonly int deluxoFlyModeOffset;

        private static readonly int drawHandlerPtrOffset;
        private static readonly int streamRenderGfxPtrOffset;
        private static readonly int streamRenderWheelWidthOffset;

        static VehicleControl()
        {
            byte* addr = MemoryFunctions.FindPattern("\x8A\xC2\x24\x01\xC0\xE0\x04\x08\x81", "xxxxxxxxx");
            handbrakeOffset = addr == null ? 0 : *(int*)(addr + 19);

            addr = MemoryFunctions.FindPattern("\x3C\x03\x0F\x85\x00\x00\x00\x00\x48\x8B\x41\x20\x48\x8B\x88", "xxxx????xxxxxxx");
            handlingOffset = addr == null ? 0 : *(int*)(addr + 0x16);

            addr = MemoryFunctions.FindPattern("\x3B\xB7\x48\x0B\x00\x00\x7D\x0D", "xx????xx");
            wheelsPtrOffset = addr == null ? 0 : *(int*)(addr + 2) - 8;

            addr = MemoryFunctions.FindPattern("\x0F\x2F\x81\xBC\x01\x00\x00\x0F\x97\xC0\xEB\x00\xD1\x00", "xx???xxxxxx?x?");
            wheelSteeringAngleOffset = addr == null ? 0 : *(int*)(addr + 3);

            addr = MemoryFunctions.FindPattern("\x45\x0f\x57\xc9\xf3\x0f\x11\x83\x60\x01\x00\x00\xf3\x0f\x5c", "xxx?xxx???xxxxx");
            wheelSuspensionCompressionOffset = addr == null ? 0 : *(int*)(addr + 8);
            wheelAngleOffset = addr == null ? 0 : (*(int*)(addr + 8)) + 0x8;
            wheelAngularVelocityOffset = addr == null ? 0 : (*(int*)(addr + 8)) + 0xc;

            addr = MemoryFunctions.FindPattern("\xF3\x0F\x11\xB3\x00\x00\x00\x00\x44\x88\x00\x00\x00\x00\x00\x48\x85\xC9", "xxxx????xx?????xxx");
            deluxoTransformationOffset = addr == null ? 0 : *(int*)(addr + 4);
            deluxoFlyModeOffset = deluxoTransformationOffset == 0 ? 0 : deluxoTransformationOffset + 4;

            addr = MemoryFunctions.FindPattern("\x44\x0F\x2F\x43\x48\x45\x8D", "xxxxxxx");
            drawHandlerPtrOffset = addr == null ? 0 : *(addr + 4);

            addr = MemoryFunctions.FindPattern("\x4C\x8D\x48\x00\x80\xE1\x01", "xxx?xxx");
            streamRenderGfxPtrOffset = addr == null ? 0 : *(int*)(addr - 4);

            addr = MemoryFunctions.FindPattern("\x48\x89\x01\xB8\x00\x00\x80\x3F\x66\x44\x89\x51", "xxxxxxxxxxxx");
            streamRenderWheelWidthOffset = addr == null ? 0 : *(int*)(addr + 23);
        }

        public static void SetWheelSize(Vehicle vehicle, float size)
        {
            IntPtr? address = vehicle?.MemoryAddress;

            if (address == IntPtr.Zero)
            {
                return;
            }

            ulong CVeh_0x48 = *(ulong*)(address + 0x48);
            ulong CVeh_0x48_0x370 = *(ulong*)(CVeh_0x48 + 0x370);

            if ((UIntPtr)CVeh_0x48_0x370 == UIntPtr.Zero)
            {
                return;
            }

            *(float*)(CVeh_0x48_0x370 + 0x8) = size;
        }

        public static float GetWheelSize(Vehicle vehicle)
        {
            IntPtr? address = vehicle?.MemoryAddress;

            if (address == IntPtr.Zero)
            {
                return 1.0f;
            }

            ulong CVeh_0x48 = *(ulong*)(address + 0x48);
            ulong CVeh_0x48_0x370 = *(ulong*)(CVeh_0x48 + 0x370);

            if ((UIntPtr)CVeh_0x48_0x370 == UIntPtr.Zero)
            {
                return 1.0f;
            }

            return *(float*)(CVeh_0x48_0x370 + 0x8);
        }

        // note, visual only.
        public static void SetWheelWidth(Vehicle vehicle, float width)
        {
            IntPtr? address = vehicle?.MemoryAddress;

            if (address == IntPtr.Zero)
            {
                return;
            }

            ulong drawHandler = *(ulong*)(address + drawHandlerPtrOffset);
            ulong streamRenderGfx = *(ulong*)(drawHandler + (uint)streamRenderGfxPtrOffset);

            if (streamRenderGfx != 0 && width != 0.0f)
            {
                *(float*)(streamRenderGfx + (uint)streamRenderWheelWidthOffset) = width;
            }
        }

        public static float GetWheelWidth(Vehicle vehicle)
        {
            IntPtr? address = vehicle?.MemoryAddress;

            if (address == IntPtr.Zero)
            {
                return 0.0f;
            }

            ulong drawHandler = *(ulong*)(address + drawHandlerPtrOffset);
            ulong streamRenderGfx = *(ulong*)(drawHandler + (uint)streamRenderGfxPtrOffset);

            if (streamRenderGfx != 0)
            {
                return *(float*)(streamRenderGfx + (uint)streamRenderWheelWidthOffset);
            }

            return 0.0f;
        }

        public static ulong GetHandlingPtr(Vehicle vehicle)
        {
            if (handlingOffset == 0)
            {
                return 0;
            }

            ulong address = (ulong)vehicle.MemoryAddress;
            return *(ulong*)(address + (ulong)handlingOffset);
        }

        public static void SetSuspensionUpperLimit(Vehicle vehicle, float limit)
        {
            ulong ptr = GetHandlingPtr(vehicle);
            *(float*)(*(ulong*)ptr + 0xC8) = limit;
        }

        public static void SetSuspensionLowerLimit(Vehicle vehicle, float limit)
        {
            ulong ptr = GetHandlingPtr(vehicle);
            *(float*)(*(ulong*)ptr + 0xCC) = limit;
        }

        public static ulong GetWheelsPtr(Vehicle vehicle)
        {
            if (wheelsPtrOffset == 0 || !vehicle.NotNullAndExists())
            {
                return 0;
            }

            ulong address = (ulong)vehicle.MemoryAddress;
            return *(ulong*)(address + (ulong)wheelsPtrOffset);
        }

        public static void SetHandbrake(Vehicle vehicle, bool brake)
        {
            if (handbrakeOffset == 0)
            {
                return;
            }

            bool* address = (bool*)((ulong)vehicle.MemoryAddress + (ulong)handbrakeOffset);
            *address = brake;
        }

        public static bool GetHandbrake(Vehicle vehicle)
        {
            if (handbrakeOffset == 0)
            {
                return false;
            }

            bool* address = (bool*)((ulong)vehicle.MemoryAddress + (ulong)handbrakeOffset);
            return *address;
        }

        public static void SetDeluxoTransformation(Vehicle v, float transformation)
        {
            if (deluxoTransformationOffset == 0 || !v.NotNullAndExists())
            {
                return;
            }

            float* address = (float*)((ulong)v.MemoryAddress + (ulong)deluxoTransformationOffset);
            *address = transformation;
        }

        public static float GetDeluxoTransformation(Vehicle v)
        {
            if (deluxoTransformationOffset == 0 || !v.NotNullAndExists())
            {
                return -1f;
            }

            float* address = (float*)((ulong)v.MemoryAddress + (ulong)deluxoTransformationOffset);
            return *address;
        }

        public static void SetDeluxoFlyMode(Vehicle v, float mode)
        {
            if (deluxoFlyModeOffset == 0 || !v.NotNullAndExists())
            {
                return;
            }

            float* address = (float*)((ulong)v.MemoryAddress + (ulong)deluxoFlyModeOffset);
            *address = mode;
        }

        public static float GetDeluxoFlyMode(Vehicle v)
        {
            if (deluxoFlyModeOffset == 0 || !v.NotNullAndExists())
            {
                return -1f;
            }

            float* address = (float*)((ulong)v.MemoryAddress + (ulong)deluxoFlyModeOffset);
            return *address;
        }

        public static float GetMaxSteeringAngle(Vehicle vehicle)
        {
            ulong handlingAddr = GetHandlingPtr(vehicle);
            if (handlingAddr == 0)
            {
                return 0f;
            }

            float* addr = (float*)(handlingAddr + 0x0080);
            return *addr;
        }

        /*
         *  0 - front left
         *  1 - front right
         *  2 - rear left
         *  3 - rear right
         */
        public static float[] GetWheelRotationSpeeds(Vehicle handle)
        {
            ulong wheelPtr = GetWheelsPtr(handle);
            int numWheels = handle.Wheels.Count;

            float[] speeds = new float[numWheels];

            if (wheelAngularVelocityOffset == 0 || !handle.NotNullAndExists())
            {
                return speeds;
            }

            for (int i = 0; i < numWheels; i++)
            {
                ulong wheelAddr = *(ulong*)(wheelPtr + 0x008 * (ulong)i);
                speeds[i] = -*(float*)(wheelAddr + (ulong)wheelAngularVelocityOffset);
            }
            return speeds;
        }

        public static void SetWheelRotationSpeeds(Vehicle handle, float[] rotations)
        {
            ulong wheelPtr = GetWheelsPtr(handle);
            int numWheels = handle.Wheels.Count;

            if (wheelAngularVelocityOffset == 0 || !handle.NotNullAndExists())
            {
                return;
            }

            for (int i = 0; i < numWheels; i++)
            {
                ulong wheelAddr = *(ulong*)(wheelPtr + 0x008 * (ulong)i);
                *(float*)(wheelAddr + (ulong)wheelAngularVelocityOffset) = rotations[i];
            }
        }

        public static float[] GetWheelRotations(Vehicle handle)
        {
            ulong wheelPtr = GetWheelsPtr(handle);
            int numWheels = handle.Wheels.Count;

            float[] speeds = new float[numWheels];

            if (wheelAngleOffset == 0 || !handle.NotNullAndExists())
            {
                return speeds;
            }

            for (int i = 0; i < numWheels; i++)
            {
                ulong wheelAddr = *(ulong*)(wheelPtr + 0x008 * (ulong)i);
                speeds[i] = *(float*)(wheelAddr + (ulong)wheelAngleOffset);
            }
            return speeds;
        }

        public static void SetWheelRotation(Vehicle handle, int index, float rotation)
        {
            ulong wheelPtr = GetWheelsPtr(handle);
            int numWheels = handle.Wheels.Count;

            if (wheelAngleOffset == 0 || !handle.NotNullAndExists() || wheelPtr == 0 || numWheels == 0)
            {
                return;
            }

            ulong wheelAddr = *(ulong*)(wheelPtr + 0x008 * (ulong)index);
            *(float*)(wheelAddr + (ulong)wheelAngleOffset) = rotation;
        }

        public static float[] GetWheelCompressions(Vehicle handle)
        {
            ulong wheelPtr = GetWheelsPtr(handle);
            int numWheels = handle.Wheels.Count;

            float[] speeds = new float[numWheels];

            if (wheelSuspensionCompressionOffset == 0)
            {
                return speeds;
            }

            for (int i = 0; i < numWheels; i++)
            {
                ulong wheelAddr = *(ulong*)(wheelPtr + 0x008 * (ulong)i);
                speeds[i] = *(float*)(wheelAddr + (ulong)wheelSuspensionCompressionOffset);
            }
            return speeds;
        }

        public static void SetWheelCompression(Vehicle handle, int index, float rotation)
        {
            ulong wheelPtr = GetWheelsPtr(handle);

            if (wheelSuspensionCompressionOffset == 0)
            {
                return;
            }

            ulong wheelAddr = *(ulong*)(wheelPtr + 0x008 * (ulong)index);
            *(float*)(wheelAddr + (ulong)wheelSuspensionCompressionOffset + 4) = rotation;
        }

        public static WheelDimensions[] GetWheelDimensions(Vehicle handle)
        {
            ulong wheelPtr = GetWheelsPtr(handle);
            int numWheels = handle.Wheels.Count;

            WheelDimensions[] dimensionsSet = new WheelDimensions[numWheels];
            ulong offTyreRadius = 0x110;
            ulong offRimRadius = 0x114;
            ulong offTyreWidth = 0x118;

            for (int i = 0; i < numWheels; i++)
            {
                ulong wheelAddr = *(ulong*)(wheelPtr + 0x008 * (ulong)i);

                WheelDimensions dimensions = new WheelDimensions
                {
                    TyreRadius = *(float*)(wheelAddr + offTyreRadius),
                    RimRadius = *(float*)(wheelAddr + offRimRadius),
                    TyreWidth = *(float*)(wheelAddr + offTyreWidth)
                };
                dimensionsSet[i] = dimensions;
            }

            return dimensionsSet;
        }

        public static float[] GetTyreSpeeds(Vehicle handle)
        {
            int numWheels = handle.Wheels.Count;
            float[] rotationSpeed = GetWheelRotationSpeeds(handle);
            WheelDimensions[] dimensionsSet = GetWheelDimensions(handle);
            float[] wheelSpeeds = new float[numWheels];

            for (int i = 0; i < numWheels; i++)
            {
                wheelSpeeds[i] = rotationSpeed[i] * dimensionsSet[i].TyreRadius;
            }

            return wheelSpeeds;
        }

        public static float[] GetWheelSteeringAngles(Vehicle vehicle)
        {
            ulong wheelPtr = GetWheelsPtr(vehicle);
            int numWheels = vehicle.Wheels.Count;

            float[] array = new float[numWheels];

            if (wheelSteeringAngleOffset == 0)
            {
                return array;
            }

            for (int i = 0; i < numWheels; i++)
            {
                ulong wheelAddr = *(ulong*)(wheelPtr + 0x008 * (ulong)i);
                array[i] = *(float*)(wheelAddr + (ulong)wheelSteeringAngleOffset);
            }

            return array;
        }

        public static float GetLargestSteeringAngle(Vehicle v)
        {
            float largestAngle = 0.0f;
            float[] angles = GetWheelSteeringAngles(v);

            foreach (float angle in angles)
            {
                if (Math.Abs(angle) > Math.Abs(largestAngle))
                {
                    largestAngle = angle;
                }
            }

            return largestAngle;
        }

        public static float CalculateReduction(Vehicle vehicle)
        {
            Vector3 vel = vehicle.Velocity;
            Vector3 pos = vehicle.Position;
            Vector3 motion = vehicle.GetOffsetPosition(new Vector3(pos.X + vel.X, pos.Y + vel.Y, pos.Z + vel.Z));
            //if (motion.Y > 3)
            //{
            //    mult = (0.15f + ((float)Math.Pow((1.0f / 1.13f), ((float)Math.Abs(motion.Y) - 7.2f))));
            //    if (mult != 0) { mult = (float)Math.Floor(mult * 1000) / 1000; }
            //    if (mult > 1) { mult = 1; }
            //}
            //mult = (1 + (mult - 1) * 1.0f);

            float remap = vel.Length().Remap(0, 30, 0, 0.6f);
            return remap > 0.6f ? 0.6f : remap;
        }

        public static float CalculateDesiredHeading(Vehicle vehicle, float steeringAngle, float steeringMax, float desiredHeading, float reduction)
        {
            float correction = desiredHeading * reduction;

            Vector3 speedVector = Function.Call<Vector3>(Hash.GET_ENTITY_SPEED_VECTOR, vehicle, true);

            if (Math.Abs(speedVector.Y) > 3.0f)
            {
                Vector3 velocityWorld = vehicle.Velocity;
                Vector3 positionWorld = vehicle.Position;
                Vector3 travelWorld = velocityWorld + positionWorld;

                float steeringAngleRelX = speedVector.Y * -(float)Math.Sin(steeringAngle);
                float steeringAngleRelY = speedVector.Y * (float)Math.Cos(steeringAngle);
                Vector3 steeringWorld = vehicle.GetOffsetPosition(new Vector3(steeringAngleRelX, steeringAngleRelY, 0.0f));

                Vector3 travelNorm = (travelWorld - positionWorld).Normalized;
                Vector3 steerNorm = (steeringWorld - positionWorld).Normalized;
                float travelDir = (float)Math.Atan2(travelNorm.Y, travelNorm.X) + desiredHeading * reduction;
                float steerDir = (float)Math.Atan2(steerNorm.Y, steerNorm.X);

                correction = 2.0f * (float)Math.Atan2(Math.Sin(travelDir - steerDir), (float)Math.Cos(travelDir - steerDir));
            }
            if (correction > steeringMax)
            {
                correction = steeringMax;
            }

            if (correction < -steeringMax)
            {
                correction = -steeringMax;
            }

            return correction;
        }

        public static void GetControls(float limitRadians, out bool handbrake, out float throttle, out bool brake, out float steer)
        {
            handbrake = Game.IsControlJustPressed(Control.VehicleHandbrake);
            throttle = -Game.GetDisabledControlValueNormalized(Control.MoveUp);

            brake = Game.IsControlJustPressed(Control.MoveDown);
            float left = Game.GetDisabledControlValueNormalized(Control.MoveLeft).Remap(0, 1f, 0, limitRadians);
            steer = -left;
        }
    }
}
