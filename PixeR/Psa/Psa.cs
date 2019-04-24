using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Psa
{
    internal class Psa
    {
        private readonly double latitude;
        private readonly double longitude;
        private readonly int dia;
        private readonly int mes;
        private readonly int ano;
        private readonly int hora;
        private readonly int minuto;

        private double meridian = -3*15;
        private int correctedYear;
        private int correctedMonth;
        private double solarMinutesAfterMidnight;
        private double t, G, C, L, alpha, obliquity, declination, hourAngle, altitudeAngle;
        private double grausParaRadianos = 3.1415927 / 180;
        private double radianosParaGraus = 180 / 3.1415927;
        private double daylightAdjustment = 0;
        private double inputMinutesAfterMidnight;


        public Psa(double latitude, double longitude, int dia, int mes, int ano, int hora, int minuto)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.dia = dia;
            this.mes = mes;
            this.ano = ano;
            this.hora = hora;
            this.minuto = minuto;
        }


        public double GetAAzimute()
        {
            double azimuthAngle = radianosParaGraus * Math.Acos((
                        (Math.Sin(altitudeAngle * grausParaRadianos) *
                         Math.Sin(latitude * grausParaRadianos)) -
                         Math.Sin(declination * grausParaRadianos)) /
                        (Math.Cos(altitudeAngle * grausParaRadianos) *
                         Math.Cos(latitude * grausParaRadianos)));

            if (azimuthAngle * hourAngle < 0) { azimuthAngle *= -1; };

            return azimuthAngle;
        }

        public double GetAltura()
        {
            inputMinutesAfterMidnight = 60 * hora + minuto;
            solarMinutesAfterMidnight = 12 * 60;
            solarMinutesAfterMidnight = inputMinutesAfterMidnight + (4 * (longitude - meridian)) + daylightAdjustment;
            if (mes > 2)
            {
                correctedYear = ano;
                correctedMonth = mes - 3;
            }
            else
            {
                correctedYear = ano - 1;
                correctedMonth = mes + 9;
            }

            t = ((solarMinutesAfterMidnight / 60.0 / 24.0) + dia + Math.Floor(30.6 * correctedMonth + 0.5) + Math.Floor(365.25 * (correctedYear - 1976)) - 8707.5) / 36525.0;

            G = 357.528 + 35999.05 * t;
            G = NormalizeTo360(G);

            C = (1.915 * Math.Sin(G * grausParaRadianos)) + (0.020 * Math.Sin(2.0 * G * grausParaRadianos));

            L = 280.460 + (36000.770 * t) + C;
            L = NormalizeTo360(L);

            alpha = L - 2.466 * Math.Sin(2.0 * L * grausParaRadianos) + 0.053 * Math.Sin(4.0 * L * grausParaRadianos);

            obliquity = 23.4393 - 0.013 * t;

            declination = Math.Atan(Math.Tan(obliquity * grausParaRadianos) * Math.Sin(alpha * grausParaRadianos)) * radianosParaGraus;

            // hour angle
            hourAngle = (solarMinutesAfterMidnight - 12 * 60) / 4 * -1;

            // altitude angle  
            altitudeAngle = radianosParaGraus * Math.Asin(
                            (Math.Cos(latitude * grausParaRadianos) *
                             Math.Cos(declination * grausParaRadianos) *
                             Math.Cos(hourAngle * grausParaRadianos)) +
                            (Math.Sin(latitude * grausParaRadianos) *
                             Math.Sin(declination * grausParaRadianos)));

            return altitudeAngle;
        }


        public XYZ GetPosicao()
        {

            int r = 149600000; //distancia da terra com o sol
            double altura = GetAltura();
            double azimute = GetAAzimute();

            double x = r * Math.Cos(azimute) * Math.Sin(altura);
            double y = r * Math.Sin(azimute) * Math.Sin(altura); ;
            double z = r * Math.Cos(altura);

            XYZ posicao = new XYZ(x, y, z);

            return posicao;
        }

        public double NormalizeTo360(double x)
        {
            return (x - Math.Floor(x / 360.0) * 360);
        }
    }
}