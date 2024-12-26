// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Signals
{
    /// <summary> Specialized data container used to send meta signals with or without a value payload </summary>
    [Serializable]
    public class SignalPayload
    {
        /// <summary> Value payload of the signal </summary>
        public enum ValueType
        {
            /// <summary> No value </summary>
            None,

            /// <summary> Integer Value </summary>
            Integer,

            /// <summary> Boolean Value </summary>
            Boolean,

            /// <summary> Float Value  </summary>
            Float,

            /// <summary> String Value  </summary>
            String,

            /// <summary>Color Value </summary>
            Color,

            /// <summary> 2D vector Value </summary>
            Vector2,

            /// <summary> 3D vector Value </summary>
            Vector3,

            /// <summary> 4D vector Value </summary>
            Vector4
        }

        [SerializeField] private StreamId StreamId;
        [SerializeField] private ValueType SignalValueType;
        [SerializeField] private int IntegerValue;
        [SerializeField] private bool BooleanValue;
        [SerializeField] private float FloatValue;
        [SerializeField] private string StringValue;
        [SerializeField] private Color ColorValue;
        [SerializeField] private Vector2 Vector2Value;
        [SerializeField] private Vector3 Vector3Value;
        [SerializeField] private Vector4 Vector4Value;

        /// <summary> StreamId of the signal </summary>
        public StreamId streamId
        {
            get => StreamId;
            set => StreamId = value;
        }

        /// <summary> ValueType for the payload of the signal </summary>
        public ValueType signalValueType
        {
            get => SignalValueType;
            set => SignalValueType = value;
        }

        /// <summary> Integer value of the payload of the signal </summary>
        public int integerValue
        {
            get => IntegerValue;
            set
            {
                Reset();
                SignalValueType = ValueType.Integer;
                IntegerValue = value;
            }
        }
        
        /// <summary> Boolean value of the payload of the signal </summary>
        public bool booleanValue
        {
            get => BooleanValue;
            set
            {
                Reset();
                SignalValueType = ValueType.Boolean;
                BooleanValue = value;
            }
        }
        
        /// <summary> Float value of the payload of the signal </summary>
        public float floatValue
        {
            get => FloatValue;
            set
            {
                Reset();
                SignalValueType = ValueType.Float;
                FloatValue = value;
            }
        }
        
        /// <summary> String value of the payload of the signal </summary>
        public string stringValue
        {
            get => StringValue;
            set
            {
                Reset();
                SignalValueType = ValueType.String;
                StringValue = value;
            }
        }
        
        /// <summary> Color value of the payload of the signal </summary>
        public Color colorValue
        {
            get => ColorValue;
            set
            {
                Reset();
                SignalValueType = ValueType.Color;
                ColorValue = value;
            }
        }
        
        /// <summary> Vector2 value of the payload of the signal </summary>
        public Vector2 vector2Value
        {
            get => Vector2Value;
            set
            {
                Reset();
                SignalValueType = ValueType.Vector2;
                Vector2Value = value;
            }
        }

        /// <summary> Vector3 value of the payload of the signal </summary>
        public Vector3 vector3Value
        {
            get => Vector3Value;
            set
            {
                Reset();
                SignalValueType = ValueType.Vector3;
                Vector3Value = value;
            }
        }
        
        /// <summary> Vector4 value of the payload of the signal </summary>
        public Vector4 vector4Value
        {
            get => Vector4Value;
            set
            {
                Reset();
                SignalValueType = ValueType.Vector4;
                Vector4Value = value;
            }
        }

        /// <summary> Creates a new SignalPayload instance </summary>
        public SignalPayload()
        {
            Reset();
        }

        /// <summary> Reset to No Value </summary>
        public void Reset()
        {
            SignalValueType = ValueType.None;
            IntegerValue = default;
            BooleanValue = default;
            FloatValue = default;
            StringValue = default;
            ColorValue = default;
            Vector2Value = default;
            Vector3Value = default;
            Vector4Value = default;
        }


        /// <summary> Set int value </summary>
        /// <param name="value"> New value </param>
        public SignalPayload SetValue(int value)
        {
            integerValue = value;
            return this;
        }

        /// <summary> Set bool value </summary>
        /// <param name="value"> New value </param>
        public SignalPayload SetValue(bool value)
        {
            booleanValue = value;
            return this;
        }

        /// <summary> Set float value </summary>
        /// <param name="value"> New value </param>
        public SignalPayload SetValue(float value)
        {
            floatValue = value;
            return this;
        }

        /// <summary> Set string value </summary>
        /// <param name="value"> New value </param>
        public SignalPayload SetValue(string value)
        {
            stringValue = value;
            return this;
        }

        /// <summary> Set color value </summary>
        /// <param name="value"> New value </param>
        public SignalPayload SetValue(Color value)
        {
            colorValue = value;
            return this;
        }

        /// <summary> Set Vector2 value </summary>
        /// <param name="value"> New value </param>
        public SignalPayload SetValue(Vector2 value)
        {
            vector2Value = value;
            return this;
        }

        /// <summary> Set Vector3 value </summary>
        /// <param name="value"> New value </param>
        public SignalPayload SetValue(Vector3 value)
        {
            vector3Value = value;
            return this;
        }

        /// <summary> Set Vector4 value </summary>
        /// <param name="value"> New value </param>
        public SignalPayload SetValue(Vector4 value)
        {
            vector4Value = value;
            return this;
        }

        /// <summary> Sends a Signal with the set payload value to the stream with the given stream id </summary>
        public SignalPayload SendSignal()
        {
            
            if (StreamId.Category.Equals(SignalStream.k_DefaultCategory)) 
                return this; // No stream category set, no signal sent
            if(StreamId.Name.Equals(SignalStream.k_DefaultName)) 
                return this; // No stream name set, no signal sent
            
            SignalStream stream = 
                SignalsService.GetStream(StreamId.Category, StreamId.Name); // Get stream with the given stream id

            switch (SignalValueType)
            {
                case ValueType.None:
                    stream.SendSignal();
                    break;
                case ValueType.Integer:
                    stream.SendSignal(integerValue);
                    break;
                case ValueType.Boolean:
                    stream.SendSignal(booleanValue);
                    break; 
                case ValueType.Float:
                    stream.SendSignal(floatValue);
                    break;
                case ValueType.String:
                    stream.SendSignal(stringValue, "");
                    break;
                case ValueType.Color:
                    stream.SendSignal(colorValue);
                    break;
                case ValueType.Vector2:
                    stream.SendSignal(vector2Value);
                    break;
                case ValueType.Vector3:
                    stream.SendSignal(vector3Value);
                    break;
                case ValueType.Vector4:
                    stream.SendSignal(vector4Value);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        public override string ToString()
        {
            string message = "";
            message += SignalValueType switch
                       {
                           ValueType.None    => string.Empty,
                           ValueType.Integer => $"(int)",
                           ValueType.Boolean => $"(bool)",
                           ValueType.Float   => $"(float)",
                           ValueType.String  => $"(string)",
                           ValueType.Color   => $"(color)",
                           ValueType.Vector2 => $"(Vector2)",
                           ValueType.Vector3 => $"(Vector3)",
                           ValueType.Vector4 => $"(Vector4)",
                           _                 => throw new ArgumentOutOfRangeException()
                       };
            message += $" {streamId}";

            return message;
        }
    }
}
