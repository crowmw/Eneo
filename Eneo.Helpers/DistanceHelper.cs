using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.Helpers
{
    public class DistanceHelper
    {
        private const double _toRad = Math.PI / 180;
        private const double _radiusEarthKM = 6371;
        private const double _radiusEarthMiles = 3959;
        private const double _milesToKM = 1.60934;

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        //public static double CalculateDistance(double latitudeFrom, double longitudeFrom, double? latitudeTo, double? longitudeTo)
        //{
        //    if (!latitudeTo.HasValue || !longitudeTo.HasValue)
        //        return 40000;

        //    double circumference = 40000.0; // Earth's circumference at the equator in km
        //    double distance = 0.0;

        //    //Calculate radians
        //    double latitude1Rad = DegreesToRadians(latitudeFrom);
        //    double longitude1Rad = DegreesToRadians(longitudeFrom);
        //    double latititude2Rad = DegreesToRadians(latitudeTo.Value);
        //    double longitude2Rad = DegreesToRadians(longitudeTo.Value);

        //    double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

        //    if (logitudeDiff > Math.PI)
        //    {
        //        logitudeDiff = 2.0 * Math.PI - logitudeDiff;
        //    }

        //    double angleCalculation =
        //        Math.Acos(
        //          Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
        //          Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

        //    distance = circumference * angleCalculation / (2.0 * Math.PI);

        //    return distance;
        //}



        public static double CalculateDistance(double latitudeFrom, double longitudeFrom, double? latitudeTo, double? longitudeTo)
        {
            if (!latitudeTo.HasValue || !longitudeTo.HasValue)
                return 40000;

            double _radLat1 = latitudeFrom * _toRad;
            double _radLat2 = latitudeTo.Value * _toRad;
                double _dLat = (_radLat2 - _radLat1);
                double _dLon = (longitudeFrom - longitudeTo.Value) * _toRad;

                double _a = (_dLon) * Math.Cos((_radLat1 + _radLat2) / 2);

                // central angle, aka arc segment angular distance
                double _centralAngle = Math.Sqrt(_a * _a + _dLat * _dLat);

                // great-circle (orthodromic) distance on Earth between 2 points
                return _radiusEarthMiles * _centralAngle * _milesToKM;
            
        }



    }
}
