using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Psa
{
    internal class Psa
    {
        private readonly double latitude;
        private readonly double longitude;
        private readonly int day;
        private readonly int month;
        private readonly int year;
        private readonly int hour;
        private readonly int minuto;

        private double meridian = -3*15;
        private int correctedYear;
        private int correctedMonth;
        private double solarMinutesAfterMidnight;
        private double t, G, C, L, alpha, obliquity, declination, hourAngle, altitudeAngle;
        private double degreesForRadians = 3.1415927 / 180;
        private double radiansForDegrees = 180 / 3.1415927;
        private double daylightAdjustment = 0;
        private double inputMinutesAfterMidnight;


        public Psa(double latitude, double longitude, int dia, int mes, int ano, int hora, int minuto)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.day = dia;
            this.month = mes;
            this.year = ano;
            this.hour = hora;
            this.minuto = minuto;
        }


        public double GetAAzimute()
        {
            double azimuthAngle = radiansForDegrees * Math.Acos((
                        (Math.Sin(altitudeAngle * degreesForRadians) *
                         Math.Sin(latitude * degreesForRadians)) -
                         Math.Sin(declination * degreesForRadians)) /
                        (Math.Cos(altitudeAngle * degreesForRadians) *
                         Math.Cos(latitude * degreesForRadians)));

            if (azimuthAngle * hourAngle < 0) { azimuthAngle *= -1; };

            return azimuthAngle;
        }

        public double GetAltura()
        {
            inputMinutesAfterMidnight = 60 * hour + minuto;
            solarMinutesAfterMidnight = 12 * 60;
            solarMinutesAfterMidnight = inputMinutesAfterMidnight + (4 * (longitude - meridian)) + daylightAdjustment;
            if (month > 2)
            {
                correctedYear = year;
                correctedMonth = month - 3;
            }
            else
            {
                correctedYear = year - 1;
                correctedMonth = month + 9;
            }

            t = ((solarMinutesAfterMidnight / 60.0 / 24.0) + day + Math.Floor(30.6 * correctedMonth + 0.5) + Math.Floor(365.25 * (correctedYear - 1976)) - 8707.5) / 36525.0;

            G = 357.528 + 35999.05 * t;
            G = NormalizeTo360(G);

            C = (1.915 * Math.Sin(G * degreesForRadians)) + (0.020 * Math.Sin(2.0 * G * degreesForRadians));

            L = 280.460 + (36000.770 * t) + C;
            L = NormalizeTo360(L);

            alpha = L - 2.466 * Math.Sin(2.0 * L * degreesForRadians) + 0.053 * Math.Sin(4.0 * L * degreesForRadians);

            obliquity = 23.4393 - 0.013 * t;

            declination = Math.Atan(Math.Tan(obliquity * degreesForRadians) * Math.Sin(alpha * degreesForRadians)) * radiansForDegrees;

            // hour angle
            hourAngle = (solarMinutesAfterMidnight - 12 * 60) / 4 * -1;

            // altitude angle  
            altitudeAngle = radiansForDegrees * Math.Asin(
                            (Math.Cos(latitude * degreesForRadians) *
                             Math.Cos(declination * degreesForRadians) *
                             Math.Cos(hourAngle * degreesForRadians)) +
                            (Math.Sin(latitude * degreesForRadians) *
                             Math.Sin(declination * degreesForRadians)));

            return altitudeAngle;
        }


        public XYZ GetPosition()
        {

            int r = 149600000; //distancia da terra com o sol
            double altitude = GetAltura();
            double azimuth = GetAAzimute();

            double x = r * Math.Cos(azimuth) * Math.Sin(altitude);
            double y = r * Math.Sin(azimuth) * Math.Sin(altitude); ;
            double z = r * Math.Cos(altitude);

            XYZ position = new XYZ(x, y, z);

            return position;
        }

        public double NormalizeTo360(double x)
        {
            return (x - Math.Floor(x / 360.0) * 360);
        }
    }
}