/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.Serialization
{
    [OnEnvironmentInit]
	public static partial class StringConvert_Mono
	{
        static StringConvert_Mono()
        {
            StringConverter.SubscribeConverter<Vector2>(typeof(Vector2StringConverter));
            StringConverter.SubscribeConverter<Vector3>(typeof(Vector3StringConverter));
            StringConverter.SubscribeConverter<Vector4>(typeof(Vector4StringConverter));
            StringConverter.SubscribeConverter<Rect>(typeof(RectStringConverter));
            StringConverter.SubscribeConverter<RectOffset>(typeof(RectOffsetStringConverter));
            StringConverter.SubscribeConverter<Color>(typeof(ColorStringConverter)); 
        }
        class Vector2StringConverter : StringConverter<Vector2>
        {
            public override string ConvertToString(Vector2 t)
            {
                return string.Format("{0}X{5}{1}{4}Y{5}{2}{3}",
                 leftBound,
                 t.x,
                 t.y,
                 rightBound,
                 dot,
                 colon
                 
                 );
            }
            public override bool TryConvert(string self, out Vector2 result)
            {
                string[] temp = self.Split(dot);
                if (temp.Length != 2 || !temp[0].Contains(leftBound.ToString()) || !temp[1].Contains(rightBound.ToString()))
                    throw new Exception("Parse Err Rect");
                float x, y;
                if (temp[0].Split(colon)[1].TryConvert<float>(out x) &&
                     temp[1].Split(colon)[1].Replace(rightBound.ToString(),"").TryConvert<float>(out y))
                {
                    result = new Vector2(x, y);
                    return true;
                }
                result = default(Vector2);
                return false;
            }
        }

        class Vector3StringConverter : StringConverter<Vector3>
        {
            public override string ConvertToString(Vector3 t)
            {
                return string.Format("{0}X{6}{1}{5}Y{6}{2}{5}Z{6}{3}{4}",
                 leftBound,
                 t.x,
                 t.y,
                 t.z,
                 rightBound,
                 dot,colon
                 );
            }

            public override bool TryConvert(string self, out Vector3 result)
            {
                string[] temp = self.Split(',');
                if (temp.Length != 3 || !temp[0].Contains(leftBound.ToString()) || !temp[2].Contains(rightBound.ToString()))
                    throw new System.Exception("Parse Err Rect");
                float x, y, z;
                if (temp[0].Split(colon)[1].TryConvert<float>(out x) &&
                     temp[1].Split(colon)[1].TryConvert<float>(out y) &&
                     temp[2].Split(colon)[1].Replace(rightBound.ToString(), "").TryConvert<float>(out z))
                {
                    result = new Vector3(x, y, z);
                    return true;
                }
                result = default(Vector3);
                return false;
            }
        }

        class Vector4StringConverter : StringConverter<Vector4>
        {
            public override string ConvertToString(Vector4 self)
            {
                return string.Format("{0}X{7}{1}{6}Y{7}{2}{6}Z{7}{3}{6}W{7}{4}{5}",
                leftBound,
                self.x,
                self.y,
                self.z,
                self.w,
                rightBound
                ,dot,colon
                );
            }

            public override bool TryConvert(string self, out Vector4 result)
            {
                string[] temp = self.Split(',');
                if (temp.Length != 4 || !temp[0].Contains(leftBound.ToString()) || !temp[3].Contains(rightBound.ToString()))
                    throw new System.Exception("Parse Err Rect");
                float x, y, z, w;
                if (temp[0].Split(colon)[1].TryConvert<float>(out x) &&
                     temp[1].Split(colon)[1].TryConvert<float>(out y) &&
                     temp[2].Split(colon)[1].TryConvert<float>(out z) &&
                     temp[3].Split(colon)[1].Replace(rightBound.ToString(), "").TryConvert<float>(out w))
                {
                    result = new Vector4(x, y, z, w);
                    return true;
                }
                result = default(Vector4);
                return false;
            }
        }

        class RectStringConverter : StringConverter<Rect>
        {
            public override string ConvertToString(Rect self)
            {
                return string.Format("{0}X{7}{1}{6}Y{7}{2}{6}W{7}{3}{6}H{7}{4}{5}",
                leftBound,
                self.x,
                self.y,
                self.width,
                self.height,
                rightBound
                ,dot,colon);
            }

            public override bool TryConvert(string self, out Rect result)
            {
                string[] temp = self.Split(',');
                if (temp.Length != 4 || !temp[0].Contains(leftBound.ToString()) || !temp[3].Contains(rightBound.ToString()))
                    throw new System.Exception("Parse Err Rect");
                float x, y, w, h;
                if (temp[0].Split(colon)[1].TryConvert<float>(out x) &&
                     temp[1].Split(colon)[1].TryConvert<float>(out y) &&
                     temp[2].Split(colon)[1].TryConvert<float>(out w) &&
                     temp[3].Split(colon)[1].Replace(rightBound.ToString(), "").TryConvert<float>(out h))
                {
                    result = new Rect(x, y, w, h);
                    return true;
                }
                result = default(Rect);
                return false;
            }
        }

        class RectOffsetStringConverter : StringConverter<RectOffset>
        {
            public override string ConvertToString(RectOffset self)
            {
                return string.Format("{0}L{7}{1}{6}R{7}{2}{6}T{7}{3}{6}B{7}{4}{5}",
                      leftBound,
                      self.left,
                      self.right,
                      self.top,
                      self.bottom,
                      rightBound
                      ,dot,colon);
            }

            public override bool TryConvert(string self, out RectOffset result)
            {
                string[] temp = self.Split(',');
                if (temp.Length != 4 || !temp[0].Contains(leftBound.ToString()) || !temp[3].Contains(rightBound.ToString()))
                    throw new System.Exception("Parse Err RectOffset");
                int L, R, T, B;
                if (temp[0].Split(colon)[1].TryConvert<int>(out L) &&
                     temp[1].Split(colon)[1].TryConvert<int>(out R) &&
                     temp[2].Split(colon)[1].TryConvert<int>(out T) &&
                     temp[3].Split(colon)[1].Replace(rightBound.ToString(), "").TryConvert<int>(out B))
                {
                    result = new RectOffset(L, R, T, B);
                    return true;
                }
                result = default(RectOffset);
                return false;
            }
        }
        class ColorStringConverter : StringConverter<Color>
        {
            public override string ConvertToString(Color self)
            {
                int r = Mathf.RoundToInt(self.r * 255.0f);
                int g = Mathf.RoundToInt(self.g * 255.0f);
                int b = Mathf.RoundToInt(self.b * 255.0f);
                int a = Mathf.RoundToInt(self.a * 255.0f);
                string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
                return hex;
            }

            public override bool TryConvert(string self, out Color result)
            {
                if (self.Length != 8) throw new System.Exception("Parse Err Color");
                byte br = byte.Parse(self.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte bg = byte.Parse(self.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte bb = byte.Parse(self.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                byte cc = byte.Parse(self.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                float r = br / 255f;
                float g = bg / 255f;
                float b = bb / 255f;
                float a = cc / 255f;
                result = new Color(r, g, b, a);
                return true;
            }
        }

    }
}
